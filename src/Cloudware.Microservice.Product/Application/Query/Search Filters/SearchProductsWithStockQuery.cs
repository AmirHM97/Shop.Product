using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Query.Search_Filters
{
    public record SearchProductsWithStockQuery(string SearchQuery,bool? IsAvailable  ,string TenantId,string UserId, int PageSize, int PageNumber):IRequestType;
   
}