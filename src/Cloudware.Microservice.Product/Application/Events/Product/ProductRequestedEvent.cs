using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Events.Product
{
    public class ProductRequestedEvent
    {
        public long Id { get; set; }

        public ProductRequestedEvent(long id)
        {
            Id = id;
        }
    }
}
