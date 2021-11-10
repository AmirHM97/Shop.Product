using Cloudware.Microservice.Product.Application.Command;
using Cloudware.Microservice.Product.Application.Events.Test;
//using Cloudware.Microservice.Product.Application.Events.Order;
using Cloudware.Microservice.Product.Application.Query;
using Cloudware.Microservice.Product.Application.Query.HomePage;
using Cloudware.Microservice.Product.Application.Query.Product;
using Cloudware.Microservice.Product.Application.Query.Search_Filters;
using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Common.Exceptions;
using Cloudware.Utilities.Common.Response;
using Cloudware.Utilities.Configure.Microservice;
using Cloudware.Utilities.Contract.Product;
using Contracts;
using Elasticsearch.Net;
using MassTransit;
using MassTransit.Mediator;
//using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using Cloudware.Utilities.Table;
using System.Threading.Tasks;
using Test;
using static Cloudware.Microservice.Product.Application.Command.CreateProductCategoryCommandConsumer;
using static Cloudware.Microservice.Product.Application.Command.EditProductCommandConsumer;
using static Cloudware.Microservice.Product.Application.Command.ExportProductsToExcelCommandConsumer;
using static Cloudware.Microservice.Product.Application.Command.UpdateSellersCollectionCommandConsumer;
using static Cloudware.Microservice.Product.Application.Query.GetAllProductsByFiltersQueryConsumer;
using static Cloudware.Microservice.Product.Application.Query.Product.GetProductsForExcelQueryCounsumer;
using static Cloudware.Microservice.Product.Application.Query.Search_Filters.GetAllSearchFiltersQueryConsumer;
using static Cloudware.Microservice.Product.Application.Query.Search_Filters.GetSearchSuggestionsQueryConsumer;
using static Cloudware.Microservice.Product.Application.Query.Search_Filters.SearchProductsQueryConsumer;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;
using TableSearchDto = Cloudware.Utilities.Table.SearchDto;
using Cloudware.Utilities.Contract.Basket;
using Cloudware.Microservice.Product.Application.Query.Property;
using static Cloudware.Microservice.Product.Application.Query.Property.GetPropertyQueryConsumer;
using Cloudware.Microservice.Product.DTO.Category;
using Cloudware.Microservice.Product.Application.Query.Category;
using Cloudware.Microservice.Product.Application.Query.Stock;
using Cloudware.Microservice.Product.Application.Query.Product.Admin;
using Cloudware.Microservice.Product.Application.Command.Product;
using Microsoft.AspNetCore.Authorization;
using static Cloudware.Microservice.Product.Application.Query.Product.GetProductsForTorobQueryConsumer;
using Cloudware.Microservice.Product.Application.Command.Stock;
using Cloudware.Utilities.Common.Filter;
using Cloudware.Microservice.Product.Migrations;

