using System;
using System.Collections;
using System.Collections.Generic;
using Cloudware.Utilities.Contract.Basket;

namespace Cloudware.Microservice.Product.Model
{
    public class PropertyCategory
    {
        public long Id { get; set; }
        public string TenantId { get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LastUpdatedDate { get; set; } = DateTimeOffset.UtcNow;
        public PropertyType PropertyType { get; set; }
        public virtual ProductCategory Category { get; set; }
        public  long CategoryId { get; set; }
    }
    // public enum PropertyType
    // {
    //     Color, Size, Guarantee
    // }
}