using Pfps.API.Data;
using System.ComponentModel.DataAnnotations;

namespace Pfps.API.Models
{
    public class UploadRequest
    {
        [Required]
        public UploadType Type { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(48)]
        public string Title { get; set; }

        [Required]
        public IFormFileCollection Uploads { get; set; }

        [MaxLength(128)]
        public string Description { get; set; }
        public string[] Tags { get; set; }
    }
}
