namespace Pfps.API.Data
{
    public class Notification
    {
        public Guid Id { get; set; }

        public NotificationType Type { get; set; }

        public User User { get; set; }

        public string Moderator { get; set; }
        public Guid ModeratorId { get; set; }

        public string UploadTitle { get; set; }
        public Guid UploadId { get; set; }

        public string Message { get; set; } = "No message provided.";
    }

    public enum NotificationType
    {
        DISAPPROVAL,
        APPROVAL,
        DELETION,
    }
}