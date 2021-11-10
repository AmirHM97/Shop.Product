using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Query.Property
{
    public record GetColorsQuery(string TenantId,string Query,int PageSize,int PageNumber):IRequestType;
   
}