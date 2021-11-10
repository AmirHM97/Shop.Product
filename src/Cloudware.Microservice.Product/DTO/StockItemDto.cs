using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Utilities.Contract.Basket;

namespace Cloudware.Microservice.Product.DTO
{
    public class StockItemDto
    {
        public long StockId { get; set; }
        public long Count { get; set; }
        public long MinCount { get; set; } = 0;
        public long MaxCount { get; set; } = 0;
        public decimal Price { get; set; }
        public double Discount { get; set; }
        public long SoldCount { get; set; }
        public List<StockPropertyItemDto> PropItems { get; set; }
    }
    public class StockPropertyItemDto
    {
        public long StockPropertyId { get; set; }
        public long PropertyId { get; set; }
        public PropertyType PropertyType { get; set; }
    }
}
