using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using Pfps.API.Models;

namespace Pfps.API.Data
{
    public class Upload
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; } = "No Description.";
        public bool IsApproved { get; set; } = false;
        public int Views { get; set; } = 0;

        public string[] Urls { get; set; }
        public Guid[] TagIds { get; set; }
        public string[] Tags { get; set; }

        public UploadType Type { get; set; }
        public User Uploader { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public enum UploadType
    {
        PFP_SINGLE,
        PFP_MATCHING,
        PFP_MULTIPLE,
        BANNER,
    }

    public enum OrderType
    {
        DESCENDING,
        POPULAR
    }
}