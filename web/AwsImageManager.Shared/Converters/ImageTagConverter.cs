using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using AwsImageManager.Shared.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;

namespace AwsImageManager.Shared.Converters
{
    public class ImageTagConverter : IPropertyConverter
    {
        public object FromEntry(DynamoDBEntry entry)
        {
            var tagList = new List<ImageTag>();

            var primList = entry as PrimitiveList;
            if (primList == null) return tagList;

            tagList = primList.AsListOfString()
                .Select(s => JsonConvert.DeserializeObject<ImageTag>(s))
                .ToList();

            return tagList;
        }

        public DynamoDBEntry ToEntry(object value)
        {
            var imageTagList = value as List<ImageTag>;
            if (imageTagList == null) return null;
            
            return new PrimitiveList()
            {
                Entries = imageTagList
                    .Select(it => new Primitive(JsonConvert.SerializeObject(it)))
                    .ToList()
            };
        }
    }
}
