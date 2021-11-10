using Cloudware.Utilities.Contract.Basket;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Model.ReadDbModel
{
    public class StockItemReadDb
    {
        public long StockId { get; set; }
        public long Count { get; set; }
        public long MinCount { get; set; } = 0;
        public long MaxCount { get; set; } = 0;
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Price { get; set; }
        [BsonRepresentation(BsonType.Double)]
        public double Discount { get; set; }
        public long SoldCount { get; set; }
        public bool IsAvailable { get; set; }=true;
        public bool IsDeleted { get; set; }=false;

        public List<StockPropertyItem> PropItems { get; set; }
    }
    public class StockPropertyItem
    {
        public long StockPropertyId { get; set; }
        public long PropertyId { get; set; }
        public PropertyType PropertyType { get; set; }
    }
}
