using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwsImageManager.API.Models
{
    public class ImageTagDto
    {
        public string Name { get; set; }
        public string Confidence { get; set; }
    }
}
