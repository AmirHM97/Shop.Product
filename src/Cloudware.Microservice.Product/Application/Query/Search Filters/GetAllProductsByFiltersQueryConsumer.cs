using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using MassTransit;
using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Query
{
    public class GetAllProductsByFiltersQueryConsumer : IConsumer<GetAllProductsByFiltersQuery>, IMediatorConsumerType
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ProductItem> _productItems;
        private readonly IProductReadDbContext _readDbContext;
        private readonly ILogger<GetAllProductsByFiltersQueryConsumer> _logger;


        public GetAllProductsByFiltersQueryConsumer(IUnitOfWork uow,  ILogger<GetAllProductsByFiltersQueryConsumer> logger, IProductReadDbContext readDbContext)
        {
            _uow = uow;
            _productItems = _uow.Set<ProductItem>();
            _logger = logger;
            _readDbContext = readDbContext;
        }

        public async Task Consume(ConsumeContext<GetAllProductsByFiltersQuery> context)
        {
            var collection = _readDbContext.ProductItemsDataCollection;
            List<IFindFluent<ProductItemReadDbCollection, ProductItemReadDbCollection>> filters =new();
            List<IFindFluent<ProductItemReadDbCollection, ProductItemReadDbCollection>> sorts =new();

           // var productItems = _productItems.AsQueryable();
            //log information
            if (context.Message.CategoryId.HasValue)
                //productItems = productItems.Where(c => c.CategoryId == context.Message.CategoryId);
                filters.Add(collection.Find(f => f.CategoryId == context.Message.CategoryId));
            if (context.Message.BrandId.HasValue)
                // productItems = productItems.Where(c => c.BrandId == context.Message.BrandId);
                filters.Add(collection.Find(f => f.BrandId == context.Message.BrandId));
            if (context.Message.sortBy.HasValue)
            {
                #region comment
                //switch (context.Message.sortBy.Value)
                //{
                //    case SortBy.Latest:
                //        sorts.Add(collection.Find(_ => true).SortByDescending(s => s.CreatedDate));
                //        //  productItems = productItems.OrderByDescending(o => o.CreatedDate);
                //        break;
                //    case SortBy.SpecialOffer:
                //        break;
                //    case SortBy.MostViewed:
                //        sorts.Add(collection.Find(_ => true).SortByDescending(s => s.ViewCount));
                //        //productItems = productItems.OrderByDescending(o => o.ViewCount);
                //        break;
                //    case SortBy.Newest:
                //        // productItems = productItems.OrderByDescending(o => o.LastUpdatedDate);
                //        sorts.Add(collection.Find(_ => true).SortByDescending(s => s.LastUpdatedDate));
                //        break;
                //    case SortBy.Hottest:
                //        // productItems = productItems.OrderByDescending(o => o.TotalSoldCount);
                //        sorts.Add(collection.Find(_ => true).SortByDescending(s => s.TotalSoldCount));
                //        break;
                //    case SortBy.MostExpensive:
                //        sorts.Add(collection.Find(_ => true).SortByDescending(s=>s.MinPrice));
                //        // productItems = productItems.OrderBy(o => o.StocksItems.Select(si=>si.Price));
                //        break;
                //    case SortBy.LeastExpensive:
                //        sorts.Add(collection.Find(_ => true).SortBy(s => s.MinPrice));
                //        //  productItems = productItems.OrderByDescending(o => o.StocksItems.Select(si => si.Price));
                //        // productItems = productItems.Select(s=>s.StocksItems.);
                //        break;
                //    default:
                //        break;
                //} 

                #endregion
                sorts.Add(ProductExtensions.GetSortFilter(context.Message.sortBy.Value, collection));
            }
            if (sorts.Count == 0)
                sorts.Add(collection.Find(_ => true));
            if (filters.Count == 0)
                filters.Add(collection.Find(_ => true));
            //  var b =  collection.Find(filters.Select(g => g.Filter).FirstOrDefault()).Sort(sorts.Select(g => g.Options).FirstOrDefault().Sort).FirstOrDefault();
            try
            {
                var productItemsListCatalogDto = await collection.Find(filters.Select(g => g.Filter).Aggregate((a, b) => { return a & b; }))
                      .Project(p => new ProductItemsListCatalogDto
                      {
                          Id = p.ProductId,
                          image = p.ImageUrl,
                          Name = p.Name,
                          Discount = p.StockItemsDto.OrderBy(o => o.Price).Select(s => s.Discount).FirstOrDefault(),
                          Price = p.StockItemsDto.OrderBy(o => o.Price).Select(s => s.Price).FirstOrDefault()
                      }).Sort(sorts.Select(g => g.Options).FirstOrDefault().Sort).Skip((context.Message.PageNumber - 1) * context.Message.PageSize).Limit(context.Message.PageSize).ToListAsync();
                var result = new GetAllProductsByFiltersQueryResponse(productItemsListCatalogDto);
                await context.RespondAsync(result);
            }
            catch (Exception e)
            {
                _logger.LogError("Filter Products Failed With error :" + e.Message);
                await context.RespondAsync(new List<ProductItemsListCatalogDto>());
            }

        }


        public class GetAllProductsByFiltersQueryResponse
        {
            public List<ProductItemsListCatalogDto> ProductItemsListCatalogsDto { get; set; }

            public GetAllProductsByFiltersQueryResponse(List<ProductItemsListCatalogDto> productItemsListCatalogsDto)
            {
                ProductItemsListCatalogsDto = productItemsListCatalogsDto;
            }
        }
        //public async Task<List<ProductItemsListCatalogDto>> Handle(GetAllProductsByFiltersQuery context.Message, CancellationToken cancellationToken)
        //{
        //    var collection = _readDbContext.ProductItemsDataCollection;
        //    List<IFindFluent<ProductItemReadDbCollection, ProductItemReadDbCollection>> filters =
        //        new List<IFindFluent<ProductItemReadDbCollection, ProductItemReadDbCollection>>();
        //    List<IFindFluent<ProductItemReadDbCollection, ProductItemReadDbCollection>> sorts =
        //       new List<IFindFluent<ProductItemReadDbCollection, ProductItemReadDbCollection>>();

        //    var productItems = _productItems.AsQueryable();
        //    //log information
        //    if (context.Message.CategoryId.HasValue)
        //        //productItems = productItems.Where(c => c.CategoryId == context.Message.CategoryId);
        //        filters.Add(collection.Find(f => f.CategoryId == context.Message.CategoryId));
        //    if (context.Message.BrandId.HasValue)
        //        // productItems = productItems.Where(c => c.BrandId == context.Message.BrandId);
        //        filters.Add(collection.Find(f => f.BrandId == context.Message.BrandId));
        //    if (context.Message.sortBy.HasValue)
        //    {
        //        #region comment
        //        //switch (context.Message.sortBy.Value)
        //        //{
        //        //    case SortBy.Latest:
        //        //        sorts.Add(collection.Find(_ => true).SortByDescending(s => s.CreatedDate));
        //        //        //  productItems = productItems.OrderByDescending(o => o.CreatedDate);
        //        //        break;
        //        //    case SortBy.SpecialOffer:
        //        //        break;
        //        //    case SortBy.MostViewed:
        //        //        sorts.Add(collection.Find(_ => true).SortByDescending(s => s.ViewCount));
        //        //        //productItems = productItems.OrderByDescending(o => o.ViewCount);
        //        //        break;
        //        //    case SortBy.Newest:
        //        //        // productItems = productItems.OrderByDescending(o => o.LastUpdatedDate);
        //        //        sorts.Add(collection.Find(_ => true).SortByDescending(s => s.LastUpdatedDate));
        //        //        break;
        //        //    case SortBy.Hottest:
        //        //        // productItems = productItems.OrderByDescending(o => o.TotalSoldCount);
        //        //        sorts.Add(collection.Find(_ => true).SortByDescending(s => s.TotalSoldCount));
        //        //        break;
        //        //    case SortBy.MostExpensive:
        //        //        sorts.Add(collection.Find(_ => true).SortByDescending(s=>s.MinPrice));
        //        //        // productItems = productItems.OrderBy(o => o.StocksItems.Select(si=>si.Price));
        //        //        break;
        //        //    case SortBy.LeastExpensive:
        //        //        sorts.Add(collection.Find(_ => true).SortBy(s => s.MinPrice));
        //        //        //  productItems = productItems.OrderByDescending(o => o.StocksItems.Select(si => si.Price));
        //        //        // productItems = productItems.Select(s=>s.StocksItems.);
        //        //        break;
        //        //    default:
        //        //        break;
        //        //} 

        //        #endregion
        //        sorts.Add(ProductExtensions.GetSortFilter(context.Message.sortBy.Value, collection));
        //    }
        //    if (sorts.Count == 0)
        //        sorts.Add(collection.Find(_ => true));
        //    if (filters.Count == 0)
        //        filters.Add(collection.Find(_ => true));
        //    //  var b =  collection.Find(filters.Select(g => g.Filter).FirstOrDefault()).Sort(sorts.Select(g => g.Options).FirstOrDefault().Sort).FirstOrDefault();
        //    try
        //    {
        //        var productItemsListCatalogDto = await collection.Find(filters.Select(g => g.Filter).Aggregate((a, b) => { return a & b; }))
        //              .Project(p => new ProductItemsListCatalogDto
        //              {
        //                  Id = p.ProductId,
        //                  image = p.ImageUrl,
        //                  Name = p.Name,
        //                  Discount = p.StockItemsDto.OrderBy(o => o.Price).Select(s => s.Discount).FirstOrDefault(),
        //                  Price = p.StockItemsDto.OrderBy(o => o.Price).Select(s => s.Price).FirstOrDefault()
        //              }).Sort(sorts.Select(g => g.Options).FirstOrDefault().Sort).Skip((context.Message.PageNumber - 1) * context.Message.PageSize).Limit(context.Message.PageSize).ToListAsync();
        //        return productItemsListCatalogDto;
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError("Filter Products Failed With error :"+ e.Message);
        //        return null;
        //    }

        //    //await productItems.Select(s => new ProductItemsListCatalogDto { Price = s.StocksItems.OrderBy(o => o.Price).Select(sp => sp.Price).FirstOrDefault(), Name = s.Name, image = s.Image, Id = s.Id }).ToListAsync();

        //}
    }
}
