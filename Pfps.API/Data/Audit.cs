namespace Pfps.API.Data
{
    public class Audit
    {
        public int Id { get; set; }

        public User Instigator { get; set; }
        public AuditEvent Type { get; set; }
        public string Message { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public enum AuditEvent
    {
        UPLOAD_APPROVAL,
        UPLOAD_DISAPPROVAL,
        UPLOAD_DELETION,
        UPLOAD_UPDATE,
        USER_UPDATE,
        USER_DELETION,
    }
}