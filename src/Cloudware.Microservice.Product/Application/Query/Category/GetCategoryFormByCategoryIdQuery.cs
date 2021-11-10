using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public record GetCategoryFormByCategoryIdQuery(long CategoryId,string TenantId):IRequestType;
   
}