using Cloudware.Microservice.Product.Application.Events.Product;
using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
//using Contracts;
using MassTransit;
using MassTransit.Courier.Contracts;
using MassTransit.Mediator;
using Microsoft.Extensions.DependencyInjection;
//using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog.Sinks.Elasticsearch.Durable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query
{
    public class GetProductByIdQueryConsumer : IConsumer<GetProductByIdQuery>, IMediatorConsumerType
    {
        private readonly IUnitOfWork _uow;
        // private readonly DbSet<ProductItem> _productItems;
        private readonly IProductReadDbContext _readDbContext;
        private readonly IMongoCollection<ProductItemReadDbCollection> _collection;
        private readonly ILogger<GetProductByIdQueryConsumer> _logger;
        private readonly IMediator _mediator;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public GetProductByIdQueryConsumer(IServiceScopeFactory serviceScopeFactory, IUnitOfWork uow, ILogger<GetProductByIdQueryConsumer> logger, IMediator mediator, IPublishEndpoint publishEndpoint, IProductReadDbContext readDbContext)
        {
            _uow = uow;
            _logger = logger;
            _mediator = mediator;
            _publishEndpoint = publishEndpoint;
            _serviceScopeFactory = serviceScopeFactory;
            _readDbContext = readDbContext;
            _collection = _readDbContext.ProductItemsDataCollection;
        }

        public async Task Consume(ConsumeContext<GetProductByIdQuery> context)
        {

            //  await context.Publish(new ProductRequestedEvent(context.Message.ProductItemId));
            var productItemDto = await _collection.Find(f => f.ProductId == context.Message.ProductItemId && f.TenantId == context.Message.TenantId && !f.IsDeleted.Value && f.IsAvailable).FirstOrDefaultAsync();
            // var productItemDto = await _collection.Find(f => f.ProductId == context.Message.ProductItemId&&f.IsAvailable).FirstOrDefaultAsync();
            if (productItemDto != null)
            {
                await _mediator.Publish(new ProductRequestedEvent(context.Message.ProductItemId));

                var relatedProducts = await _collection.Find(c => c.CategoryId == productItemDto.CategoryId && c.IsAvailable)
                     .Project(s => new ProductItemsListCatalogDto
                     {
                         Id = s.ProductId,
                         image = s.ImageUrl,
                         Name = s.Name,
                         Discount = s.StockItemsDto.Select(p => p.Discount).FirstOrDefault(),
                         Price = s.StockItemsDto.Select(p => p.Price).FirstOrDefault()
                     }).Limit(10).ToListAsync();

                //log info
                await context.RespondAsync(new ProductItemDto(productItemDto, relatedProducts));
            }
            await context.RespondAsync(new ProductItemDto());
        }
    }
}
#region comment
#region comment
// var productItemDto = await _readDbContext.ProductItemsDataCollection.Find(filter => filter.ProductId.Equals(request.ProductItemId)).FirstOrDefaultAsync();
//var productItemDto =   collection.FirstOrDefault(f => f.ProductId == request.ProductItemId);


