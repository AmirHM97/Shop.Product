using Cloudware.Utilities.Formbuilder.Dtos;

namespace Cloudware.Microservice.Product.DTO.Category
{
    public class EditCategoryFormDto
    {
        public long CategoryId { get; set; }
        public EditFormDto EditFormDto { get; set; }
    }
}