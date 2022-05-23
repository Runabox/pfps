using System;
using Microsoft.Extensions.Options;
using Pfps.API.Models;

namespace Pfps.API.Services
{
    public interface IReCAPTCHAService
    {
        Task<bool> VerifyReCAPTCHAResponse(string key);
    }

    public class ReCAPTCHAService : IReCAPTCHAService
    {
        private readonly PfpsOptions _options;
        private readonly ILogger<ReCAPTCHAService> _log;
        private readonly HttpClient _http;
        public ReCAPTCHAService(IOptions<PfpsOptions> options, ILogger<ReCAPTCHAService> log)
        {
            _options = options.Value;
            _log = log;
            _http = new HttpClient();

            // Configure http client
            _http.BaseAddress = new Uri("https://google.com");
        }

        public async Task<bool> VerifyReCAPTCHAResponse(string key)
        {
            if (_options.ReCaptchaSecret == null)
            {
                return true;
            }

            var res = await _http.PostAsync($"/recaptcha/api/siteverify?secret={_options.ReCaptchaSecret}&response={key}", null);
            var result = await res.Content.ReadFromJsonAsync<ReCAPTCHAVerifyResponse>();

            if (result.Success)
            {
                return true;
            }

            return false;
        }
    }

    public class ReCAPTCHAVerifyResponse
    {
        public bool Success { get; set; }
        public DateTime ChallengeTs { get; set; }
    }
}