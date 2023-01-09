using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Pfps.API.Services;

namespace Pfps.Filters
{
    public class ReCaptchaValidationAttribute : TypeFilterAttribute
    {
        public ReCaptchaValidationAttribute() : base(typeof(ReCaptchaValidationFilter)) { }
    }

    public class ReCaptchaValidationFilter : IAsyncActionFilter
    {
        private readonly IReCaptchaService _captcha;
        public ReCaptchaValidationFilter(IReCaptchaService captcha) => _captcha = captcha;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string incomingHeader = context.HttpContext.Request.Headers["X-Recaptcha-Response"];
            if (incomingHeader != null && await _captcha.VerifyReCAPTCHAResponse(incomingHeader))
            {
                await next();
                return;
            }

            context.Result = new BadRequestObjectResult(new
            {
                code = 400,
                error = "Invalid ReCAPTCHA Response",
            });
        }
    }
}