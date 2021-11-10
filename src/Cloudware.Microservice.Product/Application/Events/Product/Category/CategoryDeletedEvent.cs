using System.Collections.Generic;

namespace Cloudware.Microservice.Product.Application.Events.Product.Category
{
    public record CategoryDeletedEvent(List<long>CategoryIds,string TenantId);
}