using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Model
{
    public class ProductBrand
    {
        public string TenantId { get; set; }
        public long Id { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public DateTimeOffset CreatedDate { get; set; }= DateTimeOffset.UtcNow;
        public DateTimeOffset LastUpdatedDate { get; set; }= DateTimeOffset.UtcNow;

        public bool IsDeleted { get; set; }

        public virtual ICollection<ProductItem>   ProductItems{ get; set; }
    }
}
