using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Utilities.Common.Exceptions;
using Cloudware.Utilities.Contract.Abstractions;
using DnsClient.Internal;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using src.Product;

namespace Cloudware.Microservice.Product.Application.Query.Product
{
    public class GetProductForInvoiceQueryConsumer : IConsumer<GetProductForInvoiceQuery>, IBusConsumerType
    {
        private readonly IProductReadDbContext _readDbContext;
        private     readonly ILogger<GetProductForInvoiceQueryConsumer>_logger;

        public GetProductForInvoiceQueryConsumer(IProductReadDbContext readDbContext, ILogger<GetProductForInvoiceQueryConsumer> logger)
        {
            _readDbContext = readDbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetProductForInvoiceQuery> context)
        {
            var collection = _readDbContext.ProductItemsDataCollection;

            var ids = context.Message.GetProductForInvoiceQueryItem
            .Select(s => s.ProductId).ToList();
            var products = await collection.Find(f => ids.Contains(f.ProductId)).ToListAsync();

            if (products is not null)
            {
                var responseItems = new List<GetProductForInvoiceQueryResponseItem>();
                foreach (var item in context.Message.GetProductForInvoiceQueryItem)
                {
                    var productData = products.FirstOrDefault(w => w.ProductId == item.ProductId);
                    var stockData = productData.StockItemsDto.FirstOrDefault(w => w.StockId == item.StockId);
                    responseItems.Add(new GetProductForInvoiceQueryResponseItem
                    {
                        ProductDescription = productData.Description,
                        ProductId = productData.ProductId,
                        ProductImage = productData.ImageUrl,
                        ProductName = productData.Name,
                        ProductPrice = stockData.Price,
                        ProductStockId = stockData.StockId,
                        TechnicalProperties = productData.TechnicalPropertiesReadDb.Select(s => new GetProductForInvoiceQueryTechnicalProperty
                        {
                            Key = s.Name,
                            Value = s.Value
                        }).ToList()
                    });
                }
                _logger.LogInformation("data sent");
                await context.RespondAsync(new GetProductForInvoiceQueryResponse(responseItems));
            }else{
                throw new AppException(5056,System.Net.HttpStatusCode.BadRequest,$"product not found : productIds : {context.Message.GetProductForInvoiceQueryItem.Select(s=>s.ProductId).ToList()} |||||| stockIds : {context.Message.GetProductForInvoiceQueryItem.Select(s=>s.StockId).ToList()} !!!");
            }
        }
    }
}