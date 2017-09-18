using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwsImageManager
{
    public class ImageManagerSettings
    {
        public string S3BucketName { get; set; }
        public string DynamoDBTableName { get; set; }
    }
}
