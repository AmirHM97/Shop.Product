using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Table;

namespace Cloudware.Microservice.Product.Application.Query.Property
{
    public record GetColorsTableQuery(string TenantId,SearchDto SearchDto):IRequestType;
  
}