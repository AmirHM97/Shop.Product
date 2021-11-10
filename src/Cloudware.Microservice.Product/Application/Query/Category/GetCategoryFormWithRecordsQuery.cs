using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public record GetCategoryFormWithRecordsQuery(string TenantId,long CategoryId,string? RecordId):IRequestType;
   
}