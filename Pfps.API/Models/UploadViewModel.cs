using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Pfps.API.Data;

namespace Pfps.API.Models
{
    public class UploadViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsApproved { get; set; }
        public string[] Tags { get; set; }
        public List<string> Urls { get; set; }
        public PublicUserViewModel Uploader { get; set; }
        public DateTime Timestamp { get; set; }
        public int Views { get; set; }
        public UploadType Type { get; set; }

        public static UploadViewModel From(Upload upload)
        {
            return new UploadViewModel()
            {
                Id = upload.Id,
                Title = upload.Title,
                Description = upload.Description,
                IsApproved = upload.IsApproved,
                Tags = upload.Tags,
                Uploader = PublicUserViewModel.From(upload.Uploader),
                Urls = upload.Urls.ToList(),
                Timestamp = upload.Timestamp,
                Views = upload.Views,
                Type = upload.Type,
            };
        }
    }

    public class UploadSimplifiedViewModel
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

        public static UploadSimplifiedViewModel From(Upload upload)
        {
            return new UploadSimplifiedViewModel()
            {
                Id = upload.Id,
                Title = upload.Title,
                Description = upload.Description,
                Tags = upload.Tags,
                Uploader = PublicUserViewModel.From(upload.Uploader),
                Timestamp = upload.Timestamp,
                IsApproved = upload.IsApproved,
                Urls = upload.Urls,
                Views = upload.Views,
                Type = upload.Type,
            };
        }
    }
}