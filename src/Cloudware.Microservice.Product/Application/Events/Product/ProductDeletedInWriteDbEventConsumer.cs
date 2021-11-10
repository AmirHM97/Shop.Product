using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using MongoDB.Driver;

namespace Cloudware.Microservice.Product.Application.Events.Product
{
    public class ProductDeletedInWriteDbEventConsumer : IConsumer<ProductDeletedInWriteDbEvent>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _productReadDbContext;

        public ProductDeletedInWriteDbEventConsumer(IProductReadDbContext productReadDbContext)
        {
            _productReadDbContext = productReadDbContext;
        }

        public async Task Consume(ConsumeContext<ProductDeletedInWriteDbEvent> context)
        {
            // var upd=Builders<ProductItemReadDbCollection>.Update.Set(s=>s.IsDeleted,true);
            // await _productReadDbContext.ProductItemsDataCollection.UpdateOneAsync(f=>f.ProductId==context.Message.ProductId,upd);
            await _productReadDbContext.ProductItemsDataCollection.DeleteOneAsync(f=>f.ProductId==context.Message.ProductId);
        }
    }
}