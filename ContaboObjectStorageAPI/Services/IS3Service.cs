using ContaboObjectStorageAPI.Models;

namespace ContaboObjectStorageAPI.Services
{
    public interface IS3Service
    {
        Task<List<BucketInfo>> GetBucketsAsync();
        Task<List<S3ObjectInfo>> GetObjectsAsync(string bucketName, string prefix = "");
        Task<UploadResponse> UploadFileAsync(UploadRequest request);
        Task<DownloadResponse> DownloadFileAsync(string bucketName, string key);
        Task<DeleteResponse> DeleteFileAsync(string bucketName, string key);
        Task<bool> CreateBucketAsync(string bucketName);
        Task<bool> DeleteBucketAsync(string bucketName);
        Task<string> GeneratePresignedUrlAsync(string bucketName, string key, TimeSpan expiry);
    }
}
