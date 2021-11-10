using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Events.Product
{
    public class ProductRequestedEventConsumer : IConsumer<ProductRequestedEvent>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _readContext;
        public ProductRequestedEventConsumer( IProductReadDbContext readContext)
        {
            _readContext = readContext;
        }
        public async Task Consume(ConsumeContext<ProductRequestedEvent> context)
        {
            var collection = _readContext.ProductItemsDataCollection;
            var productItem = await collection.Find(f => f.ProductId == context.Message.Id).FirstOrDefaultAsync();
            var filter = Builders<ProductItemReadDbCollection>.Filter.Eq(e => e.ProductId , context.Message.Id);
            productItem.ViewCount += 1;
            await collection.ReplaceOneAsync(filter,productItem);
        }
    }
}
