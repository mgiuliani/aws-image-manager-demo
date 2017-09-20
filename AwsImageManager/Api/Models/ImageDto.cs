using AwsImageManager.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwsImageManager.API.Models
{
    public class ImageDto
    {
        public string Key { get; set; }
        public string Filename { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public List<ImageTagDto> Tags { get; set; }
    }
}
