using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwsImageManager.Shared.Data
{
    [DynamoDBDocument]
    public class ImageTag
    {
        public string Name { get; set; }
        public float Confidence { get; set; }
    }
}
