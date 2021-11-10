using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Model
{
    public class ProductItem
    {
        public string TenantId { get; set; }
        public long Id { get; set; }
        public Guid Guid { get; set; }=Guid.NewGuid();
        public string UserId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LastUpdatedDate { get; set; } = DateTimeOffset.UtcNow;
        public virtual ICollection<ImageUrlItems> ImageUrls { get; set; }
        public string Image { get; set; }
        public double Weight { get; set; }
        public string Description { get; set; }
        public string Story { get; set; }
        public long BrandId { get; set; }
        public virtual ProductBrand Brand { get; set; }
        public long CategoryId { get; set; }
        public virtual ProductCategory Category { get; set; }
        public string Dimensions { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string TechnicalPropertyRecordId { get; set; }
        public string TechnicalPropertyFormId { get; set; }
        public long SupplierId { get; set; }

        //public long ProvinceId { get; set; }
        //public string Province { get; set; }
        //public long CityId { get; set; }
        //public string City { get; set; }
        //public string Address { get; set; }
        //public string Mobile { get; set; }
        public virtual ICollection<TechnicalProperty> TechnicalProperties { get; set; }
        // public virtual ICollection<Property> PropertyItems { get; set; }
        public virtual ICollection<StockItem> StocksItems { get; set; }
    }//TODO:
}
