using Cloudware.Utilities.Contract.Basket;

namespace Cloudware.Microservice.Product.DTO.Property
{
    public class CategoryPropertyDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public PropertyType PropertyType { get; set; }
        public long  CategoryId { get; set; }
    }
}