using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pfps.API.Data;
using Pfps.API.Models;
using Pfps.API.Services;
using Pfps.Filters;

namespace Pfps.API.Controllers
{
    public class AdministrationController : PfpsControllerBase
    {
        private readonly PfpsContext _ctx;
        private readonly ILogger<AdministrationController> _log;
        private readonly IAuditLogger _audit;

        public AdministrationController(PfpsContext ctx, ILogger<AdministrationController> log, IAuditLogger audit)
        {
            _ctx = ctx;
            _log = log;
            _audit = audit;
        }

        [HttpGet("/api/v1/admin/uploads/unapproved")]
        [PfpsAuthorized(Flags.CONTENT_MODERATOR)]
        public async Task<IActionResult> GetUnapprovedUploadsAsync([FromQuery] int page = 0, [FromQuery] int limit = 10)
        {
            var uploads = await _ctx.Uploads
                .Where(x => x.IsApproved == false)
                .Include(x => x.Uploader)
                .OrderByDescending(x => x.Timestamp)
                .Skip(page * limit)
                .Take(limit)
                .Select(x => UploadSimplifiedViewModel.From(x))
                .ToListAsync();

            return Ok(uploads);
        }

        [HttpPost("/api/v1/uploads/{id}/disapprove")]
        [PfpsAuthorized(Flags.ADMINISTRATOR)]
        public async Task<IActionResult> DisapproveUploadAsync(Guid id, [FromQuery] string? reason)
        {
            var upload = await _ctx.Uploads
                .Include(x => x.Uploader)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (upload == null)
                return NotFound();

            upload.IsApproved = false;

            // Create notification
            var notification = new Notification()
            {
                Type = NotificationType.DISAPPROVAL,
                User = upload.Uploader,
                Moderator = base.PfpsUser.Username,
                ModeratorId = base.PfpsUser.Id,
                UploadTitle = upload.Title,
                UploadId = upload.Id,
                Message = (reason == null ? "No reason provided." : reason)
            };

            await _ctx.Notifications.AddAsync(notification); await _ctx.SaveChangesAsync();

            await _audit.LogEventAsync($"Disapproved Upload ID {id}", AuditEvent.UPLOAD_DISAPPROVAL, base.PfpsUser);
            return Ok(UploadSimplifiedViewModel.From(upload));
        }

        [HttpPost("/api/v1/uploads/{id}/approve")]
        [PfpsAuthorized(Flags.CONTENT_MODERATOR)]
        public async Task<IActionResult> ApproveUploadAsync(Guid id)
        {
            var upload = await _ctx.Uploads
                .Include(x => x.Uploader)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (upload == null)
                return NotFound();

            upload.IsApproved = true;

            // Create notification
            var notification = new Notification()
            {
                Type = NotificationType.APPROVAL,
                User = upload.Uploader,
                Moderator = base.PfpsUser.Username,
                ModeratorId = base.PfpsUser.Id,
                UploadTitle = upload.Title,
                UploadId = upload.Id,
            };

            await _ctx.Notifications.AddAsync(notification);
            await _ctx.SaveChangesAsync();

            await _audit.LogEventAsync($"Approved Upload ID {id}", AuditEvent.UPLOAD_APPROVAL, base.PfpsUser);
            return Ok(UploadSimplifiedViewModel.From(upload));
        }

        [HttpDelete("/api/v1/uploads/{id}")]
        [PfpsAuthorized(Flags.CONTENT_MODERATOR)]
        public async Task<IActionResult> DeleteUploadAsync(Guid id, [FromQuery] string? reason)
        {
            var upload = await _ctx.Uploads
                .Include(x => x.Uploader)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (upload == null)
                return NotFound();

            if (upload.Uploader.Favorites.Any(x => x.Upload == upload))
            {
                _ctx.Favorites.Remove(upload.Uploader.Favorites.FirstOrDefault(x => x.Upload == upload));
                _ctx.Uploads.Remove(upload);
            }
            else
            {
                _ctx.Uploads.Remove(upload);
            }

            // Create notification
            var notification = new Notification()
            {
                Type = NotificationType.DELETION,
                User = upload.Uploader,
                Moderator = base.PfpsUser.Username,
                ModeratorId = base.PfpsUser.Id,
                UploadTitle = upload.Title,
                UploadId = upload.Id,
                Message = (reason == null ? "No reason provided." : reason)
            };

            await _ctx.Notifications.AddAsync(notification); await _ctx.SaveChangesAsync();
            await _ctx.SaveChangesAsync();

            await _audit.LogEventAsync($"Deleted Upload ID {id}", AuditEvent.UPLOAD_DELETION, base.PfpsUser);
            return NoContent();
        }
    }
}