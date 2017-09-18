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

namespace AwsImageManager.Controllers
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
        public async Task<IEnumerable<string>> Get()
        {
            // TODO: Pull all items from DynamoDB, generate an access URL.

            var test = await this._s3Client.ListBucketsAsync();

            return test.Buckets.Select(b => b.BucketName);
        }
        
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            // TODO: Pull specific item from DynamoDB, generate an access URL.

            return "value";
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
                var key = Guid.NewGuid().ToString();
                await this._s3Client.UploadObjectFromStreamAsync(this._settings.S3BucketName, key, fileStream, new Dictionary<string, object>());

                // Push into DynamoDB
                await this._dbClient.PutItemAsync(new PutItemRequest()
                {
                    TableName = this._settings.DynamoDBTableName,
                    Item = new Dictionary<string, AttributeValue>()
                    {
                        { "image-key", new AttributeValue(key) },
                        { "image-type", new AttributeValue("original") },
                        { "bucket", new AttributeValue(this._settings.S3BucketName) },
                        { "filename", new AttributeValue(file.FileName) },
                        { "extension", new AttributeValue(Path.GetExtension(file.FileName)) },
                        { "size", new AttributeValue(file.Length.ToString()) },
                        { "content-type", new AttributeValue(file.ContentType) }
                    }
                });
            }
        }
        
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
