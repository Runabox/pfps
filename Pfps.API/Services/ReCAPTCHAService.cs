using Microsoft.Extensions.Options;
using Pfps.API.Models;

namespace Pfps.API.Services
{
    public interface IReCaptchaService
    {
        Task<bool> VerifyReCAPTCHAResponse(string key);
    }

    public class ReCaptchaService : IReCaptchaService
    {
        private readonly HttpClient _client;
        private readonly PfpsOptions _options;

        public ReCaptchaService(IOptions<PfpsOptions> options, IHttpClientFactory factory)
        {
            _options = options.Value;
            _client = factory.CreateClient("Google");
        }

        public async Task<bool> VerifyReCAPTCHAResponse(string key)
        {
            if (_options.ReCaptchaSecret == null)
                return true;

            var res = await _client.PostAsync($"/recaptcha/api/siteverify?secret={_options.ReCaptchaSecret}&response={key}", null);
            var result = await res.Content.ReadFromJsonAsync<ReCaptchaResponse>();

            return result.Success;
        }
    }

    public record ReCaptchaResponse(bool Success);
}