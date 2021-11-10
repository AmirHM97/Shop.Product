using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Infrastructure.Services;
using Cloudware.Utilities.Contract.Abstractions;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public class GetCategoryListQueryConsumer : IConsumer<GetCategoryListQuery>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _productReadDbContext;
        private readonly IPaginationService _paginationService;

        public GetCategoryListQueryConsumer(IProductReadDbContext productReadDbContext, IPaginationService paginationService)
        {
            _productReadDbContext = productReadDbContext;
            _paginationService = paginationService;
        }

        public async Task Consume(ConsumeContext<GetCategoryListQuery> context)
        {
            var collection = _productReadDbContext.ProductCategoryNormalizedCollection.AsQueryable();
            var skip = (context.Message.PageNumber - 1) * context.Message.PageSize;
            // var categoriesQuery = collection.Where(f => f.TenantId == context.Message.TenantId ).Skip(skip).Take(context.Message.PageSize).Select(p => new GetCategoryListQueryResponseItem
            var categoriesQuery = collection.Where(f => f.TenantId == context.Message.TenantId && f.Name.Contains(context.Message.Query)).Select(p => new GetCategoryListQueryResponseItem
            {
                CategoryId = p.CategoryId,
                CategoryName = p.Name,
                ParentId = p.ParentId
            });
            _paginationService.Pagination(categoriesQuery, context.Message.PageSize);
            var categories = await categoriesQuery.Skip(skip).Take(context.Message.PageSize).ToListAsync();
            await context.RespondAsync(new GetCategoryListQueryResponse(categories));

        }
    }
    public class GetCategoryListQueryResponse
    {
        public List<GetCategoryListQueryResponseItem> Categories { get; set; }

        public GetCategoryListQueryResponse(List<GetCategoryListQueryResponseItem> categories)
        {
            Categories = categories;
        }
    }
    public class GetCategoryListQueryResponseItem
    {
        public string CategoryName { get; set; }
        public long CategoryId { get; set; }
        public long? ParentId { get; set; }
    }
}