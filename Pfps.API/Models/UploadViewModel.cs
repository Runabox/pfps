using Pfps.API.Data;

namespace Pfps.API.Models
{
    public class UploadViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public PublicUserViewModel Uploader { get; set; }
        public bool IsApproved { get; set; }
        public string[] Urls { get; set; }

        public DateTime Timestamp { get; set; }
        public int Views { get; set; }
        public UploadType Type { get; set; }
    }
}