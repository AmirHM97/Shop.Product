using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Cloudware.Utilities.Contract.Abstractions;

namespace Cloudware.Microservice.Product.Application.Query
{
    public class GetAllProductCategoriesQueryConsumer : IConsumer<GetAllProductCategoriesQuery>, IMediatorConsumerType
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ProductCategory> _productCategory;
        private readonly IProductReadDbContext _readContext;

        public GetAllProductCategoriesQueryConsumer(IUnitOfWork uow, IProductReadDbContext readContext)
        {
            _uow = uow;
            _productCategory = _uow.Set<ProductCategory>();
            _readContext = readContext;
        }

        public async Task Consume(ConsumeContext<GetAllProductCategoriesQuery> context)
        {
            try
            {
                var collection = _readContext.ProductCategoriesDataCollection;
                var categories = await collection.Find(f => f.ClientId ==context.Message.TenantId).FirstOrDefaultAsync();
                await context.RespondAsync(new GetAllProductCategoriesQueryResponse(categories));
            }
            catch (Exception)
            {

                await context.RespondAsync(new GetAllProductCategoriesQueryResponse(new ProductCategoryReadDbCollection()));

            }
        }

        //public async Task<List<ProductCategoryReadDbCollection>> Handle(GetAllProductCategoriesQuery request, CancellationToken cancellationToken)
        //{
        //    var collection = _readContext.ProductCategoriesDataCollection;
        //    var test = await _productCategory.ToListAsync();
        //    //var json = JsonConvert.SerializeObject(test);
        //    //var a = _productCategory
        //    //    .Join(_productCategory, pc => pc.Id, cc => cc.ParentId, (pc, cc) => new { pc, cc })
        //    //    .Select(s => new tempcategory {Id=s.pc.Id,Name=s.pc.Name,Children=s.pc.Children.Select(c=>new tempcategory) }).ToListAsync();

        //    var productCategoryDtoList = new List<ProductCategoryDto>();
        //    foreach (var item in test.Where(f => !f.ParentId.HasValue).ToList())
        //    {
        //        productCategoryDtoList.Add(new ProductCategoryDto
        //        {
        //            Id = item.Id,
        //            Name = item.Name,
        //            Children = GetChildren(test, item.Id)
        //        });
        //    }

        //    //var readDbobjList = new List<ProductCategoryReadDbCollection>();
        //    //foreach (var item in test.Where(f => !f.ParentId.HasValue).ToList())
        //    //{
        //    //    readDbobjList.Add(new ProductCategoryReadDbCollection { CategoryId = item.Id, Name = item.Name, Children = GetChildrenForReadDb(test, item.Id) });
        //    //}
        //    //await collection.InsertManyAsync(readDbobjList);

        //    //  await collection.Find(_ => true).Project(p => new ProductCategoryDto {Id=p.CategoryId,Name=p.Name,ParentId=p.ParentId,Children = MapCollectionToDto(p.Children,p.CategoryId)}).ToListAsync();
        //    return await collection.Find(_ => true).ToListAsync();
        //    //await _productCategory.Select(s => new ProductCategoryDto { Id = s.Id, Name = s.Name, ParentId = s.ParentId }).ToListAsync();
        //}
        //public List<ProductCategoryDto> GetChildren(List<ProductCategory> listItem, long parentId)
        //{
        //    return listItem
        //            .Where(c => c.ParentId == parentId)
        //            .Select(c => new ProductCategoryDto
        //            {
        //                Id = c.Id,
        //                Name = c.Name,
        //                ParentId = c.ParentId,
        //                Children = GetChildren(listItem, c.Id)
        //            })
        //            .ToList();
        //}
        //private List<ProductCategoryReadDbCollection> GetChildrenForReadDb(List<ProductCategory> listItem, long parentId)
        //{
        //    return listItem
        //            .Where(c => c.ParentId == parentId)
        //            .Select(c => new ProductCategoryReadDbCollection
        //            {
        //                CategoryId = c.Id,
        //                Name = c.Name,
        //                ParentId = c.ParentId,
        //                Children = GetChildrenForReadDb(listItem, c.Id)
        //            })
        //            .ToList();
        //}
        //private List<ProductCategoryDto> MapCollectionToDto(List<ProductCategoryReadDbCollection> listItem, long parentId)
        //{

        //   var a=listItem.Where(c => c.ParentId == parentId).FirstOrDefault();
        //    return listItem
        //            .Where(c => c.ParentId == parentId)
        //            .Select(c => new ProductCategoryDto
        //            { 
        //                Id = c.CategoryId,
        //                Name = c.Name,
        //                ParentId = c.ParentId,
        //                Children = MapCollectionToDto(listItem, c.CategoryId)
        //            })
        //            .ToList();
        //}

    }
    //public class GetAllProductCategoriesQueryResponse
    //{
    //    public List<ProductCategoryReadDbCollection> ProductCategories { get; set; }

    //    public GetAllProductCategoriesQueryResponse(List<ProductCategoryReadDbCollection> productCategories)
    //    {
    //        ProductCategories = productCategories;
    //    }
    //}
    public class GetAllProductCategoriesQueryResponse
    {
        public ProductCategoryReadDbCollection ProductCategories { get; set; }

        public GetAllProductCategoriesQueryResponse(ProductCategoryReadDbCollection productCategories)
        {
            ProductCategories = productCategories;
        }
    }
}
