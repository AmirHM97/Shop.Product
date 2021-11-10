using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Events
{
    public record ProductCategoryCreatedEvent(string ClientId);
}
