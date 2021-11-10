using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query.Search_Filters
{
    public class GetSearchSuggestionsQueryConsumer : IConsumer<GetSearchSuggestionsQuery>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _readDbContext;
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ProductCategory> _productCategories;
        private readonly ILogger<GetSearchSuggestionsQueryConsumer> _logger;

        public GetSearchSuggestionsQueryConsumer(ILogger<GetSearchSuggestionsQueryConsumer> logger,  IUnitOfWork uow, IProductReadDbContext readDbContext)
        {
            _logger = logger;

            _uow = uow;
            _productCategories = _uow.Set<ProductCategory>();
            _readDbContext = readDbContext;
        }

        public async Task Consume(ConsumeContext<GetSearchSuggestionsQuery> context)
        {
            //========to do fix it 
            try
            {
                var productCollection = _readDbContext.ProductItemsDataCollection;
                var categories = await _productCategories
                    .Where(w => w.TenantId == context.Message.TenantId)
                    .Where(w => w.Name.Contains(context.Message.SearchQuery))
                    .Select(s => new GetSearchSuggestionsQueryResponseItem { Id = s.Id, Name = s.Name })
                    .Take(3).AsNoTracking().ToListAsync();
                //if (categories.Count == 0)
                //{
                int skip = (context.Message.PageNumber - 1) * context.Message.PageSize;
                var products = await productCollection
                .Find(f => f.TenantId == context.Message.TenantId && f.IsAvailable && f.Name.Contains(context.Message.SearchQuery))
                .Project(p => new GetSearchSuggestionsQueryResponseItem
                {
                    Id = p.ProductId,
                    Name = p.Name
                }).Skip(skip).Limit(context.Message.PageSize).ToListAsync();
                await context.RespondAsync(new GetSearchSuggestionsQueryResponse { Categories = categories, Products = products });
                //}
                //else
                //{
                //    var products = await productCollection.Find(f => categories.FirstOrDefault().Id==f.CategoryId).Project(p => new GetSearchSuggestionsQueryResponseItem { Id = p.ProductId, Name = p.Name }).Limit(7).ToListAsync();
                //    await context.RespondAsync(new GetSearchSuggestionsQueryResponse { Categories = categories, Products = products });
                //}
            }
            catch (Exception e)
            {
                await context.RespondAsync(new GetSearchSuggestionsQueryResponse());

                _logger.LogDebug($"error occurred in GetSearchSuggestionsQueryConsumer with error {e.Message}!!!");
            }
        }

        public class GetSearchSuggestionsQueryResponse
        {
            public List<GetSearchSuggestionsQueryResponseItem> Categories { get; set; } = new List<GetSearchSuggestionsQueryResponseItem>();
            public List<GetSearchSuggestionsQueryResponseItem> Products { get; set; } = new List<GetSearchSuggestionsQueryResponseItem>();

        }
        public class GetSearchSuggestionsQueryResponseItem
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
    }
}
