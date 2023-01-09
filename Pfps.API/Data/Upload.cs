namespace Pfps.API.Data
{
    public class Upload
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; } = "None";
        public bool IsApproved { get; set; }
        public int Views { get; set; }

        public string[] Urls { get; set; }
        public string[] Tags { get; set; }

        public UploadType Type { get; set; }
        public User Uploader { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }

    public enum UploadType
    {
        Single,
        Matching,
        Multiple,
        Banner,
    }

    public enum OrderType
    {
        Descending,
        Popular
    }
}