using Cloudware.Utilities.Formbuilder.Dtos;

namespace Cloudware.Microservice.Product.DTO
{
    public class AddCategoryFormDto
    {
        public long CategoryId { get; set; }
        public CreateFormDto CreateFormDto { get; set; }
    }
}