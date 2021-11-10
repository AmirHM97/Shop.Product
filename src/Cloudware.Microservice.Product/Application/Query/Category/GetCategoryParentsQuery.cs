using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public record GetCategoryParentsQuery(string TenantId,long CategoryId):IRequestType;
}