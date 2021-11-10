using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Events.Product
{


    public record ProductEditFailedEvent(Guid ProductId);


}
