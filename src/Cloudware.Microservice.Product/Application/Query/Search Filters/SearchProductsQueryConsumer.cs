using Cloudware.Microservice.Product.Application.Query.Category;
using Cloudware.Microservice.Product.Application.Query.Product;
using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Infrastructure.Extensions;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Table;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query.Search_Filters
{
    public class SearchProductsQueryConsumer : IConsumer<SearchProductsQuery>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _context;
        private readonly ILogger<SearchProductsQueryConsumer> _logger;
        private readonly IPaginationService _paginationService;
        private readonly IMediator _mediator;
        public SearchProductsQueryConsumer(ILogger<SearchProductsQueryConsumer> logger, IPaginationService paginationService, IMediator mediator, IProductReadDbContext context)
        {

            _logger = logger;
            _paginationService = paginationService;
            _mediator = mediator;
            _context = context;
        }

        public async Task Consume(ConsumeContext<SearchProductsQuery> context)
        {
            if (context.Message.useV1)
            {
                await SearchV1(context);
            }
            else
            {
                await SearchV2(context);
            }
        }

        private async Task SearchV1(ConsumeContext<SearchProductsQuery> context)
        {
            var collection = _context.ProductItemsDataCollection;
            List<IFindFluent<ProductItemReadDbCollection, ProductItemReadDbCollection>> filters = new();
            List<IFindFluent<ProductItemReadDbCollection, ProductItemReadDbCollection>> sorts = new();
            if (context.Message.Searches.Count != 0 || context.Message.Searches == null)
            {
                foreach (var item in context.Message.Searches)
                {

                    switch (item.SearchOn)
                    {

                        case SearchOn.Category:
                            if (item.SelectionIds.Count != 0 && item.SelectionIds != null && !item.SelectionIds.Contains(0))
                            {
                                filters.Add(collection.Find(c => item.SelectionIds.Contains(c.CategoryId)));
                                break;
                            }
                            filters.Add(collection.Find(_ => true));
                            break;
                        case SearchOn.Brand:
                            if (item.SelectionIds.Count != 0)
                                filters.Add(collection.Find(c => item.SelectionIds.Contains(c.BrandId)));
                            filters.Add(collection.Find(_ => true));
                            break;
                        case SearchOn.Price:
                            var max = item.MaxValue.Value;
                            var min = item.MinValue.Value;
                            var findFluent = collection
                                .Find(Builders<ProductItemReadDbCollection>.Filter
                                .ElemMatch(
                                           foo => foo.StockItemsDto,
                                           foobar => foobar.Price < max && foobar.Price > min));
                            filters.Add(findFluent);
                            break;
                        case SearchOn.IsExist:
                            filters.Add(collection.Find(c => c.IsAvailable == true));
                            break;
                        case SearchOn.Text:
                            filters.Add(collection.Find(s => s.Name.Contains(item.SearchQuery)));
                            break;
                        case SearchOn.HaveDiscount:
                            filters.Add(collection.Find(s => s.StockItemsDto.Any(a => a.Discount > 0)));
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                filters.Add(collection.Find(_ => true));
            }
            if (context.Message.sortBy.HasValue)
                sorts.Add(ProductExtensions.GetSortFilter(context.Message.sortBy.Value, collection));
            else
                sorts.Add(collection.Find(_ => true));
            try
            {

                int skip = (context.Message.PageNumber - 1) * context.Message.PageSize;

                var productsList = await collection.Find(filters.Select(g => g.Filter)
                       .Aggregate((a, b) => { return a & b; }))
                       .Project(s => new ProductItemsListCatalogDto
                       {
                           Id = s.ProductId,
                           image = s.ImageUrl,
                           Name = s.Name,
                           Discount = s.StockItemsDto.Select(s => s.Discount).FirstOrDefault(),
                           Price = s.StockItemsDto.Select(p => p.Price).FirstOrDefault()
                       }).Sort(sorts.Select(g => g.Options).FirstOrDefault().Sort).ToListAsync();
                _paginationService.Pagination(productsList.AsQueryable(), context.Message.PageSize);

                var productListWithPagination = productsList.Skip(skip).Take(context.Message.PageSize).ToList();
                //}).Sort(sorts.Select(g => g.Options).FirstOrDefault().Sort).ToListAsync();
                await context.RespondAsync(new SearchProductsQueryResponse(productListWithPagination));
            }
            catch (Exception e)
            {
                _logger.LogError("Search Products Failed With Error : " + e.Message);
                var productIdsList = new List<ProductItemsListCatalogDto>();
                await context.RespondAsync(new SearchProductsQueryResponse(productIdsList));

            }
        }

        private async Task SearchV2(ConsumeContext<SearchProductsQuery> context)
        {
            var collection = _context.ProductItemsDataCollection;
            List<IFindFluent<ProductItemReadDbCollection, ProductItemReadDbCollection>> filters = new();
            List<IFindFluent<ProductItemReadDbCollection, ProductItemReadDbCollection>> sorts = new();
            filters.Add(collection.Find(c => c.TenantId == context.Message.TenantId));
            filters.Add(collection.Find(c => c.IsAvailable));
            if (context.Message.SearchesV2.CategoryIds.Count != 0)
            {
                var categoryIds = context.Message.SearchesV2.CategoryIds;
                var req = _mediator.CreateRequestClient<GetCategoryChildrenIdQuery>();
                var allCategoryIds = await req.GetResponse<GetCategoryChildrenIdQueryResponse>(new GetCategoryChildrenIdQuery(categoryIds, context.Message.TenantId));
                var categoryCollection = _context.ProductCategoriesDataCollection;

                filters.Add(collection.Find(c => allCategoryIds.Message.CategoryIds.Contains(c.CategoryId)));
            }
            if (context.Message.SearchesV2.BrandIds.Count != 0)
            {
                filters.Add(collection.Find(c => context.Message.SearchesV2.BrandIds.Contains(c.BrandId)));
            }
            if (context.Message.SearchesV2.DeliverdBySeller)
            {
                //=======================================ToDo==================================
                // filters.Add(collection.Find(c => context.Message.SearchesV2.CategoryIds.Contains(c.CategoryId)));
            }
            if (context.Message.SearchesV2.IsAvailable)
            {
                filters.Add(collection.Find(c => c.IsAvailable));
            }
            if (context.Message.SearchesV2.HaveDiscount)
            {
                filters.Add(collection.Find(s => s.StockItemsDto.Any(a => a.Discount > 0)));
                // var filt=collection.Find(Builders<ProductItemReadDbCollection>.filter
                // .EleMatch(
                //     stock=>stock.StockItemsDto,
                //     maxPrice=>maxPrice.Discount
                // ));
            }
            if (context.Message.SearchesV2.MaxValue > 0)
            {
                var max = context.Message.SearchesV2.MaxValue;
                var min = context.Message.SearchesV2.MinValue;
                var findFluent = collection
                    .Find(Builders<ProductItemReadDbCollection>.Filter
                    .ElemMatch(
                               foo => foo.StockItemsDto,
                               foobar => foobar.Price < max && foobar.Price > min));
                filters.Add(findFluent);
            }
            if (context.Message.SearchesV2.SearchQuery != "" && context.Message.SearchesV2.SearchQuery != null)
            {
                filters.Add(collection.Find(s => s.Name.Contains(context.Message.SearchesV2.SearchQuery)));
            }

            if (filters.Count == 0)
            {
                filters.Add(collection.Find(_ => true));
            }
            //==========Sorts=============
            sorts.Add(ProductExtensions.GetSortFilter(context.Message.SearchesV2.SortBy, collection));
            try
            {
                int skip = (context.Message.PageNumber - 1) * context.Message.PageSize;

                var productsList = collection.Find(filters.Select(g => g.Filter)
                       .Aggregate((a, b) => { return a & b; }))
                       .Project(s => new ProductItemsListCatalogDto
                       {
                           Id = s.ProductId,
                           image = s.ImageUrl,
                           Name = s.Name,
                           Discount = s.StockItemsDto.Select(s => s.Discount).FirstOrDefault(),
                           Price = s.StockItemsDto.Select(p => p.Price).FirstOrDefault()
                       }).Sort(sorts.Select(g => g.Options).FirstOrDefault().Sort);
                if (context.Message.Addpagination)
                {

                    _paginationService.Pagination(context.Message.PageSize, Convert.ToInt32(await productsList.CountDocumentsAsync()));
                }
                    var productListWithPagination = await productsList.Skip(skip).Limit(context.Message.PageSize).ToListAsync();
                //}).Sort(sorts.Select(g => g.Options).FirstOrDefault().Sort).ToListAsync();
                await context.RespondAsync(new SearchProductsQueryResponse(productListWithPagination));
            }
            catch (Exception e)
            {
                _logger.LogError("Search Products Failed With Error : " + e.Message);
                var productIdsList = new List<ProductItemsListCatalogDto>();
                await context.RespondAsync(new SearchProductsQueryResponse(productIdsList));

            }
        }



        public class SearchProductsQueryResponse
        {
            public List<ProductItemsListCatalogDto> ProductItemsListCatalogsDto { get; set; }

            public SearchProductsQueryResponse(List<ProductItemsListCatalogDto> productItemsListCatalogsDto)
            {
                ProductItemsListCatalogsDto = productItemsListCatalogsDto;
            }
        }
    }
}

#region comment
//    public async Task<List<ProductItemsListCatalogDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
//    {
//        //var collection = _context.ProductItemsDataCollection;
//        //List<IFindFluent<ProductItemReadDbCollection, ProductItemReadDbCollection>> filters = new List<IFindFluent<ProductItemReadDbCollection, ProductItemReadDbCollection>>();
//        //List<IFindFluent<ProductItemReadDbCollection, ProductItemReadDbCollection>> sorts = new List<IFindFluent<ProductItemReadDbCollection, ProductItemReadDbCollection>>();
//        //if (request.Searches.Count != 0)
//        //{
//        //    foreach (var item in request.Searches)
//        //    {
//        //        switch (item.SearchOn)
//        //        {

//        //            case SearchOn.Category:
//        //                if (item.SelectionIds.Count != 0)
//        //                    filters.Add(collection.Find(c => item.SelectionIds.Contains(c.CategoryId)));
//        //                filters.Add(collection.Find(_ => true));
//        //                break;
//        //            case SearchOn.Brand:
//        //                if (item.SelectionIds.Count != 0)
//        //                    filters.Add(collection.Find(c => item.SelectionIds.Contains(c.BrandId)));
//        //                filters.Add(collection.Find(_ => true));
//        //                break;
//        //            case SearchOn.Price:
//        //                //filters.Add(collection.Find(f => f.StockItemsDto.Select(s => s.Price).FirstOrDefault() < item.MaxValue.Value
//        //                //                              && f.StockItemsDto.Select(s => s.Price).FirstOrDefault() > item.MinValue.Value));
//        //                var max = item.MaxValue.Value;
//        //                var min = item.MinValue.Value;
//        //                //filters.Add(collection.Find(f => f.StockItemsDto[0].Price < a));
//        //                //filters.Add(collection.Find(f => f.StockItemsDto[0].Price > b));

//        //                var findFluent = collection
//        //                    .Find(Builders<ProductItemReadDbCollection>.Filter
//        //                    .ElemMatch(
//        //                               foo => foo.StockItemsDto,
//        //                               foobar => foobar.Price < max && foobar.Price > min));
//        //                filters.Add(findFluent);
//        //                #region comment
//        //                //filters.Add(findFluent1);

//        //                //var a= collection.Find(f => f.StockItemsDto.Any(d=>d.Price < item.MaxValue.Value)).ToList();
//        //                //var a= collection.Find(f => f.StockItemsDto.Any(d=>d.Price < item.MaxValue.Value)).ToList();
//        //                //var r = collection.Find(f => f.ProductId == 4).FirstOrDefault();
//        //                //var q = item.MaxValue.Value;
//        //                //var asds = r.StockItemsDto[0].Price;
//        //                //if (r.StockItemsDto[0].Price< item.MaxValue.Value)
//        //                //{
//        //                //    var fg = 6;
//        //                //}
//        //                //filters.Add(collection.Find(f => f.StockItemsDto[0].Price < item.MaxValue.Value
//        //                //                              && f.StockItemsDto[0].Price > item.MinValue.Value));
//        //                //collection.AsQueryable()
//        //                //    .Where(c => c.StockItemsDto.Select(s => s.Price).FirstOrDefault() < item.MaxValue)
//        //                //    .Where(c => c.StockItemsDto.Select(s => s.Price).FirstOrDefault() > item.MinValue); 
//        //                #endregion
//        //                break;
//        //            case SearchOn.IsExist:
//        //                filters.Add(collection.Find(c => c.IsAvailable == true));
//        //                break;
//        //            case SearchOn.Text:
//        //                filters.Add(collection.Find(s => s.Name.Contains(item.SearchQuery)));
//        //                break;
//        //            default:
//        //                break;
//        //        }
//        //    }
//        //}
//        //else
//        //    filters.Add(collection.Find(_ => true));
//        //if (request.sortBy.HasValue)
//        //    sorts.Add(ProductExtensions.GetSortFilter(request.sortBy.Value, collection));
//        //else
//        //    sorts.Add(collection.Find(_ => true));
//        //// var productIdsList = await collection.Find(filters.Select(g => g.Filter).Aggregate((a, b) => { return a & b; })).Project(p => p.ProductId).ToListAsync();
//        //try
//        //{
//        //    var productIdsList = await collection.Find(filters.Select(g => g.Filter)
//        //           .Aggregate((a, b) => { return a & b; }))
//        //           .Project(s => new ProductItemsListCatalogDto
//        //           {
//        //               Id = s.ProductId,
//        //               image = s.ImageUrl,
//        //               Name = s.Name,
//        //               Discount = s.StockItemsDto.Select(s => s.Discount).FirstOrDefault(),
//        //               Price = s.StockItemsDto.Select(p => p.Price).FirstOrDefault()
//        //           }).Sort(sorts.Select(g => g.Options).FirstOrDefault().Sort).Skip((request.PageNumber - 1) * request.PageSize).Limit(request.PageSize).ToListAsync();
//        //    return productIdsList;
//        //}
//        //catch (Exception e)
//        //{
//        //    _logger.LogError("Search Products Failed With Error : " + e.Message);
//        //    return null;
//        //}
//        //// return await _mediator.Send(new GetListOfProductsByIdsQuery(productIdsList));
//    }
//}
#endregion