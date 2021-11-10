using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public record GetCustomCategoryQuery(string TenantId):IRequestType;
   
}