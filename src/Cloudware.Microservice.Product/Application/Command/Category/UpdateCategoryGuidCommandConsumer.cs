using System;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Application.Events;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Application.Command.Category
{
    public class UpdateCategoryGuidCommandConsumer : IConsumer<UpdateCategoryGuidCommand>, IMediatorConsumerType
    {
        private readonly DbSet<ProductCategory> _productCategories;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public UpdateCategoryGuidCommandConsumer(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _productCategories = _unitOfWork.Set<ProductCategory>();
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<UpdateCategoryGuidCommand> context)
        {
            var categories = await _productCategories.Where(w => w.Guid == Guid.Empty).ToListAsync();
            categories = categories.Select(s => { s.Guid = Guid.NewGuid(); return s; }).ToList();
            _productCategories.UpdateRange(categories);
            await _unitOfWork.SaveChangesAsync();
            var tenants = categories.GroupBy(g => g.TenantId).Select(s => s.Key).ToList();
            foreach (var item in tenants)
            {
                await _mediator.Publish(new ProductCategoryCreatedEvent(item));
            }
        }
    }
}