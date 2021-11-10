using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public record GetCategoryChildrenQuery(string TenantId,long CategoryId,string Query,int PageSize,int PageNumber):IRequestType;
    
}