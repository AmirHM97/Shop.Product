using Cloudware.Microservice.Product.DTO.Property;
using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Command.Property
{
    public record AddGuaranteeCommand(string TenantId,AddGuaranteeDto AddGuaranteeDto):IRequestType;
  
}