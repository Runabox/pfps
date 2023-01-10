using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Pfps.API.Models;

namespace Pfps.API.Services
{
    public interface IDiscordService
    {
        Task<DiscordAccessToken> GetAccessToken(string code);
        Task<DiscordUser> GetDiscordUserAsync(string token);
    }

    public class DiscordService : IDiscordService
    {
        private readonly PfpsOptions _options;
        private readonly HttpClient _client;

        public DiscordService(IOptions<PfpsOptions> options, IHttpClientFactory factory)
        {
            _options = options.Value;
            _client = factory.CreateClient("Discord");
        }

        public async Task<DiscordAccessToken> GetAccessToken(string code)
        {
            var body = new Dictionary<string, string>
            {
                { "code", code },
                { "grant_type", "authorization_code" },
                { "client_id", _options.DiscordClientId },
                { "redirect_uri", _options.DiscordRedirectUri },
                { "client_secret", _options.DiscordClientSecret }
            };

            var res = await _client.PostAsync("oauth2/token", new FormUrlEncodedContent(body));

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<DiscordAccessToken>();
        }

        public async Task<DiscordUser> GetDiscordUserAsync(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await _client.GetFromJsonAsync<DiscordUser>("users/@me");
        }
    }

    public class DiscordAccessToken
    {
        [JsonPropertyName("access_token")]
        public string Token { get; set; }
    }

    public class DiscordUser
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}