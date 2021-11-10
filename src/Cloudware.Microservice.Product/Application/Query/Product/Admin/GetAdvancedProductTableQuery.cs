using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Table;

namespace Cloudware.Microservice.Product.Application.Query.Product.Admin
{
    public record GetAdvancedProductTableQuery(string TenantId,SearchDto SearchDto):IRequestType;
   
}