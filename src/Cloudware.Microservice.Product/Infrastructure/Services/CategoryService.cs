using System;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Cloudware.Microservice.Product.Infrastructure.Services
{
    public interface ICategoryService
    {
        Task<string> GetCategoryFormId(long categoryId, string tenantId);
        Task<ProductCategory> GetOneAsync(long categoryId, string tenantId);
        Task UpdateFormIdAsync(long categoryId, string formId);
        Task UpdateRecordIdAsync(long categoryId, string recordId);
    }

    public class CategoryService : ICategoryService
    {
        private readonly DbSet<ProductCategory> _productCategories;
        private readonly IUnitOfWork _uow;
        private readonly IProductReadDbContext _productReadDbContext;

        public CategoryService(IUnitOfWork uow, IProductReadDbContext productReadDbContext)
        {
            _uow = uow;
            _productCategories = _uow.Set<ProductCategory>();
            _productReadDbContext = productReadDbContext;
        }
        public async Task UpdateFormIdAsync(long categoryId, string formId)
        {
            var category = await _productCategories.FirstOrDefaultAsync(f => f.Id == categoryId);
            category.FormId = formId;
            category.LastUpdatedDate = DateTimeOffset.UtcNow;
            _productCategories.Update(category);
            await _uow.SaveChangesAsync();
        }
        public async Task UpdateRecordIdAsync(long categoryId, string recordId)
        {
            var category = await _productCategories.FirstOrDefaultAsync(f => f.Id == categoryId);
            category.RecordId = recordId;
            category.LastUpdatedDate = DateTimeOffset.UtcNow;
            _productCategories.Update(category);
            await _uow.SaveChangesAsync();
        }
        public async Task<string> GetCategoryFormId(long categoryId, string tenantId)
        {
            var category = await _productCategories.FirstOrDefaultAsync(f => f.Id == categoryId && f.TenantId == tenantId);
            if (category is null)
            {
                throw new AppException(5056, System.Net.HttpStatusCode.BadRequest, "item not found!!!");
            }
            return category.FormId;
        }
        public async Task<ProductCategory> GetOneAsync(long categoryId, string tenantId)
        {
            var category = await _productCategories.FirstOrDefaultAsync(f => f.Id == categoryId && f.TenantId == tenantId);

            return category;
        }
        public async Task DeleteOne(string TenantId, long Id)
        {
            //var category=await
        }

    }
}