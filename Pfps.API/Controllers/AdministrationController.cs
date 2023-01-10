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
    public class AdministrationController : PfpsControllerBase
    {
        private readonly PfpsContext _ctx;
        private readonly IAuditLogger _audit;
        private readonly IMapper _mapper;

        public AdministrationController(PfpsContext ctx, IMapper mapper, IAuditLogger audit)
        {
            _ctx = ctx;
            _audit = audit;
            _mapper = mapper;
        }

        [HttpGet("/api/v1/admin/uploads/unapproved")]
        [PfpsAuthorized(UserFlags.ContentModerator)]
        public async Task<IActionResult> GetUnapprovedUploadsAsync([FromQuery] [Range(0, int.MaxValue)] int page = 0, [FromQuery] [Range(5, 100)] int limit = 10)
        {
            var uploads = await _ctx.Uploads
                .Where(x => x.IsApproved == false)
                .Include(x => x.Uploader)
                .OrderByDescending(x => x.Timestamp)
                .ProjectTo<UploadViewModel>(_mapper.ConfigurationProvider)
                .ToPagedListAsync(page, limit);

            return Ok(uploads);
        }

        [HttpPost("/api/v1/uploads/{id}/approve")]
        [PfpsAuthorized(UserFlags.ContentModerator)]
        public async Task<IActionResult> ApproveUploadAsync(Guid id)
        {
            var upload = await _ctx.Uploads
                .Include(x => x.Uploader)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (upload == null)
                return NotFound();

            // Create notification
            var notification = new Notification()
            {
                Type = NotificationType.Approval,
                User = upload.Uploader,
                Moderator = base.PfpsUser.Username,
                ModeratorId = base.PfpsUser.Id,
                UploadTitle = upload.Title,
                UploadId = upload.Id,
            };

            upload.IsApproved = true;
            await _ctx.Notifications.AddAsync(notification);
            await _audit.LogEventAsync($"Approved Upload ID {id}", AuditEvent.UploadApproval, base.PfpsUser);
            await _ctx.SaveChangesAsync();

            return NoContent(); // no reason to send the upload data back here.
        }

        [HttpDelete("/api/v1/uploads/{id}")]
        [PfpsAuthorized(UserFlags.ContentModerator)]
        public async Task<IActionResult> DeleteUploadAsync(Guid id, [FromQuery] string reason = "No reason provided.")
        {
            var upload = await _ctx.Uploads
                .Include(x => x.Uploader)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (upload == null)
                return NotFound();

            // Create notification
            var notification = new Notification()
            {
                Type = NotificationType.Deletion,
                User = upload.Uploader,
                Moderator = base.PfpsUser.Username,
                ModeratorId = base.PfpsUser.Id,
                UploadTitle = upload.Title,
                UploadId = upload.Id,
                Message = reason
            };

            _ctx.Uploads.Remove(upload);
            await _ctx.Notifications.AddAsync(notification);
            await _audit.LogEventAsync($"Deleted Upload ID {id}", AuditEvent.UploadDeletion, base.PfpsUser);
            await _ctx.SaveChangesAsync();

            return NoContent();
        }
    }
}