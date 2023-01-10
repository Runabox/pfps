using Pfps.API.Data;

namespace Pfps.API.Models
{
    public class UserViewModel : PublicUserViewModel
    {
        public string DiscordId { get; set; }
        public bool HasLinkedDiscord { get; set; }
    }

    public class PublicUserViewModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }
        public Guid? Avatar { get; set; }
        public UserFlags Flags { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}