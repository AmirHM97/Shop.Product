using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Application.Events.Stock;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Common.Exceptions;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Cloudware.Microservice.Product.Application.Command.Stock
{
    public class EditStockCommandConsumer : IConsumer<EditStockCommand>, IMediatorConsumerType
    {
        private readonly DbSet<StockItem> _stockItems;
        private readonly IProductReadDbContext _productReadDbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public EditStockCommandConsumer(IUnitOfWork unitOfWork, IProductReadDbContext productReadDbContext, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _stockItems = _unitOfWork.Set<StockItem>();
            _productReadDbContext = productReadDbContext;
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<EditStockCommand> context)
        {
            var product = await _productReadDbContext.ProductItemsDataCollection.Find(f => f.TenantId == context.Message.TenantId && f.Guid.ToLower() == context.Message.ProductGuid.ToLower()).FirstOrDefaultAsync();
            if (product is null)
            {
                throw new AppException(5056, System.Net.HttpStatusCode.BadRequest, "product not found!!!");
            }
            var stockItems = await _stockItems.Where(w => w.ProductId == product.ProductId).ToListAsync();

            var newStocks = new List<StockItem>();
            foreach (var (item, newStock) in from newStock in context.Message.StocksItems
                                             let item = newStock.Id == null ? stockItems.FirstOrDefault() : stockItems.FirstOrDefault(w => w.Id == Int64.Parse(newStock.Id))
                                             select (item, newStock))
            {
                if (item == null)
                {
                    //List<StockProperty> stockPropertyItems = new();

                    //foreach (var stockPropertyId in newStock.PropItems)
                    //{
                    //    stockPropertyItems.Add(new StockProperty { PropertyId = stockPropertyId.PropertyId,PropertyType= });
                    //}
                    newStock.PropItems = newStock.PropItems.Where(w => !string.IsNullOrEmpty(w.PropertyId)).ToList();
                    var stockItem = new StockItem
                    {
                        Count = newStock.Count,
                        MaxCount = 0,
                        MinCount = 0,
                        CreatedDate = DateTimeOffset.UtcNow,
                        LastUpdatedDate = DateTimeOffset.UtcNow,
                        Price = newStock.Price,
                        Discount = newStock.Discount,
                        IsAvailable = newStock.IsAvailable,
                        ProductId = product.ProductId,
                        StockPropertyItems = newStock.PropItems.Count != 0 ? newStock.PropItems.Select(s => new StockProperty { PropertyId = Int64.Parse(s.PropertyId), PropertyType = s.PropertyType }).ToList() : null
                    };

                    newStocks.Add(stockItem);
                    continue;
                    //add new 
                }
                //update
                item.Discount = newStock.Discount;
                item.Count = newStock.Count;
                item.Price = newStock.Price;
                item.LastUpdatedDate = DateTimeOffset.UtcNow;
                item.IsDeleted = newStock.IsDeleted;
                item.IsAvailable = newStock.IsAvailable;
                stockItems.Add(item);
            }
            stockItems.AddRange(newStocks);
            _stockItems.UpdateRange(stockItems);
            await _unitOfWork.SaveChangesAsync();
            await _mediator.Publish(new StockEditedEvent(context.Message.TenantId,product.ProductId));
        }
    }
}