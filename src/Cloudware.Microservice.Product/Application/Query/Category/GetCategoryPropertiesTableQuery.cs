using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Table;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public record GetCategoryPropertiesTableQuery(SearchDto SearchDto,string TenantId,long CategoryId):IRequestType;
  
}