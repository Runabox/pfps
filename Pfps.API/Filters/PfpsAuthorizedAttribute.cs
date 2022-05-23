using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Pfps.API.Controllers;
using Pfps.API.Data;

namespace Pfps.Filters
{
    public class PfpsAuthorizedAttribute : TypeFilterAttribute
    {
        public PfpsAuthorizedAttribute(Flags flags = Flags.NONE) : base(typeof(PfpsAuthorizationFilter))
        {
            _flags = flags;
            Arguments = new object[] { flags };
        }

        private readonly Flags _flags;
    }

    public class PfpsAuthorizationFilter : IAsyncActionFilter
    {
        private Flags _flags;
        private PfpsContext _ctx;

        public PfpsAuthorizationFilter(Flags flags, PfpsContext ctx)
        {
            _ctx = ctx;
            _flags = flags;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string incomingHeader = context.HttpContext.Request.Headers.Authorization;
            if (incomingHeader == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            string[] headers = incomingHeader.Split(' ');

            if (headers.Length == 2 && headers[0] == "Bearer")
            {
                var user = await _ctx.Users
                    .Include(x => x.Favorites)
                    .FirstOrDefaultAsync(x => x.Token == headers[1]);

                if (user != null && this.ValidateHasFlag(_flags, user))
                {
                    var controller = context.Controller as PfpsControllerBase;
                    controller.PfpsUser = user;

                    await next();
                    return;
                }
            }

            context.Result = new UnauthorizedResult();
        }

        private bool ValidateHasFlag(Flags flags, User user)
        {
            if (user.Flags.HasFlag(flags))
            {
                return true;
            }

            // Always return true if user has administrator flag as administrators have access to every endpoint
            if (user.Flags.HasFlag(Flags.ADMINISTRATOR))
            {
                return true;
            }

            return false;
        }
    }
}