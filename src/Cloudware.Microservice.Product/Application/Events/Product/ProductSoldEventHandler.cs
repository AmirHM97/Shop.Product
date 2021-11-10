// using Cloudware.Microservice.Product.Infrastructure;
// using MediatR;
// using Microsoft.Extensions.Options;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using MongoDB.Driver;
// using System.Threading;
// using System.Threading.Tasks;
// using Cloudware.Microservice.Product.Model.ReadDbModel;
// using Microsoft.Extensions.Logging;
// using Cloudware.Microservice.Product.Model;
// using Microsoft.EntityFrameworkCore;
// using Cloudware.Microservice.Product.Application.Events.Product;
// using static Cloudware.Microservice.Product.Application.Events.Product.StockCountChangedEvent;
// using Cloudware.Utilities.Contract.Abstractions;

// namespace Cloudware.Microservice.Product.Application.Events
// {
//     public class ProductSoldEventHandler : INotificationHandler<ProductSoldEvent>
//     {
//         private readonly IUnitOfWork _uow;
//         private readonly DbSet<StockItem> _stockItem;
//         private readonly IMediator _mediator;
//         private readonly ProductReadDbContext _readContext;
//         private readonly ILogger<ProductSoldEventHandler> _logger;
//         public ProductSoldEventHandler( ILogger<ProductSoldEventHandler> logger, IUnitOfWork uow, IMediator mediator)
//         {
//             _readContext = new ProductReadDbContext(settings);
//             _logger = logger;
//             _uow = uow;
//             _stockItem = _uow.Set<StockItem>();
//             _mediator = mediator;
//         }
//         public async Task Handle(ProductSoldEvent notification, CancellationToken cancellationToken)
//         {
//             var collection = _readContext.ProductItemsDataCollection;
//             List<StockCountChangedItem> stockCountChangedItems = new();
//             foreach (var item in notification.ProductSoldDto)
//             {
//                 StockCountChangedItem stockCountChangedItem = new();
//                 stockCountChangedItem.ProductId = item.ProductId;
//                 stockCountChangedItem.StockId = item.StockItemId;
//                 //write db
//                 var stockWriteDb = await _stockItem.Where(w => w.Id == item.StockItemId).FirstOrDefaultAsync();
//                 stockWriteDb.Count -= item.Amount;
//                 stockCountChangedItem.RemainingCount = stockWriteDb.Count;
//                 _stockItem.Update(stockWriteDb);
//                // read db
//                 var productReadDb = await collection.Find(f => f.ProductId == item.ProductId).FirstOrDefaultAsync();
//                 var filter = Builders<ProductItemReadDbCollection>.Filter.Eq(s => s.ProductId, item.ProductId);
//                 productReadDb.TotalSoldCount += item.Amount;
//                 productReadDb.StockItemsDto.FirstOrDefault(a => a.StockId == item.StockItemId).Count -= item.Amount;
//                 productReadDb.StockItemsDto.FirstOrDefault(a => a.StockId == item.StockItemId).SoldCount += item.Amount;
//                 await collection.ReplaceOneAsync(filter, productReadDb);
//                 _logger.LogInformation($"count of product with Id : {item.ProductId} decreased {item.Amount}");

//                 stockCountChangedItems.Add(stockCountChangedItem);
//                 // product.
//             }
//             await _uow.SaveChangesAsync();
//             // return Task.CompletedTask; 
//             await _mediator.Publish(new StockCountChangedEvent(stockCountChangedItems));
//            //publish  StockCount decreased
           
//         }
//     }
// }
