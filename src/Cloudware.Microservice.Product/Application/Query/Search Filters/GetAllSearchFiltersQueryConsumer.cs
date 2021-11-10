using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Cloudware.Microservice.Product.Application.Query.GetAllProductBrandsQueryConsumer;

namespace Cloudware.Microservice.Product.Application.Query.Search_Filters
{
    public class GetAllSearchFiltersQueryConsumer : IConsumer<GetAllSearchFiltersQuery>, IMediatorConsumerType
    {

        private readonly IRequestClient<GetAllProductBrandsQuery> _getAllProductBrandsQuery;
        private readonly IRequestClient<GetAllProductCategoriesQuery> _getAllProductCategoriesQuery;

        public GetAllSearchFiltersQueryConsumer(IRequestClient<GetAllProductBrandsQuery> getAllProductBrandsQuery, IRequestClient<GetAllProductCategoriesQuery> getAllProductCategoriesQuery)
        {
            _getAllProductBrandsQuery = getAllProductBrandsQuery;
            _getAllProductCategoriesQuery = getAllProductCategoriesQuery;
        }



        public async Task Consume(ConsumeContext<GetAllSearchFiltersQuery> context)
        {
            var categories = await _getAllProductCategoriesQuery.GetResponse<GetAllProductCategoriesQueryResponse>(new GetAllProductCategoriesQuery(context.Message.TenantId));
            var brands = await _getAllProductBrandsQuery.GetResponse<GetAllProductBrandsQueryResponse>(new GetAllProductBrandsQuery(context.Message.TenantId,1000,1));
            var fliters = new List<SearchDto>();
            var sorts = new List<SortDto>
            {
                new SortDto { Id = 0, Name = "آخرین محصولات" },
                new SortDto { Id = 1, Name = "پیشنهادات ویژه" },
                new SortDto { Id = 2, Name = "پربازدیدها" },
                new SortDto { Id = 3, Name = "جدیدترین ها" },
                new SortDto { Id = 4, Name = "داغ ترین ها" },
                new SortDto { Id = 5, Name = "گران ترین ها" },
                new SortDto { Id = 6, Name = "ارزان ترین ها" }
            };
            fliters.Add(new SearchDto { Id = (int)SearchOn.IsExist, Name = "کالاهای موجود", SearchType = SearchType.Bool });
            fliters.Add(new SearchDto
            {
                Id = (int)SearchOn.Category,
                Name = "دسته بندی نتایج",
                SearchType = SearchType.Selection,
                Categories = categories.Message.ProductCategories.ProductCategories

            }) ;
            fliters.Add(new SearchDto { Id = (int)SearchOn.Price, Name = "قیمت", SearchType = SearchType.Number, MaxValue = 1000000, MinValue = 10000 });
            fliters.Add(new SearchDto
            {
                Id = (int)SearchOn.Brand,
                Name = "برند",
                SearchType = SearchType.Selection,
                SearchItems = brands.Message.ProductBrandsDto.Select((s, a) => new SearchItemDto { Id = a, Name = s.Name }).ToList()
            });
            fliters.Add(new SearchDto { Id = (int)SearchOn.DeliveredBySeller, Name = "امکان ارسال توسط فروشنده", SearchType = SearchType.Bool });
            fliters.Add(new SearchDto { Id = (int)SearchOn.HaveDiscount, Name = "تخفیف دار", SearchType = SearchType.Bool });
            await context.RespondAsync(new GetAllSearchFiltersQueryResponse(fliters, sorts));
        }

        //public async Task<List<SearchDto>> Handle(GetAllSearchFiltersQuery request, CancellationToken cancellationToken)
        //{
        //    var categories = await _mediator.Send(new GetAllProductCategoriesQuery());
        //    var brands = await _mediator.Send(new GetAllProductBrandsQuery());
        //    List<SearchDto> fliters = new List<SearchDto>();
        //    fliters.Add(new SearchDto { Id = 0, Name = "کالاهای موجود", SearchType = SearchType.Bool });
        //    fliters.Add(new SearchDto { Id = 1, Name = "دسته بندی نتایج", SearchType = SearchType.Selection, SearchItems = categories.Select((s, a) => new SearchItemDto { Id = a, Name = s.Name }).ToList() });
        //    fliters.Add(new SearchDto { Id = 2, Name = "قیمت", SearchType = SearchType.Number, MaxValue = 1000000, MinValue = 10000 });
        //    fliters.Add(new SearchDto { Id = 3, Name = "برند", SearchType = SearchType.Selection, SearchItems = brands.Select((s, a) => new SearchItemDto { Id = a, Name = s.Name }).ToList() });
        //    return fliters;
        //}
        public class GetAllSearchFiltersQueryResponse
        {
            public List<SearchDto> SearchesDto { get; set; }
            public List<SortDto> Sorts { get; set; }

            public GetAllSearchFiltersQueryResponse(List<SearchDto> searchesDto, List<SortDto> sorts)
            {
                SearchesDto = searchesDto;
                Sorts = sorts;
            }
        }
    }
}
