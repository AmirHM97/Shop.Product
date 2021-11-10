using System;
using System.Collections.Generic;

namespace Cloudware.Microservice.Product.Application.Command.Product
{
    public record DeleteProductListCommand(List<Guid>ProductIds,string TenantId);
   
}