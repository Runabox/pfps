using Microsoft.AspNetCore.Mvc;
using Pfps.API.Data;

namespace Pfps.API.Controllers
{
    public class PfpsControllerBase : ControllerBase
    {
        public User PfpsUser { get; set; }

        [NonAction]
        public IActionResult Error(string error, int code = 400)
        {
            Response.StatusCode = code;
            return BadRequest(new
            {
                code = 500,
                error
            });
        }
    }
}