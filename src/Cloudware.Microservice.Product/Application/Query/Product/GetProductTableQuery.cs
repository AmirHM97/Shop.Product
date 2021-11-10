using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Table;

namespace Cloudware.Microservice.Product.Application.Query.Product
{
    public record GetProductTableQuery(string TenantId,bool IsAdvance,SearchDto SearchDto):IRequestType;
  
}