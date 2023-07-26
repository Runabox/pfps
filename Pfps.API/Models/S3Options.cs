namespace Pfps.API.Models
{
    public class S3Options
    {
        public string ServiceURL { get; set; }
        public string Bucket { get; set; }
        public string Region { get; set; }
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
    }
}
