using Amazon.S3;
using Amazon.S3.Model;
using ContaboObjectStorageAPI.Models;

namespace ContaboObjectStorageAPI.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly ILogger<S3Service> _logger;

        public S3Service(IAmazonS3 s3Client, ILogger<S3Service> logger)
        {
            _s3Client = s3Client;
            _logger = logger;
        }

        public async Task<List<Models.BucketInfo>> GetBucketsAsync()
        {
            try
            {
                var response = await _s3Client.ListBucketsAsync();
                return response.Buckets.Select(b => new Models.BucketInfo
                {
                    Name = b.BucketName,
                    CreationDate = b.CreationDate
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting buckets");
                throw;
            }
        }

        public async Task<List<S3ObjectInfo>> GetObjectsAsync(string bucketName, string prefix = "")
        {
            try
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    Prefix = prefix,
                    MaxKeys = 1000
                };

                var response = await _s3Client.ListObjectsV2Async(request);

                return response.S3Objects.Select(obj => new S3ObjectInfo
                {
                    Key = obj.Key,
                    BucketName = bucketName,
                    Size = obj.Size,
                    LastModified = obj.LastModified,
                    ETag = obj.ETag?.Trim('"') ?? string.Empty,
                    ContentType = "application/octet-stream" // Default, as ListObjects doesn't return content-type
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting objects from bucket {BucketName}", bucketName);
                throw;
            }
        }

        public async Task<UploadResponse> UploadFileAsync(UploadRequest uploadRequest)
        {
            try
            {
                using var stream = uploadRequest.File.OpenReadStream();

                var request = new PutObjectRequest
                {
                    BucketName = uploadRequest.BucketName,
                    Key = uploadRequest.Key,
                    InputStream = stream,
                    ContentType = uploadRequest.File.ContentType,
                    ServerSideEncryptionMethod = ServerSideEncryptionMethod.None
                };

                var response = await _s3Client.PutObjectAsync(request);

                return new UploadResponse
                {
                    Success = true,
                    Message = "File uploaded successfully",
                    ObjectUrl = $"s3://{uploadRequest.BucketName}/{uploadRequest.Key}",
                    ETag = response.ETag?.Trim('"') ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file {Key} to bucket {BucketName}",
                    uploadRequest.Key, uploadRequest.BucketName);

                return new UploadResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DownloadResponse> DownloadFileAsync(string bucketName, string key)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = key
                };

                using var response = await _s3Client.GetObjectAsync(request);
                using var memoryStream = new MemoryStream();

                await response.ResponseStream.CopyToAsync(memoryStream);

                return new DownloadResponse
                {
                    Data = memoryStream.ToArray(),
                    ContentType = response.Headers.ContentType,
                    FileName = Path.GetFileName(key)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file {Key} from bucket {BucketName}", key, bucketName);
                throw;
            }
        }

        public async Task<DeleteResponse> DeleteFileAsync(string bucketName, string key)
        {
            try
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = key
                };

                await _s3Client.DeleteObjectAsync(request);

                return new DeleteResponse
                {
                    Success = true,
                    Message = "File deleted successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {Key} from bucket {BucketName}", key, bucketName);

                return new DeleteResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<bool> CreateBucketAsync(string bucketName)
        {
            try
            {
                var request = new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true
                };

                await _s3Client.PutBucketAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bucket {BucketName}", bucketName);
                return false;
            }
        }

        public async Task<bool> DeleteBucketAsync(string bucketName)
        {
            try
            {
                await _s3Client.DeleteBucketAsync(bucketName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting bucket {BucketName}", bucketName);
                return false;
            }
        }

        public async Task<string> GeneratePresignedUrlAsync(string bucketName, string key, TimeSpan expiry)
        {
            try
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = key,
                    Expires = DateTime.UtcNow.Add(expiry),
                    Verb = HttpVerb.GET
                };

                return await _s3Client.GetPreSignedURLAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating presigned URL for {Key} in bucket {BucketName}", key, bucketName);
                throw;
            }
        }
    }
}
