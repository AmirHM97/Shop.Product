using Cloudware.Microservice.Product.Application.Query.Category;
using Cloudware.Microservice.Product.Application.Query.Search_Filters;
using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Cloudware.Microservice.Product.Application.Query.GetAllProductsByFiltersQueryConsumer;
using static Cloudware.Microservice.Product.Application.Query.Search_Filters.SearchProductsQueryConsumer;

namespace Cloudware.Microservice.Product.Application.Query.HomePage
{
    public class GetHomePageDataQueryConsumer : IConsumer<GetHomePageDataQuery>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _context;
        private readonly IRequestClient<GetAllProductsByFiltersQuery> _getAllProductsByFiltersQuery;
        private readonly IRequestClient<SearchProductsQuery> _searchProductsQuery;
        private readonly IMediator _mediator;

        public GetHomePageDataQueryConsumer(IRequestClient<GetAllProductsByFiltersQuery> getAllProductsByFiltersQuery, IRequestClient<SearchProductsQuery> searchProductsQuery, IProductReadDbContext context, IMediator mediator)
        {
            _getAllProductsByFiltersQuery = getAllProductsByFiltersQuery;
            _searchProductsQuery = searchProductsQuery;
            _context = context;
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<GetHomePageDataQuery> context)
        {

            HomePageDto homePageDto = new();
            var HottestProducts = await _searchProductsQuery.GetResponse<SearchProductsQueryResponse>(new SearchProductsQuery(new AdvancedSearchItemDtoV2 { SortBy = SortBy.Hottest }, context.Message.TenantId));
            homePageDto.HottestProducts = new Section { Products = HottestProducts.Message.ProductItemsListCatalogsDto, Link = "?sortBy=4" };


            var MostViewedProducts = await _searchProductsQuery.GetResponse<SearchProductsQueryResponse>(new SearchProductsQuery(new AdvancedSearchItemDtoV2 { SortBy = SortBy.MostViewed }, context.Message.TenantId,10,1,false));
            homePageDto.MostViewedProducts = new Section { Products = MostViewedProducts.Message.ProductItemsListCatalogsDto, Link = "?sortBy=2" };

            var NewestProducts = await _searchProductsQuery.GetResponse<SearchProductsQueryResponse>(new SearchProductsQuery(new AdvancedSearchItemDtoV2 { SortBy = SortBy.Newest }, context.Message.TenantId,10,1,false));
            homePageDto.NewestProducts = new Section { Products = NewestProducts.Message.ProductItemsListCatalogsDto, Link = "?sortBy=3" };



            var SpecialOfferProducts = await _searchProductsQuery.GetResponse<SearchProductsQueryResponse>(new SearchProductsQuery(new AdvancedSearchItemDtoV2 { BrandIds = new List<long> { 131 } }, context.Message.TenantId,10,1,false));
            homePageDto.SpecialOfferProducts = new Section { Products = SpecialOfferProducts.Message.ProductItemsListCatalogsDto, Link = "?BrandIds=131" };
            var link = "";
            var foodIds = new List<long>
            {
                 390,389
                // 7408,2
                // 277,3
            };
            foreach (var item in foodIds)
            {
                link = $"{link}categoryId={item}&";
            }
            var req = _mediator.CreateRequestClient<GetCategoryChildrenIdQuery>();
            var foodProductsIds = await req.GetResponse<GetCategoryChildrenIdQueryResponse>(new GetCategoryChildrenIdQuery(foodIds, context.Message.TenantId));
            var FoodProducts = await _searchProductsQuery.GetResponse<SearchProductsQueryResponse>(new SearchProductsQuery(new AdvancedSearchItemDtoV2 { CategoryIds = foodProductsIds.Message.CategoryIds }, context.Message.TenantId,10,1,false));
            homePageDto.FoodProducts = new Section { Products = FoodProducts.Message.ProductItemsListCatalogsDto, Link = "?" + link.Remove(link.Length - 1) };


            var HaveDiscountProducts = await _searchProductsQuery.GetResponse<SearchProductsQueryResponse>(new SearchProductsQuery(new AdvancedSearchItemDtoV2 { HaveDiscount = true }, context.Message.TenantId,10,1,false));
            homePageDto.HaveDiscountProducts = new Section { Products = HaveDiscountProducts.Message.ProductItemsListCatalogsDto, Link = "?sortBy=3&HaveDiscount=true" };

            var idsWeekly = new List<long>
            {
                // 291,7400
                419,420
            };
            link = "";
            foreach (var item in idsWeekly)
            {
                link = $"{link}categoryId={item}&";
            }
            var weekly = await req.GetResponse<GetCategoryChildrenIdQueryResponse>(new GetCategoryChildrenIdQuery(idsWeekly, context.Message.TenantId));

            var WeeklySuggestedProducts = await _searchProductsQuery.GetResponse<SearchProductsQueryResponse>(new SearchProductsQuery(new AdvancedSearchItemDtoV2 { CategoryIds = weekly.Message.CategoryIds }, context.Message.TenantId,10,1,false));
            homePageDto.WeeklySuggestedProducts = new Section { Products = WeeklySuggestedProducts.Message.ProductItemsListCatalogsDto, Link = "?" + link.Remove(link.Length - 1) };


            await context.RespondAsync(homePageDto);
        }
    }
}
#region comment

