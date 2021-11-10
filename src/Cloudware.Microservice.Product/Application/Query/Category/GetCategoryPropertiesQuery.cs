using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public record GetCategoryPropertiesQuery(long CategoryId, string TenantId) : IRequestType;

}