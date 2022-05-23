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
        private readonly ILogger<UserController> _log;
        private readonly IPasswordHasher<User> _hash;
        private readonly IDiscordOAuth2Service _discord;

        public UserController(PfpsContext ctx, ILogger<UserController> log, IPasswordHasher<User> hash, IDiscordOAuth2Service discord)
        {
            _ctx = ctx;
            _log = log;
            _hash = hash;
            _discord = discord;
        }

        // TODO: Verify ReCAPTCHA in production
        /*
            Register new user with username, password, and email.

            ReCAPTCHA response is required.
        */
        [HttpPost("/api/v1/register")]
        [ReCaptchaValidation]
        public async Task<IActionResult> RegisterUserAsync([FromBody] ApiUserModel model)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // Validate that user with same username or email does not exist
            if (await _ctx.Users.AnyAsync(x => x.Username == model.Username)
                || await _ctx.Users.AnyAsync(x => x.Email == model.Email))
            {
                return BadRequest(new
                {
                    code = 400,
                    error = "A user with that username or email already exists!",
                });
            }

            // Create user object
            var user = new User()
            {
                Username = model.Username,
                Email = model.Email,
            };

            user.Password = _hash.HashPassword(user, model.Password);

            await _ctx.Users.AddAsync(user);
            await _ctx.SaveChangesAsync();

            return Ok(UserViewModel.From(user));
        }

        /*
            Login and retreive token of regular account with username and password
        */
        [HttpPost("/api/v1/login")]
        [ReCaptchaValidation]
        public async Task<IActionResult> LoginUserAsync([FromBody] ApiUserModel model)
        {
            var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
            if (user != null && user.DiscordUser == true)
            {
                return BadRequest(new
                {
                    code = 400,
                    error = "Requested user has created an account with Discord. Please login using Discord.",
                });
            }

            if (user != null && _hash.VerifyHashedPassword(user, user.Password, model.Password) == PasswordVerificationResult.Success)
            {
                return Ok(new
                {
                    token = user.Token,
                });
            }

            return Unauthorized(new
            {
                code = 401,
                error = "Username or password is incorrect. Please try again.",
            });
        }

        /*
            Login to discord (if account is already created with discord account it will return token and not create new account)
        */
        [HttpGet("/api/v1/discord/login")]
        public async Task<IActionResult> DiscordLoginAsync([FromQuery] string code)
        {
            var oauth2TokenResult = await _discord.OAuth2TokenRequestAsync(code);
            if (oauth2TokenResult.Access_Token == null)
            {
                return BadRequest(new
                {
                    code = 400,
                    error = "Invalid OAuth2 Code or Discord API Error",
                });
            }

            var discordUser = await _discord.GetDiscordUserAsync(oauth2TokenResult.Access_Token);
            if (discordUser.Id == null)
            {
                return BadRequest(new
                {
                    code = 400,
                    error = "Error retrieving Discord User",
                });
            }

            if (await _ctx.Users.AnyAsync(x => x.Password == discordUser.Id && x.DiscordUser == true))
            {
                var usr = await _ctx.Users.FirstOrDefaultAsync(x => x.Password == discordUser.Id && x.DiscordUser == true);
                return Ok(new
                {
                    token = usr.Token,
                });
            }

            var user = new User()
            {
                Username = discordUser.Username,
                Password = discordUser.Id,
                Email = discordUser.Email,
                DiscordUser = true,
            };

            await _ctx.Users.AddAsync(user);
            await _ctx.SaveChangesAsync();

            return Ok(new
            {
                token = user.Token,
            });
        }

        /*
            Get Current User Object with Authorization
        */
        [HttpGet("/api/v1/users/@me")]
        [PfpsAuthorized]
        public IActionResult GetSelf()
        {
            return Ok(UserViewModel.From(base.PfpsUser));
        }

        [HttpGet("/api/v1/users/@me/notifications")]
        [PfpsAuthorized]
        public async Task<IActionResult> GetNotificationsAsync([FromQuery] int limit = 10, [FromQuery] int page = 0)
        {
            var userId = base.PfpsUser.Id;

            var user = await _ctx.Users
                .AsNoTracking()
                .Where(x => x.Id == userId)
                .Include(x => x.Notifications)
                .Skip(page * limit)
                .Take(10)
                .FirstOrDefaultAsync();

            var notifications = user.Notifications
                .Select(x => NotificationViewModel.From(x));

            return Ok(notifications);
        }

        [HttpGet("/api/v1/users/@me/favorites")]
        [PfpsAuthorized]
        public async Task<IActionResult> GetFavoritesAsync([FromQuery] int limit = 10, [FromQuery] int page = 0)
        {
            var userId = base.PfpsUser.Id;

            var favorites = await _ctx.Favorites
                .Where(x => x.UserId == userId)
                .Skip(page * limit)
                .Take(10)
                .Include(x => x.Upload)
                .Select(x => UploadSimplifiedViewModel.From(x.Upload))
                .ToListAsync();

            return Ok(favorites);
        }

        [HttpDelete("/api/v1/notifications/{id}")]
        [PfpsAuthorized]
        public async Task<IActionResult> DismissNotificationAsync(Guid id)
        {
            var notification = await _ctx.Notifications.FirstOrDefaultAsync(x => x.Id == id);

            if (notification == null)
            {
                return NotFound();
            }

            _ctx.Notifications.Remove(notification);
            await _ctx.SaveChangesAsync();

            return NoContent();
        }

        /*
            Disable when database is set up.
        */
        [HttpGet("/api/v1/users/@me/admin")]
        [PfpsAuthorized]
        public async Task<IActionResult> GiveSelfAdminAsync()
        {
            base.PfpsUser.Flags = Flags.ADMINISTRATOR;
            await _ctx.SaveChangesAsync();

            return NoContent();
        }
    }
}