//var HottestProducts = await _getAllProductsByFiltersQuery.GetResponse<GetAllProductsByFiltersQueryResponse>(new GetAllProductsByFiltersQuery(null, null, SortBy.Hottest));
//homePageDto.HottestProducts = new Section { Products = HottestProducts.Message.ProductItemsListCatalogsDto, Link = "?sortBy=3" };

//var MostViewdProducts = await _getAllProductsByFiltersQuery.GetResponse<GetAllProductsByFiltersQueryResponse>(new GetAllProductsByFiltersQuery(null, null, SortBy.MostViewed));
//homePageDto.MostViewdProducts = new Section { Products = MostViewdProducts.Message.ProductItemsListCatalogsDto, Link = "?sortBy=2" };

//var NewestProducts = await _getAllProductsByFiltersQuery.GetResponse<GetAllProductsByFiltersQueryResponse>(new GetAllProductsByFiltersQuery(null, null, SortBy.Newest));
//homePageDto.NewestProducts = new Section { Products = NewestProducts.Message.ProductItemsListCatalogsDto, Link = "?sortBy=3" };

//var SpecialOfferProducts = await _getAllProductsByFiltersQuery.GetResponse<GetAllProductsByFiltersQueryResponse>(new GetAllProductsByFiltersQuery(null, null, SortBy.MostExpensive));
//homePageDto.SpecialOfferProducts = new Section { Products = SpecialOfferProducts.Message.ProductItemsListCatalogsDto, Link = "?sortBy=4" };

//var FoodProducts = await _getAllProductsByFiltersQuery.GetResponse<GetAllProductsByFiltersQueryResponse>(new GetAllProductsByFiltersQuery(1, null, SortBy.Hottest));
//homePageDto.FoodProducts = new Section { Products = FoodProducts.Message.ProductItemsListCatalogsDto, Link = "?sortBy=5" };

////To Do =====================>>>> Fix
//var WeeklySuggestedProducts = await _getAllProductsByFiltersQuery.GetResponse<GetAllProductsByFiltersQueryResponse>(new GetAllProductsByFiltersQuery(null, null, SortBy.Hottest));
//homePageDto.WeeklySuggestedProducts = new Section { Products = WeeklySuggestedProducts.Message.ProductItemsListCatalogsDto, Link = "?sortBy=5" }; 
//public async Task<HomePageDto> Handle(GetHomePageDataQuery request, CancellationToken cancellationToken)
//{
//    HomePageDto homePageDto = new HomePageDto();
//    homePageDto.HottestProducts = (List<ProductItemsListCatalogDto>)await _getAllProductsByFiltersQuery.GetResponse<List<ProductItemsListCatalogDto>>(new GetAllProductsByFiltersQuery(null, null, SortBy.Hottest));
//    homePageDto.MostViewdProducts = (List<ProductItemsListCatalogDto>)await _getAllProductsByFiltersQuery.GetResponse<List<ProductItemsListCatalogDto>>(new GetAllProductsByFiltersQuery(null, null, SortBy.MostViewed));
//    homePageDto.NewestProducts= (List<ProductItemsListCatalogDto>)await _getAllProductsByFiltersQuery.GetResponse<List<ProductItemsListCatalogDto>>(new GetAllProductsByFiltersQuery(null, null, SortBy.Newest));
//    homePageDto.SpecialOfferProducts= (List<ProductItemsListCatalogDto>)await _getAllProductsByFiltersQuery.GetResponse<List<ProductItemsListCatalogDto>>(new GetAllProductsByFiltersQuery(null, null, SortBy.Hottest));
//    homePageDto.FoodProducts= (List<ProductItemsListCatalogDto>)await _getAllProductsByFiltersQuery.GetResponse<List<ProductItemsListCatalogDto>>(new GetAllProductsByFiltersQuery(null, null, SortBy.MostExpensive));
//    //To Do =====================>>>> Fix
//    homePageDto.WeeklySuggestedProducts= (List<ProductItemsListCatalogDto>)await _getAllProductsByFiltersQuery.GetResponse<List<ProductItemsListCatalogDto>>(new GetAllProductsByFiltersQuery(null, null, SortBy.Hottest));
//    return homePageDto;
//}
#endregion
