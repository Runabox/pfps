using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Pfps.API.Models;

namespace Pfps.API.Services
{
    public interface IDiscordOAuth2Service
    {
        Task<OAuth2TokenResult> OAuth2TokenRequestAsync(string code);
        Task<DiscordUser> GetDiscordUserAsync(string accessToken);
    }

    public class DiscordOAuth2Service : IDiscordOAuth2Service
    {
        private static HttpClient _http = new HttpClient();
        private readonly PfpsOptions _options;
        private readonly ILogger<DiscordOAuth2Service> _log;

        public DiscordOAuth2Service(IOptions<PfpsOptions> options, ILogger<DiscordOAuth2Service> log)
        {
            _options = options.Value;
            _log = log;
        }

        public async Task<OAuth2TokenResult> OAuth2TokenRequestAsync(string code)
        {
            var body = new Dictionary<string, string>();
            body.Add("client_id", _options.DiscordClientId);
            body.Add("client_secret", _options.DiscordClientSecret);
            body.Add("grant_type", "authorization_code");
            body.Add("code", code);
            body.Add("redirect_uri", _options.DiscordRedirectUri);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://discord.com/api/v10/oauth2/token")
            {
                Content = new FormUrlEncodedContent(body),
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            var res = await _http.SendAsync(request);
            return await res.Content.ReadFromJsonAsync<OAuth2TokenResult>();
        }

        public async Task<DiscordUser> GetDiscordUserAsync(string accessToken)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, "https://discord.com/api/v10/users/@me"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var res = await _http.SendAsync(request);
                return await res.Content.ReadFromJsonAsync<DiscordUser>();
            }
        }
    }

    public class OAuth2TokenResult
    {
        public string Access_Token { get; set; }
        public int Expires_In { get; set; }
        public string Refresh_Token { get; set; }
        public string Scope { get; set; }
        public string Token_Type { get; set; }
    }

    public class DiscordUser
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public string Discriminator { get; set; }
        public int Public_Flags { get; set; }
        public int Flags { get; set; }
        public string Banner { get; set; }
        public string Locale { get; set; }
        public bool Mfa_Enabled { get; set; }
        public int Premium_Type { get; set; }
        public string Email { get; set; }
        public bool Verified { get; set; }
    }
}