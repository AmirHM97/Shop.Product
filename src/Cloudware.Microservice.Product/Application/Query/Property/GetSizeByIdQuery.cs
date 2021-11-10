using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Query.Property
{
    public record GetSizeByIdQuery(string TenantId,string Guid):IRequestType;
 
}