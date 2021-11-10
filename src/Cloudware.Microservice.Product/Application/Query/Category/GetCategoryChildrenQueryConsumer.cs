using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Infrastructure.Services;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public class GetCategoryChildrenQueryConsumer : IConsumer<GetCategoryChildrenQuery>, IMediatorConsumerType
    {
        private readonly DbSet<ProductCategory> _categories;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaginationService _paginationService;


        public GetCategoryChildrenQueryConsumer(IUnitOfWork unitOfWork, IPaginationService paginationService)
        {
            _unitOfWork = unitOfWork;
            _categories = _unitOfWork.Set<ProductCategory>();
            _paginationService = paginationService;
        }

        public async Task Consume(ConsumeContext<GetCategoryChildrenQuery> context)
        {
            int skip = (context.Message.PageNumber - 1) * context.Message.PageSize;
            IQueryable<ProductCategory> query;
            if (context.Message.CategoryId == 0)
                query = _categories.Where(w => w.ParentId == null && !w.IsDeleted && w.TenantId == context.Message.TenantId);
            else
                query = _categories.Where(w => w.ParentId == context.Message.CategoryId && !w.IsDeleted && w.TenantId == context.Message.TenantId);
            var res = query.Where(w => w.Name.Contains(context.Message.Query)).Select(s => new GetCategoryChildrenQueryResponseItem
            {
                CategoryId = s.Id,
                Name = s.Name,
                Guid = s.Guid.ToString()
            });
            _paginationService.Pagination(res, context.Message.PageSize);

            await context.RespondAsync(new GetCategoryChildrenQueryResponse(await res.Skip(skip).Take(context.Message.PageSize).ToListAsync()));
        }
    }
    public class GetCategoryChildrenQueryResponse
    {
        public List<GetCategoryChildrenQueryResponseItem> Categories { get; set; }

        public GetCategoryChildrenQueryResponse(List<GetCategoryChildrenQueryResponseItem> categories)
        {
            Categories = categories;
        }
    }
    public class GetCategoryChildrenQueryResponseItem
    {
        public long CategoryId { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
    }
}