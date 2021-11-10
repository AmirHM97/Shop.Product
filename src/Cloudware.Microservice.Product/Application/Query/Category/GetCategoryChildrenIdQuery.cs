using System.Collections.Generic;
using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public record GetCategoryChildrenIdQuery(List<long>CategoryIds,string TenantId):IRequestType;
  
}