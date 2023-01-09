using Pfps.API.Data;

namespace Pfps.API.Models;

public class NotificationViewModel
{
    public Guid Id { get; set; }

    public NotificationType Type { get; set; }
    public string Message { get; set; }
    public NotificationModeratorModel Moderator { get; set; }
    public NotificationUploadModel Upload { get; set; }
    public PublicUserViewModel User { get; set; }
}

public class NotificationModeratorModel
{
    public Guid Id { get; set; }
    public string Username { get; set; }
}

public class NotificationUploadModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
}