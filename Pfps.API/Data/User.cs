using Fare;

namespace Pfps.API.Data
{
    public class User
    {
        public User()
        {
            Favorites = new HashSet<Favorite>();
            Notifications = new HashSet<Notification>();
            Uploads = new HashSet<Upload>();
        }

        public Guid Id { get; set; }
        public string Token { get; set; } = GenerateToken();

        public bool DiscordUser { get; set; } = false;

        public string Username { get; set; }
        public string Password { get; set; } // If DiscordUser is true this is the Discord User Id, otherwise it's the password hash.
        public string Email { get; set; }

        public virtual ICollection<Favorite> Favorites { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Upload> Uploads { get; set; }

        public Guid Avatar { get; set; } = Guid.Empty;

        public Flags Flags { get; set; } = Flags.NONE;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static string GenerateToken()
        {
            var xeger = new Xeger(@"[a-zA-Z0-9]{24}\.[a-zA-Z0-9]{4}\.[a-zA-Z0-9]{27}");
            return xeger.Generate();
        }
    }

    [Flags]
    public enum Flags : ushort
    {
        NONE = 0,
        PREMIUM = 1,
        CONTENT_MODERATOR = 2,
        ADMINISTRATOR = 4,
    }
}