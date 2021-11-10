using Cloudware.Microservice.Product.DTO.Property;

namespace Cloudware.Microservice.Product.Application.Command.Category
{
    public record AddPropertyCategoryCommand(AddPropertyCategoryDto AddCategoryPropertyDto,string TenantId);

}