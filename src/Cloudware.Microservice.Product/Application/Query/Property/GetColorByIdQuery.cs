using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Query.Property
{
    public record GetColorByIdQuery(string TenantId,string Guid):IRequestType;
 
}