namespace Pfps.API.Data
{
    public class Audit
    {
        public int Id { get; set; }

        public User Instigator { get; set; }
        public AuditEvent Type { get; set; }
        public string Message { get; set; }

        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }

    public enum AuditEvent
    {
        UploadApproval,
        UploadDisapproval,
        UploadDeletion,
        UploadUpdate,
        UserUpdate,
        UserDeletion,
    }
}