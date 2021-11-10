using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Utilities.Contract.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Command
{
    public record CreateProductCategoryCommand(string Name, long ParentId, string TenantId, string Icon, string Description) : IRequestType;
}
