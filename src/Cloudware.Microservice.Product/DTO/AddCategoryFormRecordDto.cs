using Cloudware.Utilities.Formbuilder.Dtos;

namespace Cloudware.Microservice.Product.DTO
{
    public class AddCategoryFormRecordDto
    {
        public long CategoryId { get; set; }
        public AddRecordDto AddRecordDto { get; set; }
    }
}