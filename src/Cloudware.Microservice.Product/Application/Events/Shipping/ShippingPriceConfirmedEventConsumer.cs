using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Product;
using Cloudware.Utilities.Contract.Product.Stock;
using Cloudware.Utilities.Contract.Shipping;

using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Events.Shipping
{
    // public class ShippingPriceConfirmedEventConsumer : IConsumer<ShippingPriceConfirmedEvent>, IBusConsumerType
    // {
    //     private readonly DbSet<StockItem> _stockItem;
    //     private readonly IUnitOfWork _uow;
    //     private readonly ILogger<OrderCreatedEventConsumer> _logger;
    //     private readonly IServiceScopeFactory _serviceScopeFactory;
    //     private readonly IRequestClient<CheckStockQuery> _checkStockQuery;



    //     public ShippingPriceConfirmedEventConsumer(IUnitOfWork uow, ILogger<OrderCreatedEventConsumer> logger, IServiceScopeFactory serviceScopeFactory, IRequestClient<CheckStockQuery> checkStockQuery)
    //     {
    //         _uow = uow;
    //         _logger = logger;
    //         _stockItem = _uow.Set<StockItem>();
    //         _serviceScopeFactory = serviceScopeFactory;
    //         _checkStockQuery = checkStockQuery;
    //     }

    //     public async Task Consume(ConsumeContext<ShippingPriceConfirmedEvent> context)
    //     {
    //         //==================================checkStockQuery todo++++++++++++--------------
    //         var stockItems = await _stockItem.Where(w => context.Message.BasketData.BasketItems
    //         .Select(s => s.StockItemDto.Id).Contains(w.Id)).ToListAsync();
    //         bool basketItemsAvailable = true;
    //         var checkStockItemList = new List<CheckStockItem>();
    //         foreach (var basketItem in context.Message.BasketData.BasketItems)
    //         {
    //             var checkStockItem = new CheckStockItem
    //             {
    //                 RequestedCount = basketItem.RequestedCount,
    //                 StockId = basketItem.StockItemDto.Id,
    //                 Price = stockItems.FirstOrDefault(w => w.Id == basketItem.StockItemDto.Id).Price
    //             };
    //             checkStockItemList.Add(checkStockItem);

    //             //var stockItem = stockItems.Where(w => w.Id == basketItem.StockItemDto.Id).Where(w => w.Count >= basketItem.RequestedCount).Where(w => w.Price == basketItem.StockItemDto.Price).FirstOrDefault();
    //             //if (stockItem == null)
    //             //{
    //             //    basketItemsAvailable = false;
    //             //    //save not available basketitem and Stockcount
    //             //}
    //         }
    //         var checkStockQueryResponse = await _checkStockQuery.GetResponse<CheckStockQueryResponse>(new CheckStockQuery
    //         {
    //             BasketId = context.Message.BasketData.BasketId,
    //             OrderId = 0,
    //             CheckStockItems = checkStockItemList
    //         });

    //         if (checkStockQueryResponse.Message.StockAvailable)
    //         {
    //             using (var scope = _serviceScopeFactory.CreateScope())
    //             {
    //                 var publisher = scope.ServiceProvider.GetService<IPublishEndpoint>();
    //                 await publisher.Publish(new StockConfirmedEvent(context.Message),x=>x.CorrelationId=context.CorrelationId);
    //             }
    //             _logger.LogDebug($"Basket {context.Message} confirmed !!");
    //         }

    //     }
    // }
}
