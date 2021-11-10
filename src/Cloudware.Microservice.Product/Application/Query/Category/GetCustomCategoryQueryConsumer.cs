using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using MongoDB.Driver;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public class GetCustomCategoryQueryConsumer : IConsumer<GetCustomCategoryQuery>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _readDbContext;

        public GetCustomCategoryQueryConsumer(IProductReadDbContext readDbContext)
        {
            _readDbContext = readDbContext;
        }

        public async Task Consume(ConsumeContext<GetCustomCategoryQuery> context)
        {
           var collection=_readDbContext.CustomCategoryCollection;
           var res=await collection.Find(f=>f.ClientId==context.Message.TenantId).FirstOrDefaultAsync();
            await context.RespondAsync(new GetCustomCategoryQueryResponse(res));
        }
    }
    public class GetCustomCategoryQueryResponse
    {
        public CustomCategoryCollection Categories { get; set; }

        public GetCustomCategoryQueryResponse(CustomCategoryCollection categories)
        {
            Categories = categories;
        }
    }


}