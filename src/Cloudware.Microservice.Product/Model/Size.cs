using System;

namespace Cloudware.Microservice.Product.Model
{
    public class Size
    {
        public long Id { get; set; }
        public Guid Guid { get; set; }
        public string TenantId { get; set; }

        public string Name { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public bool IsDeleted { get; set; }

    }
}