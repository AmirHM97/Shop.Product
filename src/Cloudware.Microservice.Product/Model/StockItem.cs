using Cloudware.Utilities.Contract.Basket;
using System;
using System.Collections.Generic;

namespace Cloudware.Microservice.Product.Model
{
    public class StockItem
    {
        public long Id { get; set; }
        public long Count { get; set; }
        public long MaxCount { get; set; } = 0;
        public long MinCount { get; set; } = 0;
        public decimal Price { get; set; }
        public double Discount { get; set; }
        public long ProductId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAvailable { get; set; }
        public virtual ProductItem ProductItem { get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LastUpdatedDate { get; set; } = DateTimeOffset.UtcNow;
        public virtual ICollection<StockProperty> StockPropertyItems { get; set; }

    }
    public class StockProperty
    {
        public long Id { get; set; }
        public long StockId { get; set; }
        public virtual StockItem Stock { get; set; }
        public long PropertyId { get; set; }
        public PropertyType PropertyType { get; set; }
    }
}