//var related = await _readDbContext.ProductItemsDataCollection.Find(filter => filter.CategoryId.Equals(productItemDto.CategoryId)).ToListAsync();
//var relatedProducts = related.Select(s => new ProductItemsListCatalogDto
//{
//    Id = s.ProductId,
//    image = s.ImageUrl,
//    Name = s.Name,
//    Price = s.StockItemsDto.OrderBy(o => o.Price).Select(p => p.Price).FirstOrDefault()
//}).ToList(); 
#endregion
#region comment
#region sqlQuery
//var productItemDto = _productItems.Where(prodId => prodId.Id == request.ProductItemId)
//      .Select(s => new ProductItemDto
//      {
//          Id = s.Id,
//          Name = s.Name,
//          Description = s.Description,
//          Story = s.Story,
//          UserId = s.UserId,
//          ImageUrl = s.Image,
//          Weight = s.Weight,
//          CreatedDate = s.CreatedDate,
//          LastUpdatedDate = s.LastUpdatedDate,
//          Dimensions = s.Dimensions,
//          BrandId = s.BrandId,
//          BrandName = s.Brand.Name,
//          ViewCount = s.ViewCount,
//          CategoryId = s.CategoryId,
//          CategoryName = s.Category.Name,
//          ImageUrls = s.ImageUrls.Select(img => img.Url).ToList(),
//          StockItemsDto = s.StocksItems.Select(g =>
//          new StockItemDto
//          {
//              Count = g.Count,
//              Price = g.Price,
//              PropItems = g.StockPropertyItems.Select(y => y.PropItemId).ToList()
//          }).ToList(),
//          PropertiesDto = s.PropertyItems.Select(ps =>
//            new PropertyDto
//            {
//                Name = ps.Name,
//                PropertyItemDtos = ps.PropItems.Select(spit =>
//                new PropertyItemDto
//                {
//                    Id = spit.Id,
//                    Name = spit.Name,
//                    Value = spit.Value

//var related = await _readDbContext.ProductItemsDataCollection.Find(filter => filter.CategoryId.Equals(productItemDto.CategoryId)).ToListAsync();
//var relatedProducts = related.Select(s => new ProductItemsListCatalogDto
//{
//    Id = s.ProductId,
//    image = s.ImageUrl,
//    Name = s.Name,
//    Price = s.StockItemsDto.OrderBy(o => o.Price).Select(p => p.Price).FirstOrDefault()
//}).ToList(); 
#endregion
//var relatedProducts = await _collection.Find(c => c.CategoryId == productItemDto.CategoryId)
//     .Project(s => new ProductItemsListCatalogDto
//     {
//         Id = s.ProductId,
//         image = s.ImageUrl,
//         Name = s.Name,
//         Discount = s.StockItemsDto.Select(p => p.Discount).FirstOrDefault(),
//         Price = s.StockItemsDto.Select(p => p.Price).FirstOrDefault()
//     }).ToListAsync();
#region sqlQuery
//var productItemDto = _productItems.Where(prodId => prodId.Id == request.ProductItemId)
//      .Select(s => new ProductItemDto
//      {
//          Id = s.Id,
//          Name = s.Name,
//          Description = s.Description,
//          Story = s.Story,
//          UserId = s.UserId,
//          ImageUrl = s.Image,
//          Weight = s.Weight,
//          CreatedDate = s.CreatedDate,
//          LastUpdatedDate = s.LastUpdatedDate,
//          Dimensions = s.Dimensions,
//          BrandId = s.BrandId,
//          BrandName = s.Brand.Name,
//          ViewCount = s.ViewCount,
//          CategoryId = s.CategoryId,
//          CategoryName = s.Category.Name,
//          ImageUrls = s.ImageUrls.Select(img => img.Url).ToList(),
//          StockItemsDto = s.StocksItems.Select(g =>
//          new StockItemDto
//          {
//              Count = g.Count,
//              Price = g.Price,
//              PropItems = g.StockPropertyItems.Select(y => y.PropItemId).ToList()
//          }).ToList(),
//          PropertiesDto = s.PropertyItems.Select(ps =>
//            new PropertyDto
//            {
//                Name = ps.Name,
//                PropertyItemDtos = ps.PropItems.Select(spit =>
//                new PropertyItemDto
//                {
//                    Id = spit.Id,
//                    Name = spit.Name,
//                    Value = spit.Value


//                }).ToList()
//            }).ToList(),
//          RelatedProducts = _productItems.Where(w => w.CategoryId == s.CategoryId).Where(w => w.Id != s.Id)
//          .Select(rps => new ProductItemsListCatalogDto
//          {
//              Id = rps.Id,
//              image = rps.Image,
//              Name = rps.Name,
//              Price = rps.StocksItems.OrderBy(o => o.Price).Select(p => p.Price).FirstOrDefault()
//          }).ToList(),
//          TechnicalProperties = s.TechnicalProperties.Select(tp => new TechnicalPropertyDto { Name = tp.Name, Value = tp.Value }).ToList()

