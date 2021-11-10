using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Infrastructure.Extensions;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Basket;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public class GetCategoryPropertiesQueryConsumer : IConsumer<GetCategoryPropertiesQuery>, IMediatorConsumerType
    {
        private readonly DbSet<PropertyCategory> _propertyCategories;
        private readonly IUnitOfWork _uow;

        public GetCategoryPropertiesQueryConsumer(IUnitOfWork uow)
        {
            _uow = uow;
            _propertyCategories = _uow.Set<PropertyCategory>();
        }

        public async Task Consume(ConsumeContext<GetCategoryPropertiesQuery> context)
        {
            var res = await _propertyCategories.Where(w => w.CategoryId == context.Message.CategoryId && w.TenantId == context.Message.TenantId).ToListAsync();
            await context.RespondAsync(new GetCategoryPropertiesQueryResponse(context.Message.CategoryId, res.Select(s => new GetCategoryPropertiesQueryResponseItem
            {
                PropertyType=s.PropertyType,
                Title= ProductExtensions.GetPropertyName(s.PropertyType)
            }).ToList()));
        }
    }
    public class GetCategoryPropertiesQueryResponse
    {
        public GetCategoryPropertiesQueryResponse(long categoryId, List<GetCategoryPropertiesQueryResponseItem> propertyTypes)
        {
            CategoryId = categoryId;
            PropertyTypes = propertyTypes;
        }

        public long CategoryId { get; set; }
        public List<GetCategoryPropertiesQueryResponseItem> PropertyTypes { get; set; }
    }
    public class GetCategoryPropertiesQueryResponseItem
    {
        public PropertyType PropertyType { get; set; }
        public string Title { get; set; }

    }
}