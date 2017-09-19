using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
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

        [DynamoDBIgnore]
        public List<ImageTag> tags
        {
            get
            {
                return tagsJson == null ? null : JsonConvert.DeserializeObject<List<ImageTag>>(tagsJson);
            }
            set
            {
                tagsJson = JsonConvert.SerializeObject(value);
            }
        }
        
        public string tagsJson { get; set; }
    }
}
