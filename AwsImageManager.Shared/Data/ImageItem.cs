using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using AwsImageManager.Shared.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwsImageManager.Shared.Data
{
    [DynamoDBTable("centare-aws-image-manager-demo")]
    public class ImageItem
    {
        [DynamoDBHashKey]
        public string imageKey { get; set; }
        [DynamoDBRangeKey]
        public string imageType { get; set; }
        public string imagePath { get; set; }
        public string bucket { get; set; }
        public string filename { get; set; }
        public string extension { get; set; }

        public string size { get; set; }

        public string contentType { get; set; }

        [DynamoDBProperty(Converter = typeof(ImageTagConverter))]
        public List<ImageTag> tags { get; set; }
    }
}
