using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pfps.API.Data;
using Pfps.API.Models;

namespace Pfps.API.Controllers
{
    public class TagController : PfpsControllerBase
    {
        private readonly PfpsContext _ctx;
        private readonly ILogger<TagController> _log;

        public TagController(PfpsContext ctx, ILogger<TagController> log)
        {
            _ctx = ctx;
            _log = log;
        }

        [HttpGet("/api/v1/tags/{id}")]
        public async Task<IActionResult> GetTagAsync(Guid id)
        {
            var tag = await _ctx.Tags.FirstOrDefaultAsync(x => x.Id == id);

            if (tag == null)
                return NotFound();

            return Ok(TagViewModel.From(tag));
        }

        // TODO: add pages, limit
        [HttpGet("/api/v1/tags/{id}/uses")]
        public async Task<IActionResult> GetTagUsesAsync(Guid id, [FromQuery] bool count)
        {
            var tag = await _ctx.Tags.FirstOrDefaultAsync(x => x.Id == id);

            if (tag == null)
                return NotFound();

            var uploads = await _ctx.Uploads
                .AsNoTracking()
                .Where(x => x.IsApproved == true)
                .Where(x => x.TagIds.Contains(tag.Id))
                .Select(x => UploadSimplifiedViewModel.From(x))
                .ToListAsync();

            if (count == true)
            {
                return Ok(new
                {
                    uses = uploads.ToArray().Length,
                });
            }

            return Ok(new
            {
                uses = uploads.ToArray().Length,
                uploads,
            });
        }
    }
}