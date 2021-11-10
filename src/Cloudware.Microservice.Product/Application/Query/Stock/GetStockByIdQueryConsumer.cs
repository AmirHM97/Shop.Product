using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Infrastructure.Extensions;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Basket;
using MassTransit;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query.Stock
{
    public class GetStockByIdQueryConsumer : IConsumer<GetStockByIdQuery>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _productReadDbContext;

        public GetStockByIdQueryConsumer(IProductReadDbContext productReadDbContext)
        {
            _productReadDbContext = productReadDbContext;
        }

        public async Task Consume(ConsumeContext<GetStockByIdQuery> context)
        {
            var product = await _productReadDbContext.ProductItemsDataCollection.Find(f => f.ProductId == context.Message.ProductId).FirstOrDefaultAsync();
            var stock = product.StockItemsDto.FirstOrDefault(f => f.StockId == context.Message.Id);
            var props = stock.PropItems.Select(s => new GetStockByIdQueryResponseItem
            {
                PropertyId=s.PropertyId,
                PropertyType=s.PropertyType,
                PropertyTitle= ProductExtensions.GetPropertyName(s.PropertyType),
                Name=product.PropertiesDto.Where(w=>w.PropertyType==s.PropertyType).SelectMany(sp=>sp.PropertyItemDtos).Where(w=>w.Id==s.PropertyId).Select(s=>s.Name).FirstOrDefault(),
                Value= product.PropertiesDto.Where(w => w.PropertyType == s.PropertyType).SelectMany(sp => sp.PropertyItemDtos).Where(w => w.Id == s.PropertyId).Select(s => s?.Value).FirstOrDefault()??"",
            }).ToList();
            var res = new GetStockByIdQueryResponse(stock.StockId, stock.Price, stock.Count, props, stock.IsAvailable);
        }
    }
    public class GetStockByIdQueryResponse
    {
        public GetStockByIdQueryResponse(long stockId, decimal price, decimal count, List<GetStockByIdQueryResponseItem> properties, bool isAvailable)
        {
            StockId = stockId;
            Price = price;
            Count = count;
            Properties = properties;
            IsAvailable = isAvailable;
        }

        public long StockId { get; set; }
        public decimal Price { get; set; }
        public decimal Count { get; set; }
        public bool IsAvailable { get; set; }
        public List<GetStockByIdQueryResponseItem> Properties { get; set; }
    }
    public class GetStockByIdQueryResponseItem
    {
        public long PropertyId { get; set; }
        public PropertyType PropertyType { get; set; }
        public string PropertyTitle { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
