using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cloudware.Microservice.Product.Model.ReadDbModel
{
    public class CustomCategoryCollection
    {
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string ClientId { get; set; }
        public List<CustomCategoryCollectionItem> ProductCategories { get; set; } = new();
    }
    public class CustomCategoryCollectionItem
    {
        public string Name { get; set; }
        public CustomCategoryData Data { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public List<CustomCategoryCollectionItem> Children { get; set; } = new List<CustomCategoryCollectionItem>();
    }
    public class CustomCategoryData
    {
        public long CategoryId { get; set; }
        public string? Guid { get; set; }
        public long? ParentId { get; set; }
        public string? Icon { get; set; }
        public string? Description { get; set; }
        public string FormId { get; set; }
        public string RecordId { get; set; }

    }
}