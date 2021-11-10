using System;
using System.Collections;
using System.Collections.Generic;

namespace Cloudware.Microservice.Product.Model
{
    public class Color
    {
        public long Id { get; set; }
        public Guid Guid { get; set; }
        public string TenantId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public bool IsDeleted { get; set; }

        // public virtual ICollection<ColorCategory> ColorCategories { get; set; }
    }


    // public class ColorCategory
    // {
    //     public long Id { get; set; }
    //     public DateTimeOffset CreatedDate { get; set; }
    //     public DateTimeOffset LastUpdatedDate { get; set; }
    //     public long ColorId { get; set; }
    //     public virtual Color Color { get; set; }
    //     public long CategoryId { get; set; }
    //     public virtual ProductCategory Category { get; set; }

    // }
}