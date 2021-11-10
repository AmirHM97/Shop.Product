using System;

namespace Cloudware.Microservice.Product.Application.Command.Property
{
    public record EditSizeCommand(string TenantId,string Id,string Size);
   
}