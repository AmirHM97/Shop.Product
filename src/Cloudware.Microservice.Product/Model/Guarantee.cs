using System;

namespace Cloudware.Microservice.Product.Model
{
    public class Guarantee
    {
        public long Id { get; set; }
        public Guid Guid { get; set; }
        public string TenantId { get; set; }

        public string Name { get; set; }
        public string BackImage { get; set; }
        public string FrontImage { get; set; }
        public int Duration { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}