using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Model.ReadDbModel
{
    public class ProductCategoryReadDbCollection
    {

        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string ClientId { get; set; }
        public List<ProductCategoryReadDb> ProductCategories { get; set; } = new();
    }
    public class ProductCategoryReadDb
    {
        public long CategoryId { get; set; }
        public string? Guid { get; set; }
        public string FormId { get; set; }
        public string RecordId { get; set; }
        public long? ParentId { get; set; }
        public string Name { get; set; }
        public string? Icon { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public List<ProductCategoryReadDb> Children { get; set; } = new List<ProductCategoryReadDb>();
    }
}
