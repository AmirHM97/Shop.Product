using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts
{
    public class OrderCreatedEvent 
    {

        public long OrderId { get; set; }
        public List<OrderCreatedItem> OrderItems { get; set; }

        public class OrderCreatedItem
        {
            public long OrderItemId { get; set; }
            public long ProductId { get; set; }
            public long StockId { get; set; }
            public long Amount { get; set; }
        }
    }
}
