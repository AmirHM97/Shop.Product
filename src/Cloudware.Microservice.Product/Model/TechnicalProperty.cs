using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Model
{
    public class TechnicalProperty
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public long ProductItemId { get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LastUpdatedDate { get; set; } = DateTimeOffset.UtcNow;
        public virtual ProductItem ProductItem { get; set; }

    }
}
