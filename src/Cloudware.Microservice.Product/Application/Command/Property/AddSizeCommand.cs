using Cloudware.Microservice.Product.DTO.Property;

namespace Cloudware.Microservice.Product.Application.Command.Property
{
    public record AddSizeCommand(string TenantId,AddSizeDto AddSizeDto);
}