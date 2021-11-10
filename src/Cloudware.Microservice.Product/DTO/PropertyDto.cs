using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Utilities.Contract.Basket;

namespace Cloudware.Microservice.Product.DTO
{
    public class PropertyDto
    {

        public long PropertyId { get; set; }
        public string Name { get; set; }//color guarantee
        public PropertyType PropertyType { get; set; }
        public PropertyViewType PropertyViewType { get; set; }
        public string PropertyViewTypeName { get; set; }
        public List<PropertyItemDto> PropertyItemDtos { get; set; }
    }
    public class PropertyItemDto
    {
        public long Id { get; set; }
        public string Name { get; set; }// red guarantee name
        public string? Value { get; set; }
    }
}
