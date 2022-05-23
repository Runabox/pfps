using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Pfps.API.Controllers;
using Pfps.API.Data;
using Pfps.API.Services;

namespace Pfps.Filters
{
    public class ReCaptchaValidationAttribute : TypeFilterAttribute
    {
        public ReCaptchaValidationAttribute() : base(typeof(ReCaptchaValidationFilter)) { }
    }

    public class ReCaptchaValidationFilter : IAsyncActionFilter
    {
        private readonly IReCAPTCHAService _captcha;

        public ReCaptchaValidationFilter(IReCAPTCHAService captcha)
        {
            _captcha = captcha;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string incomingHeader = context.HttpContext.Request.Headers["recaptcha-response"];
            if (incomingHeader == null)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    code = "400",
                    error = "Invalid ReCAPTCHA Response",
                });
                return;
            }

            if (await _captcha.VerifyReCAPTCHAResponse(incomingHeader))
            {
                await next();
                return;
            }

            context.Result = new BadRequestObjectResult(new
            {
                code = "400",
                error = "Invalid ReCAPTCHA Response",
            });
        }
    }
}