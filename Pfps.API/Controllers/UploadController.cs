using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pfps.API.Data;
using Pfps.API.Models;
using Pfps.API.Services;
using Pfps.Filters;

namespace Pfps.API.Controllers
{
    public class UploadController : PfpsControllerBase
    {
        private readonly PfpsContext _ctx;
        private readonly ILogger<UploadController> _log;
        private readonly IFileService _file;

        public UploadController(PfpsContext ctx, ILogger<UploadController> log, IFileService file)
        {
            _ctx = ctx;
            _log = log;
            _file = file;
        }

        [HttpGet("/api/v1/uploads/orderby")]
        public async Task<IActionResult> GetUploadsOrderedAsync([FromQuery] OrderType type, [FromQuery] UploadType uploadType, [FromQuery] int page = 0, [FromQuery] int limit = 20)
        {
            var query = _ctx.ApprovedUploads
                .Where(x => x.Type == uploadType)
                .Include(x => x.Uploader)
                .OrderByDescending(x => x.Timestamp);

            if (type == OrderType.POPULAR)
                query = query.OrderByDescending(x => x.Views);

            var uploads = await query
                .Skip(page * limit)
                .Take(limit)
                .Select(x => UploadSimplifiedViewModel.From(x))
                .ToListAsync();

            return Ok(uploads);
        }


        [HttpPost("/api/v1/upload")]
        [ReCaptchaValidation]
        [PfpsAuthorized]
        public async Task<IActionResult> CreateUploadAsync([FromForm] int type, [FromForm] string tags, [FromForm] string title,
            [FromForm] string description, [FromForm] IFormFileCollection upload)
        {
            if (title.Length > 48)
                return BadRequest(new
                {
                    code = 400,
                    error = "Title cannot be over 48 characters."
                });

            if (description != null && description.Length > 128)
                return BadRequest(new
                {
                    code = 400,
                    error = "Description cannot be over 128 characters."
                });

            if (upload.ToArray().Length <= 0)
                return BadRequest(new
                {
                    code = 400,
                    error = "No files provided.",
                });

            if (type >= 3)
                return BadRequest(new
                {
                    code = 400,
                    error = "Invalid Upload Type"
                });

            if (upload.ToArray().Length == 2 && ((UploadType)type == UploadType.PFP_SINGLE || (UploadType)type == UploadType.PFP_MULTIPLE))
                return BadRequest(new
                {
                    code = 400,
                    error = $"Incorrect upload type (provided {ParseUploadTypeInt((UploadType)type)}, expected PFP_MATCHING)",
                });
            else if (upload.ToArray().Length == 1 && ((UploadType)type == UploadType.PFP_MULTIPLE || (UploadType)type == UploadType.PFP_MATCHING))
                return BadRequest(new
                {
                    code = 400,
                    error = $"Incorrect upload type (provided {ParseUploadTypeInt((UploadType)type)}, expected PFP_SINGLE)",
                });
            else if (upload.ToArray().Length > 2 && ((UploadType)type == UploadType.PFP_SINGLE || (UploadType)type == UploadType.PFP_MATCHING))
                return BadRequest(new
                {
                    code = 400,
                    error = $"Incorrect upload type (provided {ParseUploadTypeInt((UploadType)type)}, expected PFP_MULTIPLE)",
                });
            else if (upload.ToArray().Length > 8)
                return BadRequest(new
                {
                    code = 400,
                    error = $"You can only upload 8 profile pictures per post! (provided {upload.ToArray().Length})",
                });

            if ((UploadType)type == UploadType.PFP_MULTIPLE)
                return BadRequest(new
                {
                    code = 400,
                    error = "PFP_MULTIPLE is not supported yet.",
                });

            if (title == null)
                return BadRequest(new
                {
                    code = 400,
                    error = "Malformed request body",
                });

            // create tags
            string[] tagStringArray = tags.Split(',');
            if (tagStringArray.Length > 8)
                return BadRequest(new
                {
                    code = 400,
                    error = "Maximum limit for tags exceeded",
                });

            List<Tag> tagObjects = new List<Tag>();
            foreach (string tagTitle in tagStringArray)
            {
                if (tagTitle.Length > 25)
                {
                    return BadRequest(new
                    {
                        code = 400,
                        error = "Tag length cannot be over 25",
                    });
                }

                if (await _ctx.Tags.AnyAsync(x => x.Title == tagTitle))
                {
                    tagObjects.Add(await _ctx.Tags.FirstOrDefaultAsync(x => x.Title == tagTitle));
                    continue;
                }

                var tag = new Tag()
                {
                    Title = tagTitle,
                };

                await _ctx.Tags.AddAsync(tag);
                tagObjects.Add(tag);
            }

            ICollection<IFormFile> badFiles = new Collection<IFormFile>();
            foreach (var file in upload)
                if (!ValidateDisposition(ParseFileExtension(file)))
                    badFiles.Add(file);

            if (badFiles.ToArray().Length >= 1)
            {
                ICollection<FileDispositionErrorObject> objs = new Collection<FileDispositionErrorObject>();
                foreach (var file in badFiles)
                {
                    var disposition = ParseFileExtension(file);

                    objs.Add(new FileDispositionErrorObject()
                    {
                        Name = file.FileName,
                        Disposition = disposition,
                    });
                }

                return BadRequest(new
                {
                    code = 400,
                    error = new
                    {
                        message = "One or more files does not have a valid disposition.",
                        files = objs,
                    },
                });
            }

            List<string> urlObjects = new List<string>();
            foreach (var file in upload)
            {
                var fileId = Guid.NewGuid();
                var fileExtension = ParseFileExtension(file);
                // upload to S3
                var uploadFileResponse = await _file.UploadFileAsync(file, fileId, fileExtension);
                if (uploadFileResponse != true)
                {
                    return BadRequest(new
                    {
                        code = 400,
                        error = "Internal server error - failed to upload file to S3",
                    });
                }

                var url = $"https://cdn.pfps.lol/uploads/{fileId}.{fileExtension}";
                urlObjects.Add(url);
            }

            var upld = new Upload()
            {
                Title = title,
                Description = description,
                TagIds = tagObjects.Select(x => x.Id).ToArray(),
                Tags = tagObjects.Select(x => x.Title).ToArray(),
                Urls = urlObjects.ToArray(),
                Type = (UploadType)type,
                Uploader = base.PfpsUser,
            };

            await _ctx.Uploads.AddAsync(upld);
            await _ctx.SaveChangesAsync();

            var uploadViewModel = UploadViewModel.From(upld);
            _log.LogInformation("User {user} uploaded new post {upload}", UserViewModel.From(base.PfpsUser), uploadViewModel);

            return Ok(uploadViewModel);
        }

