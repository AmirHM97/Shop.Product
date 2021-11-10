using System.Collections.Generic;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Basket;

namespace Cloudware.Microservice.Product.DTO.Property
{
    public class AddPropertyCategoryDto
    {
        public List<PropertyType> PropertyType { get; set; }
        public long CategoryId { get; set; }
    }
}