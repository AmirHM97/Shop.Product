using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Query.Brand
{
    public record GetBrandByIdQuery(long Id,string TenantId):IRequestType;
 
}