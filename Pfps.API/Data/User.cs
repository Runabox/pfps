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
        public string Token { get; set; } = CreateToken();

        public bool HasLinkedDiscord { get; set; } // defaults to false

        public string Username { get; set; }
        public string Password { get; set; } // If DiscordUser is true this is the Discord User Id, otherwise it's the password hash ... interesting
        public string Email { get; set; }

        public virtual ICollection<Favorite> Favorites { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Upload> Uploads { get; set; }

        public Guid? Avatar { get; set; }

        public UserFlags Flags { get; set; } = UserFlags.None;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static string CreateToken()
        {
            return string.Concat(Enumerable.Repeat(1, 2)
                .Select(x => Guid.NewGuid().ToString("N")));
        }
    }

    [Flags]
    public enum UserFlags : ushort
    {
        None = 0,
        Premium = 1,
        ContentModerator = 2,
        Administrator = 4,
    }
}