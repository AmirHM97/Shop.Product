using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Command
{
    public class UpdateWholeReadDbCommandHandler : IConsumer<UpdateWholeReadDbCommand>,IMediatorConsumerType
    {
        private readonly IProductReadDbContext _context;
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ProductItem> _productItems;


        public UpdateWholeReadDbCommandHandler(IUnitOfWork uow, IProductReadDbContext context)
        {

            _uow = uow;
            _productItems = _uow.Set<ProductItem>();

            _uow = uow;
            _context = context;
        }

        public async Task Consume(ConsumeContext<UpdateWholeReadDbCommand> context)
        {
        //     var productItemDto = await _productItems
        //           .Select(s => new ProductItemReadDbCollection
        //           {
        //               ProductId = s.Id,
        //               Name = s.Name,
        //               Description = s.Description,
        //               Story = s.Story,
        //               UserId = s.UserId,
        //               ImageUrl = s.Image,
        //               Code = s.Code,
        //               Weight = s.Weight,
        //               CreatedDate = s.CreatedDate,
        //               LastUpdatedDate = s.LastUpdatedDate,
        //               Dimensions = s.Dimensions,
        //               BrandId = s.BrandId,
        //               BrandName = s.Brand.Name,
        //               MaxPrice = s.StocksItems.OrderByDescending(od => od.Price).Select(sp => sp.Price).FirstOrDefault(),
        //               MinPrice = s.StocksItems.OrderBy(oa => oa.Price).Select(sp => sp.Price).FirstOrDefault(),
        //               CategoryId = s.CategoryId,
        //               UserDescription="",
        //               UserImageUrl="",
        //               UserName="",
        //               CategoryName = s.Category.Name,
        //               ImageUrls = s.ImageUrls.Select(img => img.Url).ToList(),
        //               TechnicalPropertiesReadDb = s.TechnicalProperties.Select(tp => new TechnicalPropertyReadDb { Name = tp.Name, Value = tp.Value }).ToList(),
        //               IsAvailable = !s.StocksItems.Any(a => a.Count == 0),
        //               StockItemsDto = s.StocksItems.Select(g =>
        //               new StockItemReadDb
        //               {
        //                   StockId = g.Id,
        //                   Count = g.Count,
        //                   Price = g.Price,
        //                   Discount = g.Discount,
        //                   PropItems = g.StockPropertyItems.Select(y => y.PropItemId).ToList()
        //               }).ToList(),
        //               PropertiesDto = s.PropertyItems.Select(ps =>
        //                 new PropertyReadDb
        //                 {
        //                     PropertyId = ps.Id,
        //                     Name = ps.Name,
        //                     PropertyType=0,
        //                     PropertyItemDtos = ps.PropItems.Select(spit =>
        //                     new PropertyItemReadDb
        //                     {
        //                         Id = spit.Id,
        //                         Name = spit.Name,
        //                         Value = spit.Value

        //                     }).ToList()
        //                 }).ToList()
        //           }).ToListAsync();

        //     await _context.ProductItemsDataCollection.InsertManyAsync(productItemDto);
        // }

        // public async Task<Unit> Handle(UpdateWholeReadDbCommand request, CancellationToken cancellationToken)
        // {

        //     var productItemDto = await _productItems
        //            .Select(s => new ProductItemReadDbCollection
        //            {
        //                ProductId = s.Id,
        //                Name = s.Name,
        //                Description = s.Description,
        //                Story = s.Story,
        //                UserId = s.UserId,
        //                ImageUrl = s.Image,
        //                Code = s.Code,
        //                Weight = s.Weight,
        //                CreatedDate = s.CreatedDate,
        //                LastUpdatedDate = s.LastUpdatedDate,
        //                Dimensions = s.Dimensions,
        //                BrandId = s.BrandId,
        //                BrandName = s.Brand.Name,
        //                MaxPrice = s.StocksItems.OrderByDescending(od => od.Price).Select(sp => sp.Price).FirstOrDefault(),
        //                MinPrice = s.StocksItems.OrderBy(oa => oa.Price).Select(sp => sp.Price).FirstOrDefault(),
        //                CategoryId = s.CategoryId,
        //                CategoryName = s.Category.Name,
                      
        //                ImageUrls = s.ImageUrls.Select(img => img.Url).ToList(),
        //                TechnicalPropertiesReadDb = s.TechnicalProperties.Select(tp => new TechnicalPropertyReadDb { Name = tp.Name, Value = tp.Value }).ToList(),
        //                IsAvailable = !s.StocksItems.Any(a => a.Count == 0),
        //                StockItemsDto = s.StocksItems.Select(g =>
        //                new StockItemReadDb
        //                {
        //                    StockId = g.Id,
        //                    Count = g.Count,
        //                    Price = g.Price,
        //                    Discount = g.Discount,
        //                    PropItems = g.StockPropertyItems.Select(y => y.PropItemId).ToList()
        //                }).ToList(),
        //                PropertiesDto = s.PropertyItems.Select(ps =>
        //                  new PropertyReadDb
        //                  {
        //                      PropertyId = ps.Id,
        //                      Name = ps.Name,
        //                      PropertyItemDtos = ps.PropItems.Select(spit =>
        //                      new PropertyItemReadDb
        //                      {
        //                          Id = spit.Id,
        //                          Name = spit.Name,
        //                          Value = spit.Value

        //                      }).ToList()
        //                  }).ToList()
        //            }).ToListAsync();

        //     await _context.ProductItemsDataCollection.InsertManyAsync(productItemDto);
        //     return Unit.Value;
        }
    }
}
