using Cloudware.Utilities.Contract.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Command
{
    public record CreateBrandCommand(string Name,string TenantId,string Icon):IRequestType;
  
}
