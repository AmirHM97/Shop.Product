using Cloudware.Microservice.Product.DTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Model.ReadDbModel
{
    public class ProductItemReadDbCollection
    {
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string? Guid { get; set; }
        public string TenantId { get; set; }
        public long ProductId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public long? ProvinceId { get; set; }
        public string? Province { get; set; }
        public long? CityId { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Mobile { get; set; }
        public string UserImageUrl { get; set; }
        public string UserDescription { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
        // public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        // public DateTimeOffset LastUpdatedDate { get; set; } = DateTimeOffset.UtcNow;
        public List<string> ImageUrls { get; set; }
        public long ViewCount { get; set; }
        public long TotalSoldCount { get; set; }
        public bool IsAvailable { get; set; }
        public bool? IsDeleted { get; set; }=false;
        public double? Weight { get; set; }
        public string Description { get; set; }
        public string Story { get; set; }
        public string Dimensions { get; set; }
        public long BrandId { get; set; }
        public string BrandName { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal MaxPrice { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal MinPrice { get; set; }
        public long TotalCount { get; set; } = 0;
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string TechnicalPropertyRecordId { get; set; }
        public string TechnicalPropertyFormId { get; set; }
        public long SupplierId { get; set; }
        public List<TechnicalPropertyReadDb> TechnicalPropertiesReadDb { get; set; }
        public List<PropertyReadDb> PropertiesDto { get; set; }
        public List<StockItemReadDb> StockItemsDto { get; set; }
    }
}
