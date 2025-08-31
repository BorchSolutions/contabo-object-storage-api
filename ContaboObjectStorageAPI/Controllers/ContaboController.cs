using ContaboObjectStorageAPI.Models;
using ContaboObjectStorageAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContaboObjectStorageAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaboController : ControllerBase
    {
        private readonly IS3Service _s3Service;

        public ContaboController(IS3Service s3Service)
        {
            _s3Service = s3Service;
        }

        [HttpGet("buckets")]
        public async Task<ActionResult<List<BucketInfo>>> GetBuckets()
        {
            try
            {
                var buckets = await _s3Service.GetBucketsAsync();
                return Ok(buckets);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("buckets/{bucketName}")]
        public async Task<ActionResult> CreateBucket(string bucketName)
        {
            try
            {
                var result = await _s3Service.CreateBucketAsync(bucketName);
                if (result)
                    return Ok(new { message = "Bucket created successfully" });
                else
                    return BadRequest(new { error = "Failed to create bucket" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("buckets/{bucketName}")]
        public async Task<ActionResult> DeleteBucket(string bucketName)
        {
            try
            {
                var result = await _s3Service.DeleteBucketAsync(bucketName);
                if (result)
                    return Ok(new { message = "Bucket deleted successfully" });
                else
                    return BadRequest(new { error = "Failed to delete bucket" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("objects/{bucketName}")]
        public async Task<ActionResult<List<S3ObjectInfo>>> GetObjects(string bucketName, [FromQuery] string prefix = "")
        {
            try
            {
                var objects = await _s3Service.GetObjectsAsync(bucketName, prefix);
                return Ok(objects);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("upload")]
        public async Task<ActionResult<UploadResponse>> UploadFile([FromForm] UploadRequest request)
        {
            try
            {
                if (request.File == null || request.File.Length == 0)
                    return BadRequest(new { error = "No file provided" });

                var result = await _s3Service.UploadFileAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("download/{bucketName}/{*key}")]
        public async Task<ActionResult> DownloadFile(string bucketName, string key)
        {
            try
            {
                var result = await _s3Service.DownloadFileAsync(bucketName, key);
                return File(result.Data, result.ContentType, result.FileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("delete/{bucketName}/{*key}")]
        public async Task<ActionResult<DeleteResponse>> DeleteFile(string bucketName, string key)
        {
            try
            {
                var result = await _s3Service.DeleteFileAsync(bucketName, key);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("presigned-url/{bucketName}/{*key}")]
        public async Task<ActionResult> GeneratePresignedUrl(string bucketName, string key, [FromQuery] int expireMinutes = 60)
        {
            try
            {
                var expiry = TimeSpan.FromMinutes(expireMinutes);
                var url = await _s3Service.GeneratePresignedUrlAsync(bucketName, key, expiry);
                return Ok(new { url, expiresIn = expiry.TotalMinutes });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
