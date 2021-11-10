using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using static Cloudware.Microservice.Product.Application.Events.Order.OrderCreatedEvent;
using static Contracts.OrderCreatedEvent;

namespace Contracts
{
    public class StockNotConfimedEvent
    {
        public StockNotConfimedEvent(long orderId, List<OrderCreatedItem> orderItems)
        {
            OrderId = orderId;
            OrderItems = orderItems;
        }

        public long OrderId { get; set; }
        public List<OrderCreatedItem> OrderItems { get; set; }

       
    }
}
