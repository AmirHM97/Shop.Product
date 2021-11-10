using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.DTO.Category
{
    public class CreateCategoryDto
    {
        public string Name { get; set; }
        public long ParentId { get; set; }
        public string? TenantId { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
    }
}
