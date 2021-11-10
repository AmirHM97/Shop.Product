using System;

namespace Cloudware.Microservice.Product.Application.Command.Property
{
    public record EditColorCommand(string TenantId,string Id,string Name,string Code);
    
}