using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cloudware.Microservice.Product.Model.ReadDbModel
{
    public class SellersCollection
    {
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string SellerId { get; set; } 
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string TenantId { get; set; }
    }
}
