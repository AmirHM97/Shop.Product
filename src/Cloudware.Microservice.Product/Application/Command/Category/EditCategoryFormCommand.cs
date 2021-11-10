using System.IO;
using Cloudware.Microservice.Product.DTO.Category;
using Cloudware.Utilities.Formbuilder.Dtos;

namespace Cloudware.Microservice.Product.Application.Command.Category
{
    public record EditCategoryFormCommand(string TenantId,EditCategoryFormDto EditCategoryFormDto);
    
}