//      }); 
#endregion
#endregion
//public async Task<ProductItemDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
//{
//    var productItemDto = await (_collection.Find(f => f.ProductId == request.ProductItemId).FirstOrDefaultAsync());
//    #region comment
//    // var productItemDto = await _readDbContext.ProductItemsDataCollection.Find(filter => filter.ProductId.Equals(request.ProductItemId)).FirstOrDefaultAsync();
//    //var productItemDto =   collection.FirstOrDefault(f => f.ProductId == request.ProductItemId);

//    //var related = await _readDbContext.ProductItemsDataCollection.Find(filter => filter.CategoryId.Equals(productItemDto.CategoryId)).ToListAsync();
//    //var relatedProducts = related.Select(s => new ProductItemsListCatalogDto
//    //{
//    //    Id = s.ProductId,
//    //    image = s.ImageUrl,
//    //    Name = s.Name,
//    //    Price = s.StockItemsDto.OrderBy(o => o.Price).Select(p => p.Price).FirstOrDefault()
//    //}).ToList(); 
//    #endregion
//    var relatedProducts = await _collection.Find(c => c.CategoryId == productItemDto.CategoryId)
//         .Project(s => new ProductItemsListCatalogDto
//         {
//             Id = s.ProductId,
//             image = s.ImageUrl,
//             Name = s.Name,
//             Discount = s.StockItemsDto.Select(p => p.Discount).FirstOrDefault(),
//             Price = s.StockItemsDto.Select(p => p.Price).FirstOrDefault()
//         }).ToListAsync();
//    #region sqlQuery
//    //var productItemDto = _productItems.Where(prodId => prodId.Id == request.ProductItemId)
//    //      .Select(s => new ProductItemDto
//    //      {
//    //          Id = s.Id,
//    //          Name = s.Name,
//    //          Description = s.Description,
//    //          Story = s.Story,
//    //          UserId = s.UserId,
//    //          ImageUrl = s.Image,
//    //          Weight = s.Weight,
//    //          CreatedDate = s.CreatedDate,
//    //          LastUpdatedDate = s.LastUpdatedDate,
//    //          Dimensions = s.Dimensions,
//    //          BrandId = s.BrandId,
//    //          BrandName = s.Brand.Name,
//    //          ViewCount = s.ViewCount,
//    //          CategoryId = s.CategoryId,
//    //          CategoryName = s.Category.Name,
//    //          ImageUrls = s.ImageUrls.Select(img => img.Url).ToList(),
//    //          StockItemsDto = s.StocksItems.Select(g =>
//    //          new StockItemDto
//    //          {
//    //              Count = g.Count,
//    //              Price = g.Price,
//    //              PropItems = g.StockPropertyItems.Select(y => y.PropItemId).ToList()
//    //          }).ToList(),
//    //          PropertiesDto = s.PropertyItems.Select(ps =>
//    //            new PropertyDto
//    //            {
//    //                Name = ps.Name,
//    //                PropertyItemDtos = ps.PropItems.Select(spit =>
//    //                new PropertyItemDto
//    //                {
//    //                    Id = spit.Id,
//    //                    Name = spit.Name,
//    //                    Value = spit.Value

//    //                }).ToList()
//    //            }).ToList(),
//    //          RelatedProducts = _productItems.Where(w => w.CategoryId == s.CategoryId).Where(w => w.Id != s.Id)
//    //          .Select(rps => new ProductItemsListCatalogDto
//    //          {
//    //              Id = rps.Id,
//    //              image = rps.Image,
//    //              Name = rps.Name,
//    //              Price = rps.StocksItems.OrderBy(o => o.Price).Select(p => p.Price).FirstOrDefault()
//    //          }).ToList(),
//    //          TechnicalProperties = s.TechnicalProperties.Select(tp => new TechnicalPropertyDto { Name = tp.Name, Value = tp.Value }).ToList()

