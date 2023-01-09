using Pfps.API.Data;

namespace Pfps.API.Models
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public Guid? Avatar { get; set; }
        public UserFlags Flags { get; set; }
        public DateTime Timestamp { get; set; }
        public bool DiscordUser { get; set; }
        public string DiscordId { get; set; }

        public static UserViewModel From(User user)
        {
            if (user.HasLinkedDiscord == true)
            {
                return new UserViewModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    DiscordId = user.Password,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    Flags = user.Flags,
                    Timestamp = user.Timestamp,
                    DiscordUser = user.HasLinkedDiscord,
                };
            }

            return new UserViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Avatar = user.Avatar,
                Flags = user.Flags,
                Timestamp = user.Timestamp,
                DiscordUser = user.HasLinkedDiscord,
            };
        }
    }

    public class PublicUserViewModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }
        public Guid? Avatar { get; set; }
        public UserFlags Flags { get; set; }
        public DateTime Timestamp { get; set; }

        public static PublicUserViewModel From(User user)
        {
            return new PublicUserViewModel()
            {
                Id = user.Id,
                Username = user.Username,
                Avatar = user.Avatar,
                Flags = user.Flags,
                Timestamp = user.Timestamp,
            };
        }
    }
}