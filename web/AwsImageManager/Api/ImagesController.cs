using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using Microsoft.Extensions.Options;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;
using AwsImageManager.Models;
using AwsImageManager.Shared.Data;

namespace AwsImageManager.API
{
    [Produces("application/json")]
    [Route("api/images")]
    public class ImagesController : Controller
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IAmazonDynamoDB _dbClient;
        private readonly ImageManagerSettings _settings;

        public ImagesController(IAmazonS3 s3Client, IAmazonDynamoDB dbClient, IOptions<ImageManagerSettings> settings)
        {
            this._s3Client = s3Client;
            this._dbClient = dbClient;
            this._settings = settings.Value;
        }

        [HttpGet]
        public async Task<IEnumerable<ImageDto>> Get()
        {
            List<ImageItem> results = new List<ImageItem>();
            using (var context = new DynamoDBContext(this._dbClient))
            {
                results = await context.ScanAsync<ImageItem>(new List<ScanCondition>()
                {
                    new ScanCondition("imageType", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, "original")
                }).GetRemainingAsync();
            }

            var images = results.Select(ToImageDto);

            return images;
        }
        
        [HttpGet("{imageKey}")]
        public async Task<List<ImageDto>> Get(string imageKey)
        {
            List<ImageDto> results = new List<ImageDto>();

            using (var context = new DynamoDBContext(this._dbClient))
            {
                var images = await context.QueryAsync<ImageItem>(imageKey).GetRemainingAsync();
                results.AddRange(images.Select(ToImageDto));
            }

            return results;
        }
        
        /// <summary>
        /// Upload a file to S3.
        /// </summary>
        /// <param name="file"></param>
        [HttpPost]
        public async Task Post(IFormFile file)
        {
            using (var fileStream = new MemoryStream())
            {
                // Temporarily store off the stream - awaiting the ensure bucket disposes of the IFormFile stream, so
                // we need to hold onto it for now.
                file.CopyTo(fileStream);
 
                // Ensure that the S3 bucket exists.  The "EnsureBucketExistsAsync" should do this, but bombs if the bucket
                // does indeed exist, so we are working around it.
                var bucketExists = await this._s3Client.DoesS3BucketExistAsync(this._settings.S3BucketName);
                if (!bucketExists)
                {
                    await this._s3Client.EnsureBucketExistsAsync(this._settings.S3BucketName).ConfigureAwait(true);
                }

                // Push the image up to S3
                var extension = Path.GetExtension(file.FileName);
                var key = Guid.NewGuid().ToString() + extension;
                await this._s3Client.UploadObjectFromStreamAsync(this._settings.S3BucketName, key, fileStream, new Dictionary<string, object>());

                // Save into DynamoDB
                using (var context = new DynamoDBContext(this._dbClient))
                {
                    await context.SaveAsync(new ImageItem()
                    {
                        imageKey = key,
                        imageType = "original",
                        imagePath = key, // mods will go under "imageType/[key]"
                        bucket = this._settings.S3BucketName,
                        filename = file.FileName,
                        extension = extension,
                        size = file.Length.ToString(),
                        contentType = file.ContentType
                    });
                }
            }
        }
        
        [HttpDelete("{imageKey}")]
        public async Task Delete(string imageKey)
        {
            using (var context = new DynamoDBContext(this._dbClient))
            {
                var images = await context.QueryAsync<ImageItem>(imageKey).GetRemainingAsync();
                images.ForEach(async i => await context.DeleteAsync<ImageItem>(i));
            }
        }

        private ImageDto ToImageDto(ImageItem i)
        {
            return new ImageDto()
            {
                Filename = i.filename,
                Type = i.imageType,
                Url = this._s3Client.GetPreSignedURL(new GetPreSignedUrlRequest()
                {
                    BucketName = i.bucket,
                    Key = i.imageKey,
                    Expires = DateTime.Now.AddHours(1)
                }),
                Tags = i.tags
            };
        }
    }
}
