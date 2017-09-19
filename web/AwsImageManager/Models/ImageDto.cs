using AwsImageManager.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwsImageManager.Models
{
    public class ImageDto
    {
        public string Filename { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public List<ImageTag> Tags { get; set; }
    }
}
