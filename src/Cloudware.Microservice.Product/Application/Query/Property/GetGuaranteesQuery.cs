using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Query.Property
{
    public record GetGuaranteesQuery(string TenantId,string Query,int PageSize, int PageNumber) : IRequestType;

}