using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Pfps.API.Controllers;
using Pfps.API.Data;

namespace Pfps.Filters
{
    public class PfpsAuthorizedAttribute : TypeFilterAttribute
    {
        public PfpsAuthorizedAttribute(UserFlags flags = UserFlags.None) : base(typeof(PfpsAuthorizationFilter)) =>
            Arguments = new object[] { flags };
    }

    public class PfpsAuthorizationFilter : IAsyncActionFilter
    {
        private readonly UserFlags _flags;
        private readonly PfpsContext _ctx;

        public PfpsAuthorizationFilter(UserFlags flags, PfpsContext ctx)
        {
            _ctx = ctx;
            _flags = flags;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string incomingHeader = context.HttpContext.Request.Headers.Authorization;
            string[] headers = incomingHeader?.Split(' ');
            
            if (headers != null && headers.Length == 2 && headers[0] == "Bearer")
            {
                var user = await _ctx.Users
                    .Include(x => x.Favorites)
                    .FirstOrDefaultAsync(x => x.Token == headers[1]);

                if (user != null && ValidateHasFlag(_flags, user))
                {
                    var controller = context.Controller as PfpsControllerBase;
                    controller.PfpsUser = user;

                    await next();
                    return;
                }
            }

            context.Result = new UnauthorizedResult();
        }

        private static bool ValidateHasFlag(UserFlags flags, User user) =>
            user.Flags.HasFlag(flags) || user.Flags.HasFlag(UserFlags.Administrator);
    }
}