using System;

namespace Cloudware.Microservice.Product.Application.Command.Property
{
    public record EditGuaranteeCommand(string TenantId,string Id,string Name,string FrontImage,string BackImage,int Duration);
   
}