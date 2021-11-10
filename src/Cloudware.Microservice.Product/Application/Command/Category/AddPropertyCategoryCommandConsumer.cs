using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Common.Exceptions;
using Cloudware.Utilities.Contract.Abstractions;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Cloudware.Microservice.Product.Application.Command.Category
{
    public class AddPropertyCategoryCommandConsumer : IConsumer<AddPropertyCategoryCommand>, IMediatorConsumerType
    {

        private readonly DbSet<PropertyCategory> _propertyCategories;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductReadDbContext _productReadDbContext;

        public AddPropertyCategoryCommandConsumer(IUnitOfWork unitOfWork, IProductReadDbContext productReadDbContext)
        {
            _unitOfWork = unitOfWork;
            _propertyCategories = _unitOfWork.Set<PropertyCategory>();
            _productReadDbContext = productReadDbContext;
        }

        public async Task Consume(ConsumeContext<AddPropertyCategoryCommand> context)
        {
            var cat = await _productReadDbContext.ProductCategoryNormalizedCollection.Find(f => f.CategoryId == context.Message.AddCategoryPropertyDto.CategoryId).FirstOrDefaultAsync();
            if (cat is not null)
            {
                var catProps=await _propertyCategories.Where(w=>w.CategoryId==cat.CategoryId).ToListAsync();
                _propertyCategories.RemoveRange(catProps);
                var propertyCategories = new List<PropertyCategory>();
                foreach (var item in context.Message.AddCategoryPropertyDto.PropertyType.Distinct().ToList())
                {
                    propertyCategories.Add(new PropertyCategory
                    {
                        CategoryId = context.Message.AddCategoryPropertyDto.CategoryId,
                        PropertyType = item,
                        CreatedDate = DateTimeOffset.UtcNow,
                        LastUpdatedDate = DateTimeOffset.UtcNow,
                        TenantId = cat.TenantId
                    });
                }
                await _propertyCategories.AddRangeAsync(propertyCategories);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                throw new AppException(5056, System.Net.HttpStatusCode.BadRequest, "category not found");
            }
        }
    }
}