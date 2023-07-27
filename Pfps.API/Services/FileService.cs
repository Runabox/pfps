using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Pfps.API.Models;

namespace Pfps.API.Services
{
    public interface IFileService
    {
        Task<bool> UploadFileAsync(IFormFile file, Guid id, string extension);
    }

    public class FileService : IFileService
    {
        private readonly IAmazonS3 _s3;
        private readonly ILogger<FileService> _log;
        private readonly S3Options _options;

        public FileService(IAmazonS3 s3, ILogger<FileService> log, IOptions<S3Options> options)
        {
            _s3 = s3;
            _log = log;
            _options = options.Value;
        }

        public async Task<bool> UploadFileAsync(IFormFile file, Guid id, string extension)
        {
            try
            {
                var request = new PutObjectRequest()
                {
                    BucketName = _options.Bucket,
                    Key = $"uploads/{id}.{extension}",
                    InputStream = file.OpenReadStream(),
                    CannedACL = S3CannedACL.PublicRead
                };

                request.Metadata.Add("Content-Type", file.ContentType);
                await _s3.PutObjectAsync(request);

                _log.LogInformation("Uploaded S3 Object with ID {id} and extension {extension}", id, extension);
            }
            catch (Exception exception)
            {
                _log.LogError("S3 Error - {exception}", exception);
                return false;
            }

            return true;
        }
    }
}
