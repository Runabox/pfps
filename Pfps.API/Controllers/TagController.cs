using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pfps.API.Data;

namespace Pfps.API.Controllers
{
    public class TagController : PfpsControllerBase
    {
        private readonly PfpsContext _ctx;

        public TagController(PfpsContext ctx) => _ctx = ctx;

        [HttpGet("/api/v1/tags/{id}")]
        public async Task<IActionResult> GetTagAsync(Guid id)
        {
            var tag = await _ctx.Tags.FindAsync(id);

            if (tag == null)
                return NotFound();

            return Ok(tag);
        }

        // TODO: add pages, limit
        [HttpGet("/api/v1/tags/{id}/uses")]
        public async Task<IActionResult> GetTagUsesAsync(string id)
        {
            if (!await _ctx.Tags.AnyAsync(x => x.Name == id))
                return NotFound();

            var count = await _ctx.Uploads
                .AsNoTracking()
                .Where(x => x.IsApproved == true)
                .Where(x => x.Tags.Contains(id))
                .CountAsync();

            return Ok(new { count });
        }
    }
}