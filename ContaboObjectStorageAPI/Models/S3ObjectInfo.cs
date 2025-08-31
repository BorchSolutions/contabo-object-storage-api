namespace ContaboObjectStorageAPI.Models
{
    public class S3ObjectInfo
    {
        public string Key { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string ETag { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }

    public class UploadRequest
    {
        public string BucketName { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public IFormFile File { get; set; } = null!;
    }

    public class UploadResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ObjectUrl { get; set; } = string.Empty;
        public string ETag { get; set; } = string.Empty;
    }

    public class BucketInfo
    {
        public string Name { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
    }

    public class DownloadResponse
    {
        public byte[] Data { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }

    public class DeleteResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}