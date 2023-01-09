using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pfps.API.Data;
using Pfps.API.Models;
using Pfps.API.Services;
using Pfps.Filters;

namespace Pfps.API.Controllers
{
    public class UserController : PfpsControllerBase
    {
        private readonly PfpsContext _ctx;
        private readonly IPasswordHasher<User> _hash;
        private readonly IDiscordService _discord;
        private readonly IMapper _mapper;

        public UserController(PfpsContext ctx, IMapper mapper, IPasswordHasher<User> hash, IDiscordService discord)
        {
            _ctx = ctx;
            _hash = hash;
            _discord = discord;
            _mapper = mapper; ;
        }

        // TODO: Verify ReCAPTCHA in production

        /// <summary>
        /// Creates a new user using email, username, and password.
        /// </summary>
        /// <param name="model">The details for the new user to be created</param>
        [ReCaptchaValidation]
        [HttpPost("/api/v1/register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // Validate that user with same username or email does not exist
            if (await _ctx.Users.AnyAsync(x => x.Username == model.Username)
                || await _ctx.Users.AnyAsync(x => x.Email == model.Email))
                return Error("A user with that username or email already exists!");
            
            // Create user object
            var user = new User()
            {
                Email = model.Email,
                Username = model.Username
            };

            user.Password = _hash.HashPassword(user, model.Password);
            await _ctx.Users.AddAsync(user);
            await _ctx.SaveChangesAsync();

            return Ok(_mapper.Map<UserViewModel>(user));
        }


        /// <summary>
        /// Authenticates a user via their username and password.
        /// </summary>
        /// <param name="model">The user login details</param>
        [ReCaptchaValidation]
        [HttpPost("/api/v1/login")]
        public async Task<IActionResult> LoginUserAsync([FromBody] RegisterModel model)
        {
            var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
            if (user != null && user.HasLinkedDiscord == true)
                return Error("Requested user has created an account with Discord. Please login using Discord.");

            if (user != null && _hash.VerifyHashedPassword(user, user.Password, model.Password) == PasswordVerificationResult.Success)
                return Ok(new { user.Token });

            return Error("Username or password is incorrect. Please try again.", 401);
        }

        /// <summary>
        /// Logins the user via Discord; if an account doesn't exist, one will be created for them.
        /// </summary>
        /// <param name="code">The Discord OAuth2 response code</param>
        /// <returns>A token for the user that's been logged in and/or registered.</returns>
        [HttpGet("/api/v1/discord/login")]
        public async Task<IActionResult> DiscordLoginAsync([FromQuery] string code)
        {
            var tokenResult = await _discord.GetAccessToken(code);
            if (tokenResult == null) // now it does
                return Error("Invalid OAuth2 Code or Discord API Error");

            var discordUser = await _discord.GetDiscordUserAsync(tokenResult.Token);
            var usr = await _ctx.Users.FirstOrDefaultAsync(x => x.Password == discordUser.Id && x.HasLinkedDiscord == true);
            if (usr != null)
                return Ok(new { usr.Token });

            var user = new User()
            {
                Username = discordUser.Username,
                Password = discordUser.Id,
                Email = discordUser.Email,
                HasLinkedDiscord = true,
            };

            await _ctx.Users.AddAsync(user);
            await _ctx.SaveChangesAsync();

            return Ok(new { user.Token });
        }

        /// <summary>
        /// Requests the current user's profile.
        /// </summary>
        /// <returns>A UserViewModel object representing the currently authenticated user</returns>
        [PfpsAuthorized]
        [HttpGet("/api/v1/users/@me")]
        public IActionResult GetCurrentUser() =>
            Ok(_mapper.Map<UserViewModel>(base.PfpsUser));

        /// <summary>
        /// Requests the current user's active notifications
        /// </summary>
        [PfpsAuthorized]
        [HttpGet("/api/v1/users/@me/notifications")]
        public async Task<IActionResult> GetNotificationsAsync()
        {
            var notifications = await _ctx.Notifications
                .Where(x => x.User.Id == PfpsUser.Id)
                .ProjectTo<NotificationViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(notifications);
        }

        /// <summary>
        /// Requests the current user's favorited avatars
        /// </summary>
        [PfpsAuthorized]
        [HttpGet("/api/v1/users/@me/favorites")]
        public async Task<IActionResult> GetFavoritesAsync()
        {
            var favorites = await _ctx.Favorites
                .Where(x => x.UserId == PfpsUser.Id)
                .Include(x => x.Upload)
                .Select(x => x.Upload)
                .ProjectTo<UploadViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(favorites);
        }

        /// <summary>
        /// Dismisses a notification from the user's notification queue.
        /// </summary>
        /// <param name="id">The notification ID to delete</param>
        [PfpsAuthorized]
        [HttpDelete("/api/v1/notifications/{id}")]
        public async Task<IActionResult> DismissNotificationAsync(Guid id)
        {
            var notification = await _ctx.Notifications.FindAsync(id);

            if (notification == null)
                return NotFound();

            if (notification.User.Id != PfpsUser.Id)
                return Error("This notification isn't yours!");

            _ctx.Notifications.Remove(notification);
            await _ctx.SaveChangesAsync();

            return NoContent();
        }
    }
}