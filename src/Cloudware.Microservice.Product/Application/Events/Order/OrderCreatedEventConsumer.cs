using Cloudware.Microservice.Product.Infrastructure;
using MassTransit;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Model;
using Microsoft.EntityFrameworkCore;
//using Cloudware.Microservice.Product.Application.Events.Order;
using Microsoft.Extensions.Logging;
//using static Cloudware.Microservice.Product.Application.Events.Order.OrderCreatedEvent;
using Contracts;
using static Contracts.OrderCreatedEvent;
using MassTransit.Mediator;
using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Events
{
    /// <summary>
    /// check stock if order items are available or not
    /// </summary>
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>,IMediatorConsumerType
    {
        private readonly IProductReadDbContext _readContext;
        private readonly DbSet<StockItem> _stockItem;
        private readonly IUnitOfWork _uow;
        private readonly IMediator _mediator;
        private readonly ILogger<OrderCreatedEventConsumer> _logger;

        public OrderCreatedEventConsumer(IUnitOfWork uow, IMediator mediator, ILogger<OrderCreatedEventConsumer> logger, IProductReadDbContext readContext)
        {
            _uow = uow;
            _stockItem = _uow.Set<StockItem>();
            _mediator = mediator;
            _logger = logger;
            _readContext = readContext;
        }
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
           // List<OrderCreatedItem> AvilableItems = new();
           // var collection = _readContext.ProductItemsDataCollection;
           // var stockItems = await _stockItem.AsNoTracking().Where(w => context.Message.OrderItems.Select(s => s.StockId).Contains(w.Id)).ToListAsync();
           //// var stockitemss = await _stockItem.AsNoTracking().Where(w => context.Message.OrderItems.Select(s => s.StockId).Contains(w.Id)).ToListAsync();
           // List<OrderCreatedItem> NotAvailableItems = new ();
           // if (stockItems==null)
           // {

           // }
           // bool stockAvailable = true;
           // foreach (var item in context.Message.OrderItems)
           // {
           //     var stockItem = stockItems.Where(w => w.Id == item.StockId).Where(w => w.Count >= item.Amount).FirstOrDefault();
           //     if (stockItem != null)
           //         AvilableItems.Add(new OrderCreatedItem
           //         {
           //             OrderItemId = item.OrderItemId,
           //             ProductId = item.ProductId,
           //             StockId = item.StockId,
           //         });
           //     else
           //     {
           //         NotAvailableItems.Add(new OrderCreatedItem
           //         {
           //             OrderItemId = item.OrderItemId,
           //             ProductId = item.ProductId,
           //             StockId = item.StockId
           //         });
           //         stockAvailable = false;
           //        // NotAvailableItems.Add()
           //     }
           // }
           // if (stockAvailable)
           // {
           //    //  await _mediator.Publish(new StockConfimedEvent(context.Message.OrderId, AvilableItems));
           //     _logger.LogInformation("Order with Id: {context.Message.OrderId}'s Stock Is Available ");
           // }
           // else
           // {
           //     await _mediator.Publish(new StockNotConfimedEvent(context.Message.OrderId, NotAvailableItems));
           //     _logger.LogInformation("Order with Id: {context.Message.OrderId}'s Stock Is Not Available ");

            }
            //var response = new OrderCreatedResponse
            //{
            //    OrderId = context.Message.OrderId,
            //    OrderItems = responseItems
            //};
            // await context.RespondAsync<List<OrderCreatedResponse>>(response);
            //publish event
        }
    }

   
    /// <summary>
    /// response for order Service to show which orderItem is Available
    /// </summary>
    //public class OrderCreatedResponse
    //{
    //    public int OrderId { get; set; }
    //    public List<OrderCreatedItemResponse> OrderItems { get; set; }
    //}
    //public class OrderCreatedItemResponse
    //{
    //    public int OrderItemId { get; set; }
    //    public int ProductId { get; set; }
    //    public int StockId { get; set; }
    //    public bool IsAvailable { get; set; }
    //}
//}
