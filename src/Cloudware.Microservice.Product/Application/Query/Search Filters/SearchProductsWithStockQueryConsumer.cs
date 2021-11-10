using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Utilities.Contract.Abstractions;
using DocumentFormat.OpenXml.Office2013.Word;
using MassTransit;
using MongoDB.Driver;

namespace Cloudware.Microservice.Product.Application.Query.Search_Filters
{
    public class SearchProductsWithStockQueryConsumer : IConsumer<SearchProductsWithStockQuery>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _readDbContext;

        public SearchProductsWithStockQueryConsumer(IProductReadDbContext readDbContext)
        {
            _readDbContext = readDbContext;
        }

        public async Task Consume(ConsumeContext<SearchProductsWithStockQuery> context)
        {
            var productCollection = _readDbContext.ProductItemsDataCollection;

            int skip = (context.Message.PageNumber - 1) * context.Message.PageSize;
            IFindFluent<Model.ReadDbModel.ProductItemReadDbCollection, Model.ReadDbModel.ProductItemReadDbCollection> query;
            if (context.Message.IsAvailable is not null)
            {
                query = productCollection
                    //  .Find(f => f.TenantId == context.Message.TenantId && f.UserId == context.Message.UserId && f.IsAvailable == context.Message.IsAvailable && f.Name.Contains(context.Message.SearchQuery));
                     .Find(f => f.TenantId == context.Message.TenantId && f.IsAvailable == context.Message.IsAvailable && f.Name.Contains(context.Message.SearchQuery));
            }
            else
            {
                query = productCollection
                // .Find(f => f.TenantId == context.Message.TenantId && f.UserId == context.Message.UserId && f.Name.Contains(context.Message.SearchQuery));
                .Find(f => f.TenantId == context.Message.TenantId && f.Name.Contains(context.Message.SearchQuery));
            }
            var products = await query
             .Project(p => new SearchProductsWithStockQueryResponseItem
             {
                 ProductId = p.ProductId,
                 Name = p.Name,
                 StockId = p.StockItemsDto.Select(s => s.StockId).FirstOrDefault(),
                 Price = p.StockItemsDto.Select(s => s.Price).FirstOrDefault(),
                 Code = p.Code,
                 Image = p.ImageUrl,
                 IsAvailable = p.IsAvailable
             }).Skip(skip).Limit(context.Message.PageSize).SortByDescending(s => s.LastUpdatedDate).ToListAsync();
            await context.RespondAsync(new SearchProductsWithStockQueryResponse(products));
        }
    }
    public class SearchProductsWithStockQueryResponse
    {
        public List<SearchProductsWithStockQueryResponseItem> Products { get; set; }

        public SearchProductsWithStockQueryResponse(List<SearchProductsWithStockQueryResponseItem> products)
        {
            Products = products;
        }
    }
    public class SearchProductsWithStockQueryResponseItem
    {
        public long ProductId { get; set; }
        public long StockId { get; set; }
        public string Name { get; set; }
        public bool IsAvailable { get; set; }
        public string Image { get; set; }
        public string Code { get; set; }
        public decimal Price { get; set; }
    }
}