using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.People;
using MassTransit;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Events.Product
{
    public class GetPeopleByIdResponseIntegrationEventConsumer : IConsumer<GetPeopleByIdResponseIntegrationEvent>, IBusConsumerType
    {
        private readonly IProductReadDbContext _readContext;
        public GetPeopleByIdResponseIntegrationEventConsumer(IProductReadDbContext readContext)
        {
            _readContext = readContext;
        }
        public async Task Consume(ConsumeContext<GetPeopleByIdResponseIntegrationEvent> context)
        {
            var collection = _readContext.ProductItemsDataCollection;                  
            var product = await collection.Find(f => new Guid(f.Guid) == context.CorrelationId).FirstOrDefaultAsync();

            var up = Builders<ProductItemReadDbCollection>.Update.Set(s => s.UserDescription, context.Message.People.Description).Set(s => s.UserImageUrl,context.Message.People.ImageUrl).Set(s=>s.UserName,context.Message.People.FirstName +"" + context.Message.People.LastName);
            await _readContext.ProductItemsDataCollection.UpdateOneAsync(w=>w.Guid == product.Guid, up);
        }
    }
}
