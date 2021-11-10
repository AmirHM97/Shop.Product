using Cloudware.Microservice.Product.DTO.Category;

namespace Cloudware.Microservice.Product.Application.Command.Category
{
    public record EditCategoryCommand(EditCategoryDto EditCategoryDto,string TenantId);
  
}