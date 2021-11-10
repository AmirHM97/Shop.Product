// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;

// namespace Cloudware.Microservice.Product.Application.Events.Product
// {
//     public class StockCountChangedEvent:INotification
//     {
//         public StockCountChangedEvent(List<StockCountChangedItem> stockCountDecreasedItems)
//         {
//             StockCountDecreasedItems = stockCountDecreasedItems;
//         }
//         public List<StockCountChangedItem> StockCountDecreasedItems { get; set; }
//         public class StockCountChangedItem
//         {
//             public long ProductId { get; set; }
//             public long StockId { get; set; }
//             public long RemainingCount { get; set; }
//         }
//     }
// }
