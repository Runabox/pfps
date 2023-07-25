using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pfps.API.Data;
using Pfps.API.Models;
using Pfps.API.Services;
using Pfps.Filters;
using System.ComponentModel.DataAnnotations;
using X.PagedList;

namespace Pfps.API.Controllers
{
    public class UploadController : PfpsControllerBase
    {
        private readonly PfpsContext _ctx;
        private readonly ILogger<UploadController> _log;
        private readonly IFileService _file;
        private readonly IMapper _mapper;

        public UploadController(PfpsContext ctx, IMapper mapper, ILogger<UploadController> log, IFileService file)
        {
            _ctx = ctx;
            _log = log;
            _file = file;
            _mapper = mapper;
        }

        [HttpGet("/api/v1/uploads/orderby")]
        public async Task<IActionResult> GetUploadsOrderedAsync([FromQuery] OrderType type, [FromQuery] UploadType uploadType, [FromQuery] [Range(0, int.MaxValue)] int page = 0, [FromQuery] [Range(5, 100)] int limit = 20)
        {
            var query = _ctx.ApprovedUploads
                .Where(x => x.Type == uploadType)
                .Include(x => x.Uploader)
                .OrderByDescending(x => x.Timestamp);

            if (type == OrderType.Popular)
                query = query.OrderByDescending(x => x.Views);

            var uploads = await query
                .ProjectTo<UploadViewModel>(_mapper.ConfigurationProvider)
                .ToPagedListAsync(page, limit);

            return Ok(uploads);
        }


        [PfpsAuthorized]
        [ReCaptchaValidation]
        [HttpPost("/api/v1/upload")]
        public async Task<IActionResult> CreateUploadAsync([FromForm] UploadRequest request)
        {
            if (request.Tags?.Any(x => x.Length > 25) == true)
                ModelState.AddModelError("Tags", "Tag length cannot exceed 25 characters.");

            if (request.Type == UploadType.Multiple)
                ModelState.AddModelError("Type", "Multiple profile pictures are not yet supported.");

            if ((request.Type == UploadType.Single || request.Type == UploadType.Banner) && request.Uploads?.Count != 1)
                ModelState.AddModelError("Uploads", "Expected only one upload for this upload type.");

            if (request.Type == UploadType.Matching && request.Uploads?.Count != 2)
                ModelState.AddModelError("Uploads", "Expected two uploads for this upload type.");

            foreach (var file in request.Uploads)
                if (!IsValidFile(file))
                    ModelState.AddModelError("Uploads", $"File {file.FileName} is invalid and/or prohibited.");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            List<Tag> tags = new();
            foreach (var tag in request.Tags)
            {
                var t = await _ctx.Tags.FindAsync(tag);
                if (t == null)
                    await _ctx.Tags.AddAsync(t = new Tag(tag));

                tags.Add(t);
            }

            List<string> urls = new();
            foreach (var file in request.Uploads)
            {
                var fileId = Guid.NewGuid();
                var fileExtension = Path.GetExtension(file.FileName);
                var uploadFileResponse = await _file.UploadFileAsync(file, fileId, fileExtension);
                if (uploadFileResponse != true)
                    return Error("Internal server error - failed to upload file to S3", 500);

                urls.Add($"https://cdn.pfps.one/uploads/{fileId}.{fileExtension}");
            }

            var upld = new Upload()
            {
                Title = request.Title,
                Description = request.Description,
                Tags = tags.Select(x => x.Name).ToArray(),
                Urls = urls.ToArray(),
                Type = request.Type,
                Uploader = base.PfpsUser
            };

            await _ctx.Uploads.AddAsync(upld);
            await _ctx.SaveChangesAsync();

            _log.LogInformation("User {user} uploaded new post {upload}", base.PfpsUser.Id, upld.Id);
            return Ok(_mapper.Map<UploadViewModel>(upld));
        }

        [HttpGet("/api/v1/uploads/{id}")]
        public async Task<IActionResult> GetUploadAsync(Guid id)
        {
            var upload = await _ctx.Uploads
                .Include(x => x.Uploader)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (upload == null)
                return NotFound();

            upload.Views++;
            _ = _ctx.SaveChangesAsync().ConfigureAwait(false); // this may throw an ObjectDisposedException

            return Ok(_mapper.Map<UploadViewModel>(upload));
        }

        [PfpsAuthorized]
        [HttpPost("/api/v1/uploads/{id}/favorite")]
        public async Task<IActionResult> FavoriteUploadAsync(Guid id)
        {
            var upload = await _ctx.Uploads.FirstOrDefaultAsync(x => x.Id == id);
            if (upload == null)
                return NotFound();

            if (base.PfpsUser.Favorites.Any(x => x.Upload == upload))
                return Error("Post already in favorites.");
            
            var favorite = new Favorite()
            {
                UserId = base.PfpsUser.Id,
                Upload = upload,
            };

            base.PfpsUser.Favorites.Add(favorite);
            await _ctx.SaveChangesAsync();
            return NoContent();
        }

        [NonAction]
        public bool IsValidFile(IFormFile file)
        {
            // this is cringe
            return file.ContentType switch
            {
                "image/jpeg" or "image/jpg" or "image/png" or "image/webp" or "image/gif" => true,
                _ => false,
            };
        }
    }
}