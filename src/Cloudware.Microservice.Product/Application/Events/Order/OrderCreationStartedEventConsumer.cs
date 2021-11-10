using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.BusTools;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Ordering;
using Cloudware.Utilities.Contract.Product;
using Cloudware.Utilities.Contract.Product.Stock;
using Contracts;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.EntityFrameworkCore;
using src.Ordering;

namespace Cloudware.Microservice.Product.Application.Events.Order
{
    public class OrderCreationStartedEventConsumer : IConsumer<OrderCreationStartedEvent>, IBusConsumerType
    {
        private readonly IMediator _mediator;
        private readonly IProductReadDbContext _productReadDbContext;
        private readonly DbSet<StockItem> _stockItem;
        private readonly IUnitOfWork _uow;
        private readonly IPublishEndpointInSingletonService _publishEndpointInSingletonService;

        public OrderCreationStartedEventConsumer(IMediator mediator, IUnitOfWork uow, IPublishEndpointInSingletonService publishEndpointInSingletonService)
        {
            _mediator = mediator;
            _uow = uow;
            _stockItem = _uow.Set<StockItem>();
            _publishEndpointInSingletonService = publishEndpointInSingletonService;
        }

        public async Task Consume(ConsumeContext<OrderCreationStartedEvent> context)
        {
            var checkStockItemList = new List<CheckStockItem>();
            var stockItems = await _stockItem.Where(w => context.Message.BasketItems
          .Select(s => s.StockItemDto.Id).Contains(w.Id)).ToListAsync();
            foreach (var basketItem in context.Message.BasketItems)
            {
                var checkStockItem = new CheckStockItem
                {
                    RequestedCount = basketItem.RequestedCount,
                    StockId = basketItem.StockItemDto.Id,
                    Price = stockItems.FirstOrDefault(w => w.Id == basketItem.StockItemDto.Id).Price
                };
                checkStockItemList.Add(checkStockItem);
            }

            var checkStockReq = _mediator.CreateRequestClient<CheckStockQuery>();
            var checkStockRes = await checkStockReq.GetResponse<CheckStockQueryResponse>(new CheckStockQuery
            {
                CheckStockItems = checkStockItemList,
                OrderId=context.Message.OrderId,
            });
            if (checkStockRes.Message.StockAvailable)
            {
                await _publishEndpointInSingletonService.Publish(new StockConfirmedEvent(context.Message.OrderId,context.Message.TenantId));
            }
        }
    }
}