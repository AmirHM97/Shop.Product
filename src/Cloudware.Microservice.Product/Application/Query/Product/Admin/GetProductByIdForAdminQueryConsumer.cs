using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Application.Query.Category;
using Cloudware.Microservice.Product.Application.Query.Stock;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Formbuilder.Services;
using Cloudware.Utilities.Table;
using MassTransit;
using MassTransit.Mediator;
using MongoDB.Driver;

namespace Cloudware.Microservice.Product.Application.Query.Product.Admin
{
    public class GetProductByIdForAdminQueryConsumer : IConsumer<GetProductByIdForAdminQuery>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _productReadDbContext;
        private readonly IMediator _mediator;

        public GetProductByIdForAdminQueryConsumer(IProductReadDbContext productReadDbContext, IMediator mediator)
        {
            _productReadDbContext = productReadDbContext;
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<GetProductByIdForAdminQuery> context)
        {
            
            var collection = _productReadDbContext.ProductItemsDataCollection;
            var productItemDto = await collection.Find(f => f.Guid.ToLower() == context.Message.Guid.ToLower() && f.TenantId == context.Message.TenantId && !f.IsDeleted.Value).FirstOrDefaultAsync();
            // if (!string.IsNullOrEmpty (productItemDto.TechnicalPropertyFormId ))
            // {

            var tableReq = _mediator.CreateRequestClient<GetProductStocksTableQuery>();
            var tTableRes = tableReq.GetResponse<GetProductStocksTableQueryResponse>(new GetProductStocksTableQuery(new(), productItemDto.ProductId, context.Message.TenantId));

            var formReq = _mediator.CreateRequestClient<GetCategoryFormWithRecordsQuery>();
            var tFormRes = formReq.GetResponse<GetCategoryFormWithRecordsQueryResponse>(new GetCategoryFormWithRecordsQuery(context.Message.TenantId, productItemDto.CategoryId,productItemDto?.TechnicalPropertyRecordId??""));

            var categoryParentsReq = _mediator.CreateRequestClient<GetCategoryParentsQuery>();
            var tCategoryParentsRes = categoryParentsReq.GetResponse<GetCategoryParentsQueryResponse>(new GetCategoryParentsQuery(context.Message.TenantId, productItemDto.CategoryId));

            Task.WaitAll(tFormRes, tTableRes, tCategoryParentsRes);
            var tableRes = await tTableRes;
            var formRes = await tFormRes;
            var categoryParentsRes = await tCategoryParentsRes;
            var parents = categoryParentsRes.Message.ProductCategories.Count > 0 ? new CategoryParentDto(categoryParentsRes.Message.ProductCategories.Select(s => s.Name).ToList()) : new(new());
            await context.RespondAsync(new GetProductByIdForAdminQueryResponse(productItemDto, tableRes.Message.TableData, formRes.Message.Form, parents));
            // }
            // await context.RespondAsync(new GetProductByIdForAdminQueryResponse(productItemDto, new(), new ()));

        }
    }
    public class GetProductByIdForAdminQueryResponse
    {
        public ProductItemReadDbCollection ProductData { get; set; }
        public TableData StockTable { get; set; }
        public FormWithRecord FormWithRecord { get; set; }
        public CategoryParentDto CategoryParents { get; set; }

        public GetProductByIdForAdminQueryResponse(ProductItemReadDbCollection productData, TableData stockTable, FormWithRecord formWithRecord, CategoryParentDto categoryParents)
        {
            ProductData = productData;
            StockTable = stockTable;
            FormWithRecord = formWithRecord;
            CategoryParents = categoryParents;
        }
    }
    public class CategoryParentDto
    {
        public CategoryParentDto(List<string> categoryParents)
        {
            CategoryParents = categoryParents;
        }

        public List<string> CategoryParents { get; set; }

    }

}