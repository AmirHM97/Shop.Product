using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Query.Product.Admin
{
    public record GetProductByIdForAdminQuery(string Guid,string TenantId):IRequestType;
   
}