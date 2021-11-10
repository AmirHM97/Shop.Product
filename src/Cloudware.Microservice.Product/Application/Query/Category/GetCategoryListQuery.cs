using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public record GetCategoryListQuery(string Query,int PageSize,int PageNumber,string TenantId):IRequestType;
  
}