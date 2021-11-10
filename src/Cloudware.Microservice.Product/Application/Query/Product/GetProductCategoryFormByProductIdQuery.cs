using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Query.Product
{
    public record GetProductCategoryFormByProductIdQuery(string TenantId,long ProductId):IRequestType;
    
}