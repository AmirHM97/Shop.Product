using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Product.Stock;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query.Stock
{
    public class CheckStockQueryConsumer : IConsumer<CheckStockQuery>, IBusConsumerType, IMediatorConsumerType
    {
        private readonly DbSet<StockItem> _stockItem;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<CheckStockQueryConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CheckStockQueryConsumer(IUnitOfWork uow, ILogger<CheckStockQueryConsumer> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _uow = uow;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _stockItem = _uow.Set<StockItem>();
        }

        public async Task Consume(ConsumeContext<CheckStockQuery> context)
        {

            //from ordering or basket
            if (context.Message.OrderId != 0 || context.Message.BasketId != "")
            {
                var stockItems = await _stockItem.Where(w => context.Message.CheckStockItems.Select(s => s.StockId).Contains(w.Id)).ToListAsync();
                var CheckStockQueryResponseItemList = new List<CheckStockQueryResponseItem>();
                bool stockAvailable = true;
                foreach (var item in context.Message.CheckStockItems)
                {
                    var stockItemQuery = stockItems.Where(w => w.Id == item.StockId).Where(w => w.Count >= item.RequestedCount);
                    if (context.Message.BasketId != "")
                    {
                        stockItemQuery = stockItemQuery.Where(w => w.Price == item.Price);
                    }
                    var stockItem = stockItemQuery.FirstOrDefault();
                    var stockData = stockItems.FirstOrDefault(w => w.Id == item.StockId);
                    if (stockItem == null)
                    {
                        var checkStockQueryResponseItem = new CheckStockQueryResponseItem
                        {
                            StockItemAvailable = false,
                            AvailableCount = stockData.Count,
                            CheckStockItem = new CheckStockItem
                            {
                                StockId = item.StockId,
                                Price = stockData.Price,
                                RequestedCount = item.RequestedCount,
                            }
                        };
                        stockAvailable = false;
                        CheckStockQueryResponseItemList.Add(checkStockQueryResponseItem);
                    }
                    else
                    {
                        var checkStockQueryResponseItem = new CheckStockQueryResponseItem
                        {
                            StockItemAvailable = true,
                            AvailableCount = stockData.Count,
                            CheckStockItem = new CheckStockItem
                            {
                                StockId = item.StockId,
                                Price = stockData.Price,
                                RequestedCount = item.RequestedCount,
                            }
                        };
                        CheckStockQueryResponseItemList.Add(checkStockQueryResponseItem);
                    }
                }
                var checkStockQueryResponse = new CheckStockQueryResponse
                {
                    StockAvailable = stockAvailable,
                    BasketId = context.Message.BasketId,
                    OrderId = context.Message.OrderId,
                    CheckStockQueryResponseItems = CheckStockQueryResponseItemList
                };
                await context.RespondAsync(checkStockQueryResponse);
            }
            //basket
            #region comment
            //else if (context.Message.BasketId != "")
            //{
            //    var stockItems = await _stockItem.Where(w => context.Message.CheckStockItems.Select(s => s.StockId).Contains(w.Id)).ToListAsync();
            //    bool basketItemsAvailable = true;
            //    var CheckStockQueryResponseItemList = new List<CheckStockQueryResponseItem>();

            //    foreach (var basketItem in context.Message.CheckStockItems)
            //    {
            //        var stockItem = stockItems.Where(w => w.Id == basketItem.StockId).Where(w => w.Count >= basketItem.RequestedCount).Where(w => w.Price == basketItem.Price).FirstOrDefault();
            //        var stockData = stockItems.FirstOrDefault(w => w.Id == basketItem.StockId);
            //        if (stockItem == null)
            //        {
            //            basketItemsAvailable = false;

            //            //save not available basketitem and Stockcount
            //        }
            //    }
            //    if (basketItemsAvailable)
            //    {
            //        using (var scope = _serviceScopeFactory.CreateScope())
            //        {
            //            var publisher = scope.ServiceProvider.GetService<IPublishEndpoint>();
            //            await publisher.Publish(new StockConfirmedEvent(context.Message));
            //        }
            //        _logger.LogDebug($"Basket {context.Message} confirmed !!");
            //    }
            //} 
            #endregion
            else
            {

            }
            // throw new NotImplementedException();
        }
    }
}
