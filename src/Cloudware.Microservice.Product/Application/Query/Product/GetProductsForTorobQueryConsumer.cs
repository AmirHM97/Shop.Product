using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Infrastructure.Extensions;
using Cloudware.Microservice.Product.Infrastructure.Services;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Basket;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query.Product
{
    public class GetProductsForTorobQueryConsumer : IConsumer<GetProductsForTorobQuery>, IMediatorConsumerType
    {
        private readonly IProductReadDbContext _context;
        private readonly IPaginationService _paginationService;
        private readonly ILogger<GetProductsForTorobQueryConsumer> _logger;
        public GetProductsForTorobQueryConsumer(IProductReadDbContext context,
             IPaginationService paginationService,
             ILogger<GetProductsForTorobQueryConsumer> logger)
        {
            _context = context;
            _paginationService = paginationService;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<GetProductsForTorobQuery> context)
        {
            var collection = _context.ProductItemsDataCollection;
            List<IFindFluent<ProductItemReadDbCollection, ProductItemReadDbCollection>> filters = new();
            List<IFindFluent<ProductItemReadDbCollection, ProductItemReadDbCollection>> sorts = new();

            filters.Add(collection.Find(c => c.TenantId == context.Message.TenantId));
            filters.Add(collection.Find(c => c.IsAvailable));
            //var pagesize = context.Message.PageSize;
            //var pageNumber = context.Message.Page;
            var pagesize = 100;
            var pageNumber = 1;
            if (context.Message.GetProductsForTorobDto != null)
            {
                pagesize = context.Message.GetProductsForTorobDto.PageSize.Value;
                pageNumber = context.Message.GetProductsForTorobDto.Page;
             
                if (context.Message.GetProductsForTorobDto.Page_Unique != null)
                {
                    var id = Convert.ToInt64(context.Message.GetProductsForTorobDto.Page_Unique);
                    filters.Add(collection.Find(c => c.ProductId == id && c.IsAvailable));
                    //filters.Add(collection.Find(c => context.Message.GetProductsForTorobDto.Page_Unique.Contains(c.Id)));

                }
                else if (context.Message.GetProductsForTorobDto.Page_Url != null)
                {
                    var id = context.Message.GetProductsForTorobDto.Page_Url.Split('/').Last();
                    List<string> strList = id.Split('?').ToList();
                    id = strList.First();
                    var longId = Convert.ToInt64(id);
                    filters.Add(collection.Find(c => c.ProductId == longId && c.IsAvailable));
                    //filters.Add(collection.Find(c => id.Contains(c.Id)));
                }
            }
            sorts.Add(ProductExtensions.GetSortFilter(SortBy.Newest, collection));
            try
            {
                var productsList = collection.Find(filters.Select(g => g.Filter)
                       .Aggregate((a, b) => { return a & b; }))
                       .Project(s => new Cloudware.Microservice.Product.DTO.Products
                       {
                           Current_Price = s.StockItemsDto.Select(p => p.Price).FirstOrDefault().ToString(),
                           Short_Dsc = s.Description,
                           Page_Url = "https://shop.kookbaz.ir/product/" + s.Id.ToString(),
                           Image_Link = s.ImageUrl,
                           Old_Price = "",
                           Page_Unique = s.ProductId.ToString(),
                           Title = s.Name,
                           Availability = s.TotalCount == 0 ? "0" : "instock",
                           Category_Name = s.CategoryName,
                           Registry = "",
                           Guarantee = string.Join(",", s.PropertiesDto.Where(w => w.PropertyType == PropertyType.Guarantee)
                                                      .SelectMany(s => s.PropertyItemDtos).Select(e => e.Name).ToList()),
                           Spec = new Spec
                           {
                               Color = string.Join(",", s.PropertiesDto.Where(w => w.PropertyType == PropertyType.Color)
                                                      .SelectMany(s => s.PropertyItemDtos).Select(e => e.Name).ToList())
                           }

                       }).Sort(sorts.Select(g => g.Options).FirstOrDefault().Sort);
                var productsCount = await productsList.CountDocumentsAsync();
                var totalPage = productsList.CountDocuments() / 100;
                int skip = (pageNumber - 1) * pagesize;
                var productListWithPagination = await productsList.Skip(skip).Limit(pagesize).ToListAsync();

                await context.RespondAsync(new GetProductsForTorobQueryResponse(productListWithPagination, productsCount, totalPage));
            }
            catch (Exception e)
            {
                _logger.LogError("Search Products Failed With Error : " + e.Message);
            }
        }

        public class GetProductsForTorobQueryResponse
        {
            public GetProductsForTorobQueryResponse(List<Products> products, long productsCount, long totalPage)
            {
                Products = products;
                ProductsCount = productsCount;
                TotalPage = totalPage;
            }

            public List<Cloudware.Microservice.Product.DTO.Products> Products { get; set; }
            public long ProductsCount { get; set; }
            public long TotalPage { get; set; }
        }
    }
}
