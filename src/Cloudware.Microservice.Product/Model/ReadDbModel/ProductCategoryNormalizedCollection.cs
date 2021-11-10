using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cloudware.Microservice.Product.Model.ReadDbModel
{
    public class ProductCategoryNormalizedCollection
    {
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string? Guid { get; set; }

        public string TenantId { get; set; }
        public long CategoryId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public long? ParentId { get; set; }
        public string FormId { get; set; }
        public string RecordId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }=DateTimeOffset.UtcNow;
        public DateTimeOffset LastUpdatedDate { get; set; }=DateTimeOffset.UtcNow;
    }
}