namespace Cloudware.Microservice.Product.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ClwControllerBase
    {

        private readonly ILogger<ProductController> _logger;
        private readonly IRequestClient<GetProductByIdQuery> _getProductByIdQueryClient;
        private readonly IRequestClient<GetAllProductsByFiltersQuery> _getAllProductsByFiltersQuery;
        private readonly IRequestClient<GetHomePageDataQuery> _getHomePageDataQuery;
        private readonly IRequestClient<GetAllSearchFiltersQuery> _getAllSearchFiltersQuery;
        private readonly IRequestClient<SearchProductsQuery> _searchProductsQuery;
        private readonly IRequestClient<CreateProductCommand> _createProductCommand;
        private readonly IRequestClient<EditProductCommand> _editProductCommand;
        private readonly IRequestClient<CreateProductCategoryCommand> _createProductCategoryCommand;
        //private readonly IRequestClient<GetProductForBasketQuery> _getProductForBasketQuery;
        private readonly IRequestClient<GetSearchSuggestionsQuery> _getSearchSuggestionsQuery;
        private readonly IRequestClient<GetAllProductCategoriesQuery> _getAllProductCategoriesQuery;
        private readonly IRequestClient<TestEvent> Test;
        private readonly IRequestClient<faultTest> _faultTest;
        private readonly IRequestClient<UpdateSellersCollectionCommand> _updateSellersCollectionCommand;
        private readonly IProductReadDbContext _readDbContext;
        private readonly IMediator _mediator;


        private readonly IPublishEndpoint _publishEndpoint;

        public ProductController(
            ILogger<ProductController> logger
            , IRequestClient<GetProductByIdQuery> getProductByIdQueryClient
            , IRequestClient<GetAllProductsByFiltersQuery> getAllProductsByFiltersQuery
            , IRequestClient<GetHomePageDataQuery> getHomePageDataQuery
            , IRequestClient<GetAllSearchFiltersQuery> getAllSearchFiltersQuery
            , IRequestClient<SearchProductsQuery> searchProductsQuery
            , IRequestClient<CreateProductCommand> createProductCommand
            //, IRequestClient<GetProductForBasketQuery> getProductForBasketQuery
            , IRequestClient<EditProductCommand> editProductCommand, IPublishEndpoint publishEndpoint
            , IRequestClient<CreateProductCategoryCommand> createProductCategoryCommand
            , IRequestClient<TestEvent> test, IRequestClient<GetSearchSuggestionsQuery> getSearchSuggestionsQuery
            , IRequestClient<GetAllProductCategoriesQuery> getAllProductCategoriesQuery, IRequestClient<faultTest> faultTest, IRequestClient<UpdateSellersCollectionCommand> updateSellersCollectionCommand,
            IMediator mediator, IProductReadDbContext readDbContext)
        {

            _logger = logger;
            _getProductByIdQueryClient = getProductByIdQueryClient;
            _getAllProductsByFiltersQuery = getAllProductsByFiltersQuery;
            _getHomePageDataQuery = getHomePageDataQuery;
            _getAllSearchFiltersQuery = getAllSearchFiltersQuery;
            _searchProductsQuery = searchProductsQuery;
            _createProductCommand = createProductCommand;
            _editProductCommand = editProductCommand;
            _publishEndpoint = publishEndpoint;

            _createProductCategoryCommand = createProductCategoryCommand;
            //  _getProductForBasketQuery = getProductForBasketQuery;
            Test = test;
            _getSearchSuggestionsQuery = getSearchSuggestionsQuery;
            _getAllProductCategoriesQuery = getAllProductCategoriesQuery;
            _faultTest = faultTest;
            _updateSellersCollectionCommand = updateSellersCollectionCommand;
            _mediator = mediator;
            _readDbContext = readDbContext;
        }

        /// <summary>
        ///  Get A Product By ProductId
        /// </summary>
        /// <param name="productId">pass productId</param>
        /// <response code="204">Response Is Empty</response>
        /// <returns>ProductItem</returns>
        [HttpGet("{productId}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetProductById([FromRoute] int productId)
        {
            var requestId = HttpContext.TraceIdentifier;
            _logger.LogInformation($"product with {productId} Id is requested {requestId} ");

            var productItemDto = await _getProductByIdQueryClient.GetResponse<ProductItemDto>(new GetProductByIdQuery(productId, TenantId));

            if (productItemDto.Message.ProductData == null)
            {
                throw new AppException(400, HttpStatusCode.BadRequest, "Product NotFound!!");
                // return NoContent();
            }
            return Ok(productItemDto.Message);
        }

        /// <summary>
        ///  Get A Product By ProductId
        /// </summary>
        /// <param name="productId">pass productId</param>
        /// <response code="204">Response Is Empty</response>
        /// <returns>ProductItem</returns>
        [HttpGet("{productId}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetProductByIdForAdmin([FromRoute] string productId)
        {
            var requestId = HttpContext.TraceIdentifier;
            _logger.LogInformation($"product with {productId} Id is requested {requestId} ");
            var req = _mediator.CreateRequestClient<GetProductByIdForAdminQuery>();
            var productItemDto = await req.GetResponse<GetProductByIdForAdminQueryResponse>(new GetProductByIdForAdminQuery(productId, TenantId));

            return Ok(productItemDto.Message);
        }

        /// <summary>
        /// Get List Of Products By Given Filters
        /// </summary>
        /// <param name="categoryId">categoryId</param>
        /// <param name="brandId">brandId</param>
        /// <param name="sortBy">        0=Latest | 1=Special Offer | 2=Most Viewed | 3=Newest | 4=Hottest | 5=Most Expensive | 6=Least Expensive </param>
        ///   <response code="204">Response Is Empty</response>
        /// <returns>List Of ProductItems</returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ProductItemsListCatalogDto>>> GetProductsList(int? categoryId, int? brandId, SortBy? sortBy)
        {
            var prductItemCatalogList = await _getAllProductsByFiltersQuery.GetResponse<GetAllProductsByFiltersQueryResponse>(new GetAllProductsByFiltersQuery(categoryId, brandId, sortBy, TenantId));
            //List<ProductItemsListCatalogDto> prductItemCatalogList = await _getAllProductsByFiltersQuery.GetResponse<List<ProductItemsListCatalogDto>>(new GetAllProductsByFiltersQuery(categoryId, brandId, sortBy));
            if (prductItemCatalogList.Message.ProductItemsListCatalogsDto.Count == 0)
                return NoContent();
            return Ok(prductItemCatalogList.Message.ProductItemsListCatalogsDto);
        }

        /// <summary>
        ///  Home Page Data
        /// </summary>
        /// <response code="204">Response Is Empty</response>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<HomePageDto>> GetHomePageData()
        {
            var homePageDto = await _getHomePageDataQuery.GetResponse<HomePageDto>(new GetHomePageDataQuery(TenantId));
            if (homePageDto == null)
                return NoContent();
            return Ok(homePageDto.Message);
        }

        /// <summary>
        /// Get All Product Categories
        /// </summary>
        /// <response code="204">Response Is Empty</response>
        /// <returns>List Of All Categories</returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ProductCategoryReadDbCollection>>> GetAllCategories()
        {
            var productCategoriesList = await _getAllProductCategoriesQuery.GetResponse<GetAllProductCategoriesQueryResponse>(new GetAllProductCategoriesQuery(TenantId));

            return Ok(productCategoriesList.Message.ProductCategories);
        }

        /// <summary>
        /// Get Search Filters
        /// </summary>
        /// <remarks> Search Type :  0 = Boolean | 1 = Selection | 2 = Number</remarks>
        /// <response code="204">Response Is Empty</response>
        /// <returns>List Of All Search Filters</returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetSearchFilters()
        {
            var filtersList = await _getAllSearchFiltersQuery.GetResponse<GetAllSearchFiltersQueryResponse>(new GetAllSearchFiltersQuery(TenantId));
            if (filtersList.Message.SearchesDto.Count == 0)
                return NoContent();
            return Ok(filtersList.Message);
        }

        /// <summary>
        /// Advanced Search Api
        /// </summary>
        /// <param name="searches">0 = category | 1 = brand | 2 = Price | 3 = IsAvailable | 4 = Text | 5 = HaveDiscount | 6 = DeliveredBySeller</param>
        /// <param name="sortBy">0=Latest | 1=Special Offer | 2=Most Viewed | 3=Newest | 4=Hottest | 5=Most Expensive | 6=Least Expensive </param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <response code="204">Response Is Empty</response>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ProductItemsListCatalogDto>>> AdvancedSearch(List<AdvancedSearchItemDto>? searches, SortBy? sortBy, int pageNumber = 1, int pageSize = 10)
        {
            var productsList = await _searchProductsQuery.GetResponse<SearchProductsQueryResponse>(new SearchProductsQuery(searches, sortBy ?? SortBy.Newest, pageSize, pageNumber));
            if (productsList.Message.ProductItemsListCatalogsDto.Count == 0)
            {
                return NoContent();
            }
            return Ok(productsList.Message.ProductItemsListCatalogsDto);
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ProductItemsListCatalogDto>>> AdvancedSearchV2([FromQuery] AdvancedSearchItemDtoV2 searches, int pageSize = 20, int pageNumber = 1)
        {


            var productsList = await _searchProductsQuery.GetResponse<SearchProductsQueryResponse>(new SearchProductsQuery(searches, TenantId, pageSize, pageNumber));
            if (productsList.Message.ProductItemsListCatalogsDto.Count == 0)
            {
                return NoContent();
            }
            return Ok(productsList.Message.ProductItemsListCatalogsDto);
        }

        /// <summary>
        /// Creates A Product
        /// </summary>
        /// <param name="PropertyType">0 = Button | 1 = RadioButton | 2 = Color </param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<long>> AddProduct(CreateProductCommandDto dto)
        {
            // if (TenantId == null)
            // {
            //     return BadRequest("TenantId is Null !!!!");
            // }
            //if (dto.TechnicalProperties?.AddRecord != null)
            //{
            //        var te="364cbb18-b044-4339-b5c9-de772c2f011d";
            //        var us="35201";
            //    dto.TechnicalProperties.AddRecord.UserId = us;
            //    dto.TechnicalProperties.AddRecord.TenantId = te;
            //    dto.TechnicalProperties.AddRecord.ServiceId = te;
            //    // dto.TechnicalProperties.AddRecord.UserId = UserId;
            //    // dto.TechnicalProperties.AddRecord.TenantId = TenantId;
            //    // dto.TechnicalProperties.AddRecord.ServiceId = TenantId;
            //}
            //var result = await _createProductCommand.GetResponse<CreateProductCommandResponse>(new CreateProductCommand(dto, dto.UserId ?? UserId, TenantId));

            var result = await _createProductCommand.GetResponse<CreateProductCommandResponse>(new CreateProductCommand(dto, UserId , TenantId));
            if (result.Message.ProductId == 0)
            {
                return BadRequest();
            }
            return Ok(new { id = result.Message.ProductId, guid = result.Message.Guid });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<long>> EditProduct(EditProductCommandDto edit)
        {
            var id = await _editProductCommand.GetResponse<EditProductCommandResponse>(new EditProductCommand(edit, UserId, TenantId));
            if (id.Message.ProductId == 0)
            {
                throw new AppException(400, HttpStatusCode.BadRequest);
                //return BadRequest();
            }
            return Ok(id.Message.ProductId.ToString());
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteProduct(string guid)
        {
            await _mediator.Publish(new DeleteProductCommand(TenantId, guid));
            return Ok();
        }

        // [HttpPost]
        // public async Task<ActionResult> CreateProductCategory(CreateCategoryDto createCategoryDto)
        // {
        //     if (string.IsNullOrEmpty(createCategoryDto.TenantId))
        //     {
        //         createCategoryDto.TenantId = TenantId;
        //     }
        //     var id = await _createProductCategoryCommand.GetResponse<CreateProductCategoryCommandResponse>(new CreateProductCategoryCommand(createCategoryDto.Name, createCategoryDto.ParentId, createCategoryDto.TenantId, createCategoryDto.Icon, createCategoryDto.Description));
        //     if (id.Message.CategoryId == 0)
        //     {
        //         // throw new AppException(204, HttpStatusCode.NoContent);
        //         return NoContent();
        //     }
        //     return Ok(new ApiResult(200, 200, HttpContext.TraceIdentifier, id.Message.CategoryId.ToString()));
        // }
        /// <summary>
        /// Get Search Suggestions
        /// </summary>
        /// <param name="getSearchSuggestionsQuery">search Query</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GetSearchSuggestionsQueryResponse>> GetSearchSuggestions(string getSearchSuggestionsQuery, int pageNumber = 1, int pageSize = 7)
        {
            if (string.IsNullOrEmpty(getSearchSuggestionsQuery))
            {
                getSearchSuggestionsQuery = "";
            }
            var getSearchSuggestionsQueryResponse = await _getSearchSuggestionsQuery.GetResponse<GetSearchSuggestionsQueryResponse>(new GetSearchSuggestionsQuery(getSearchSuggestionsQuery, TenantId, pageSize, pageNumber));
            // if (getSearchSuggestionsQueryResponse.Message.Products.Count == 0 && getSearchSuggestionsQueryResponse.Message.Categories.Count == 0)
            // {
            //     return NoContent();
            // }
            return Ok(getSearchSuggestionsQueryResponse.Message);
        }

        [HttpPost]
        public async Task<ActionResult> GetProductTable(TableSearchDto searchDto)
        {
            var req = _mediator.CreateRequestClient<GetProductTableQuery>();
            var res = await req.GetResponse<GetProductTableQueryResponse>(new GetProductTableQuery(TenantId, false, searchDto));
            return Ok(res.Message.TableData);
        }

        [HttpPost]
        public async Task<ActionResult> GetAdvancedProductTable(TableSearchDto searchDto)
        {
            var req = _mediator.CreateRequestClient<GetAdvancedProductTableQuery>();
            var res = await req.GetResponse<GetAdvancedProductTableQueryResponse>(new GetAdvancedProductTableQuery(TenantId, searchDto));
            return Ok(res.Message.TableData);
        }

        [HttpGet]
        public async Task<ActionResult> SearchProduct(string query, bool? isAvailable, int pageNumber = 1, int pageSize = 10)
        {

            if (string.IsNullOrEmpty(query))
            {
                query = "";
            }
            var req = _mediator.CreateRequestClient<SearchProductsWithStockQuery>();
            var res = await req.GetResponse<SearchProductsWithStockQueryResponse>(new SearchProductsWithStockQuery(query, isAvailable, TenantId,UserId, pageSize, pageNumber));
            return Ok(res.Message.Products);
        }

        [HttpGet]
        public async Task<ActionResult> SearchMyProduct(string query, bool? isAvailable, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(query))
            {
                query = "";
            }
            var req = _mediator.CreateRequestClient<SearchProductsWithStockQuery>();
            var res = await req.GetResponse<SearchProductsWithStockQueryResponse>(new SearchProductsWithStockQuery(query, isAvailable, TenantId,UserId, pageSize, pageNumber));
            return Ok(res.Message.Products);
        }

        [HttpGet]
        public async Task<ActionResult> GetProductPrice(long productId)
        {
            var productItemDto = await _getProductByIdQueryClient.GetResponse<ProductItemDto>(new GetProductByIdQuery(productId, TenantId));

            return Ok(productItemDto.Message.ProductData.StockItemsDto.Select(s => s.Price).FirstOrDefault());
        }

        [NonAction]
        [HttpGet]
        public async Task<ActionResult> UpdateSellersCollection()
        {
            var res = await _updateSellersCollectionCommand.GetResponse<UpdateSellersCollectionCommandResponse>(new UpdateSellersCollectionCommand());
            if (res.Message.Result == 0)
            {
                throw new AppException();
            }
            return Ok(new ApiResult(200, (int)HttpStatusCode.OK, HttpContext.TraceIdentifier, "Read DataBase Updated Successfully!!!"));
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult> ExportToXlsxProducts()
        {
            var productsQueryReq = _mediator.CreateRequestClient<GetProductsForExcelQuery>();
            var productsQueryRes = await productsQueryReq.GetResponse<GetAllProductsQueryResponse>(new GetProductsForExcelQuery { });

            var exportProductsToExcelCommandReq = _mediator.CreateRequestClient<ExportProductsToExcelCommand>();
            var exportProductsToExcelCommandRes = await exportProductsToExcelCommandReq.GetResponse<ExportProductsToExcelCommandResponse>(new ExportProductsToExcelCommand { Products = productsQueryRes.Message.ProductItemsListCatalogsDto });

            return File(exportProductsToExcelCommandRes.Message.Content, exportProductsToExcelCommandRes.Message.ContentType, "لیست ن Vip.xlsx");
        }

        [HttpGet]
        public async Task<ActionResult> GetProductCategoryFormWitRecords(long productId)
        {
            var req = _mediator.CreateRequestClient<GetProductCategoryFormByProductIdQuery>();
            var res = await req.GetResponse<GetProductCategoryFormByProductIdQueryResponse>(new GetProductCategoryFormByProductIdQuery(TenantId, productId));
            return Ok(res.Message);
        }

        [HttpGet]
        public async Task<ActionResult> GetProperties(PropertyType propertyType, int pageSize = 10, int pageNumber = 1)
        {
            var req = _mediator.CreateRequestClient<GetPropertyQuery>();
            var res = await req.GetResponse<GetPropertyQueryResponse>(new GetPropertyQuery(TenantId,propertyType, pageNumber, pageSize));
            return Ok(res.Message);
        }

        [HttpPost]
        public async Task<ActionResult> GetProductStockTable(GetProductStockTableDto getProductStockTableDto)
        {
            var req = _mediator.CreateRequestClient<GetProductStocksTableQuery>();
            var res = await req.GetResponse<GetProductStocksTableQueryResponse>(new GetProductStocksTableQuery(getProductStockTableDto.SearchDto, getProductStockTableDto.ProductId, TenantId));
            return Ok(res.Message);
        }

        [HttpPut]
        public async Task<ActionResult> EditStock(EditStockDto editStock)
        {
           await _mediator.Publish(new EditStockCommand(TenantId,editStock.Guid,editStock.StocksItems));
            return Ok();
        }

        [SkipImportantTaskAttribute]
        [HttpPost]
        [Route("GetProductsForTorob2/products")]
        public async Task<ActionResult> GetProductsForTorob2([FromForm]GetProductsForTorobDto getProductsForTorobDto)
        {
            var req = _mediator.CreateRequestClient<GetProductsForTorobQuery>();
            var products = await req.GetResponse<GetProductsForTorobQueryResponse>(new GetProductsForTorobQuery(getProductsForTorobDto, TenantId));
            var ProductsForTorobDto = new ProductsForTorob2Dto
            {
                Count = products.Message.ProductsCount.ToString(),
                Max_Pages = products.Message.TotalPage.ToString(),
                Products = products.Message.Products
            };
            return Ok(ProductsForTorobDto);
        }

        [SkipImportantTaskAttribute]
        [HttpGet]
        [Route("GetProductsForTorob1/products")]
        public async Task<ActionResult> GetProductsForTorob1(int page=1, int? pageSize=100)
        {
            var req = _mediator.CreateRequestClient<GetProductsForTorobQuery>();
            var products = await req.GetResponse<GetProductsForTorobQueryResponse>(new GetProductsForTorobQuery(TenantId,page,pageSize));
            var res = new ProductsForTorob1Dto();
            var p = products.Message.Products;
            res.productFortorob = p.Select(s => new productFortorob
            {
                Availability = s.Availability,
                Old_Price = s.Old_Price,
                Page_Url = s.Page_Url,
                Price = s.Current_Price,
                Product_Id = s.Page_Unique
            }).ToList();


            return Ok(res.productFortorob);
        }
    }
}


#region comment
/// <summary>
/// Get All Product Brands
/// </summary>
/// <response code="204">Response Is Empty</response>
/// <returns>List Of All Brands</returns>
//[HttpGet]
//[Produces("application/json")]
//[ProducesResponseType(StatusCodes.Status204NoContent)]
//[ProducesResponseType(StatusCodes.Status200OK)]
//public async Task<ActionResult<List<ProductCategoryDto>>> GetAllBrands()
//{
//    var productBrandsList = await _mediator.Send(new GetAllProductBrandsQuery());
//    if (productBrandsList == null)
//        return NoContent();
//    return Ok(productBrandsList);
//}
//[HttpGet]
//public async Task<ActionResult> FaultTest()
//{
//    var res = await _faultTest.GetResponse<faultTestresponse>(new faultTest());
//    return Ok();
//}
//[HttpGet]

//public async Task<ActionResult> UpdateWholeReadDb()
//{
//    await _mediator.Send(new UpdateWholeReadDbCommand());
//    return Ok();
//}
//[HttpPost]
//public async Task<ActionResult> Test(OrderCreatedEvent @event)
//{
//    await _mediator.Publish(@event);
//    return Ok();
//}

//[HttpPost]
////public async Task<ActionResult> IntegrationTest(BasketCompletedEvent command)
//public async Task<ActionResult> IntegrationTest()
//{

//    //await _publishEndpoint.Publish(new BasketCompletedEvent());
//    return Ok();
//}

//[HttpPost]
//public async Task<ActionResult> test()
//{
//    var a = await _getProductForBasketQuery.GetResponse<GetProductForBasketQueryResponse>(new GetProductForBasketQuery() {GetProductForBasketQueryItems=new List<GetProductForBasketQueryItem>() });
//    //await _publishEndpoint.Publish(new BasketCompletedEvent());
//    return Ok();
//} 
/// <summary>
/// Get All Product Ids
/// </summary>
/// <response code="204">Response Is Empty</response>
/// <returns>List Of ProductItem Ids</returns>
//[HttpGet]
//[Produces("application/json")]
//[ProducesResponseType(StatusCodes.Status204NoContent)]
//[ProducesResponseType(StatusCodes.Status200OK)]
//public async Task<ActionResult<List<int>>> GetAllProducts()
//{
//    List<long> prductItemIdsList = await _mediator.Send(new GetAllProductIdsQuery());
//    if (prductItemIdsList == null)
//        return NoContent();
//    return Ok(prductItemIdsList);
//}
#endregion
