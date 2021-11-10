using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public record GetCategoryByIdQuery(long Id,string TenantId):IRequestType;
  
}