using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Table;

namespace Cloudware.Microservice.Product.Application.Query.Stock
{
    public record GetProductStocksTableQuery(SearchDto SearchDto,long ProductId,string TenantId):IRequestType;
}