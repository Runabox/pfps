namespace Pfps.API.Models
{
    public class PfpsOptions
    {
        public string DiscordClientId { get; set; }
        public string DiscordClientSecret { get; set; }
        public string DiscordRedirectUri { get; set; }
        public string S3Bucket { get; set; }
        public string ReCaptchaSecret { get; set; }
    }
}