//    //      }); 
//    #endregion
//    //log info
//    return MapCollectionModelToDto(productItemDto, relatedProducts);
//}
//private ProductItemDto MapCollectionModelToDto(ProductItemReadDbCollection model, List<ProductItemsListCatalogDto> relatedProducts)
//{
//    ProductItemDto productItemDto = new ProductItemDto
//    {
//        BrandId = model.BrandId,
//        BrandName = model.BrandName,
//        CategoryId = model.CategoryId,
//        CategoryName = model.CategoryName,
//        CreatedDate = model.CreatedDate,
//        Description = model.Description,
//        Dimensions = model.Dimensions,
//        Id = model.ProductId,
//        ImageUrl = model.ImageUrl,
//        ImageUrls = model.ImageUrls,
//        LastUpdatedDate = model.LastUpdatedDate,
//        Name = model.Name,
//        IsAvailable = model.IsAvailable,
//        PropertiesDto = model.PropertiesDto.Select(s => new PropertyDto
//        {
//            Name = s.Name,
//            PropertyItemDtos = s.PropertyItemDtos
//             .Select(pi => new PropertyItemDto { Id = pi.Id, Name = pi.Name, Value = pi.Value }).ToList()
//        }).ToList(),
//        StockItemsDto = model.StockItemsDto.Select(s => new StockItemDto { Id = s.StockId, Count = s.Count, Discount = s.Discount, Price = s.Price, PropItems = s.PropItems }).ToList(),
//        Story = model.Story,
//        TechnicalProperties = model.TechnicalPropertiesReadDb.Select(s => new TechnicalPropertyDto { Name = s.Name, Value = s.Value }).ToList(),
//        UserId = model.UserId,
//        Weight = model.Weight,
//        ViewCount = model.ViewCount,
//        RelatedProducts = relatedProducts
//    };
//    return productItemDto;
//}
#endregion
#region test
//.Select(s => new ProductItemDto
// {
//     Id = s.Id,
//     Name = s.Name,
//     Description = s.Description,
//     Story = s.Story,
//     UserId = s.UserId,
//     ImageUrl = s.Image,
//     Weight = s.Weight,
//     CreatedDate = s.CreatedDate,
//     LastUpdatedDate = s.LastUpdatedDate,
//     Dimensions = s.Dimensions,
//     BrandId = s.BrandId,
//     BrandName = s.Brand.Name,
//     ViewCount = s.ViewCount,
//     CategoryId = s.CategoryId,
//     CategoryName = s.Category.Name,
//     ImageUrls = s.ImageUrls.Select(img => img.Url).ToList(),
//     StockItemsDto = s.StocksItems.Select(g =>
//     new StockItemDto
//     {
//         Count = g.Count,
//         Price = g.Price,
//         PropItems = g.StockPropertyItems.Select(y => y.PropItemId).ToList()
//     }).ToList(),
//     PropertiesDto = s.PropertyItems.Select(ps =>
//       new PropertyDto
//       {
//           Name = ps.Name,
//           PropertyItemDtos = ps.PropItems.Select(spit =>
//           new PropertyItemDto
//           {
//               Id = spit.Id,
//               Name = spit.Name,
//               Value = spit.Value

//           }).ToList()
//       }).ToList(),
//     RelatedProducts = _productItems.Where(w => w.CategoryId == s.CategoryId).Where(w => w.Id != s.Id)
//                   .Select(rps => new ProductItemsListCatalogDto
//                   {
//                       Id = rps.Id,
//                       image = rps.Image,
//                       Name = rps.Name,
//                       Price = rps.StocksItems.OrderBy(o => o.Price).Select(p => p.Price).FirstOrDefault()
//                   }).ToList()
// });
#endregion