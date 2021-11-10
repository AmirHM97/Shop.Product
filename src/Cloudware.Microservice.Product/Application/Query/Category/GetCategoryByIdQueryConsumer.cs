using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using MongoDB.Driver;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public class GetCategoryByIdQueryConsumer : IConsumer<GetCategoryByIdQuery>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _productReadDbContext;

        public GetCategoryByIdQueryConsumer(IProductReadDbContext productReadDbContext)
        {
            _productReadDbContext = productReadDbContext;
        }

        public async Task Consume(ConsumeContext<GetCategoryByIdQuery> context)
        {
            var category = await _productReadDbContext.ProductCategoryNormalizedCollection.Find(f => f.CategoryId == context.Message.Id && f.TenantId == context.Message.TenantId).Project(p => new GetCategoryByIdQueryResponse
            {
                Id = p.CategoryId,
                Description = p.Description,
                Guid=p.Guid,
                Icon = p.Icon,
                Name = p.Name,
                ParentId = p.ParentId
            }).FirstOrDefaultAsync();
            await context.RespondAsync(category);
        }
    }
    public class GetCategoryByIdQueryResponse
    {
        public long Id { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }
}