using System;
using System.Collections.Generic;

namespace Cloudware.Microservice.Product.Model
{
   public class ProductCategory
    {
        public string TenantId { get; set; }
        public long Id { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public long? ParentId { get; set; }
        public string FormId { get; set; }
        public string RecordId { get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LastUpdatedDate { get; set; } = DateTimeOffset.UtcNow;
        public virtual ICollection<ProductItem> ProductItems { get; set; }
        public virtual ICollection<ProductCategory> Children { get; set; }
        // public virtual ICollection<Property> Properties { get; set; }
        public virtual ICollection<PropertyCategory> Properties { get; set; }
        public virtual ProductCategory Parent { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
}