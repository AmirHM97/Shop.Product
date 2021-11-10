using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Formbuilder.Services;
using MassTransit;
using MongoDB.Driver;

namespace Cloudware.Microservice.Product.Application.Query.Product
{
    public class GetProductCategoryFormByProductIdQueryConsumer : IConsumer<GetProductCategoryFormByProductIdQuery>, IMediatorConsumerType
    {
        private readonly IRecordsService _recordsService;

        private readonly IProductReadDbContext _productReadDbContext;

        public GetProductCategoryFormByProductIdQueryConsumer(IRecordsService recordsService, IProductReadDbContext productReadDbContext)
        {
            _recordsService = recordsService;
            _productReadDbContext = productReadDbContext;
        }

        public async Task Consume(ConsumeContext<GetProductCategoryFormByProductIdQuery> context)
        {
            var product = await _productReadDbContext.ProductItemsDataCollection.Find(f => f.ProductId == context.Message.ProductId).FirstOrDefaultAsync();
            
            var res = await _recordsService.GetFormWithRecordWithRecordId(context.Message.TenantId, product.TechnicalPropertyRecordId, product.TechnicalPropertyFormId);
            await context.RespondAsync(new GetProductCategoryFormByProductIdQueryResponse(res));

        }
    }
      public class GetProductCategoryFormByProductIdQueryResponse
    {
        public FormWithRecord Form { get; set; }

        public GetProductCategoryFormByProductIdQueryResponse(FormWithRecord form)
        {
            Form = form;
        }
    }
}