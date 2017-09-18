using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Amazon.S3;
using Amazon.S3.Model;

namespace AwsImageManager.Controllers
{
    [Produces("application/json")]
    [Route("api/images")]
    public class ImagesController : Controller
    {
        private readonly IAmazonS3 _s3Client;

        public ImagesController(IAmazonS3 s3Client)
        {
            this._s3Client = s3Client;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            var test = await this._s3Client.ListBucketsAsync();

            return test.Buckets.Select(b => b.BucketName);
        }
        
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
        [HttpPost]
        public void Post([FromBody]string value)
        {
            //this._s3Client.UploadObjectFromStreamAsync()
        }
        
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