        [HttpGet("/api/v1/uploads/{idNonParsed}")]
        public async Task<IActionResult> GetUploadAsync(string idNonParsed)
        {
            if (!Guid.TryParse(idNonParsed, out Guid id))
            {
                return BadRequest(new
                {
                    code = 400,
                    error = "Invalid Upload Id",
                });
            }

            var upload = await _ctx.Uploads
                .Include(x => x.Uploader)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (upload == null)
                return NotFound(new
                {
                    code = 404,
                    error = "The requested upload was not found.",
                });

            upload.Views++;
            await _ctx.SaveChangesAsync();

            return Ok(UploadViewModel.From(upload));
        }

        [HttpPost("/api/v1/uploads/{idNonParsed}/favorite")]
        [PfpsAuthorized]
        public async Task<IActionResult> FavoriteUploadAsync(string idNonParsed)
        {
            if (!Guid.TryParse(idNonParsed, out Guid id))
            {
                return BadRequest(new
                {
                    code = 400,
                    error = "Invalid Upload Id",
                });
            }

            var upload = await _ctx.Uploads.FirstOrDefaultAsync(x => x.Id == id);
            if (upload == null)
                return NotFound(new
                {
                    code = 404,
                    error = "The requested upload was not found.",
                });

            if (!base.PfpsUser.Favorites.Any(x => x.Upload == upload))
            {
                var favorite = new Favorite()
                {
                    UserId = base.PfpsUser.Id,
                    Upload = upload,
                };

                await _ctx.AddAsync(favorite);
                base.PfpsUser.Favorites.Add(favorite);

                await _ctx.SaveChangesAsync();
            }
            else
            {
                return BadRequest(new
                {
                    code = 400,
                    error = "Post already in favorites.",
                });
            }

            return NoContent();
        }

        [NonAction]
        public string ParseUploadTypeInt(UploadType type)
        {
            switch (type)
            {
                case UploadType.PFP_SINGLE:
                    return "PFP_SINGLE";
                case UploadType.PFP_MATCHING:
                    return "PFP_MATCHING";
                case UploadType.PFP_MULTIPLE:
                    return "PFP_MULTIPLE";
                case UploadType.BANNER:
                    return "BANNER";
                default:
                    return $"UNKNOWN_UPLOAD_TYPE ({(int)type})";
            }
        }

        [NonAction]
        public string ParseOrderTypeInt(OrderType type)
        {
            switch (type)
            {
                case OrderType.DESCENDING:
                    return "DESCENDING";
                case OrderType.POPULAR:
                    return "POPULAR";
                default:
                    return $"UNKNOWN_ORDER_TYPE ({(int)type})";
            }
        }

        [NonAction]
        public string ParseFileExtension(IFormFile file)
        {
            return file.FileName.Split(".").LastOrDefault();
        }

        [NonAction]
        public bool ValidateDisposition(string disposition)
        {
            switch (disposition)
            {
                case "jpg":
                    return true;
                case "png":
                    return true;
                case "jpeg":
                    return true;
                case "webp":
                    return true;
                case "gif":
                    return true;
                default:
                    return false;
            }
        }
    }

    public class FileDispositionErrorObject
    {
        public string Name { get; set; }
        public string Disposition { get; set; }
    }
}