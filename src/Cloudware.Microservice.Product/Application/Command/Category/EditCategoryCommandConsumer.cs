using System.Threading.Tasks;
using Cloudware.Microservice.Product.Application.Events;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Common.Exceptions;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Application.Command.Category
{
    public class EditCategoryCommandConsumer : IConsumer<EditCategoryCommand>, IMediatorConsumerType
    {
        private readonly DbSet<ProductCategory> _categories;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public EditCategoryCommandConsumer(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _categories = _unitOfWork.Set<ProductCategory>();
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<EditCategoryCommand> context)
        {
            var category = await _categories.FirstOrDefaultAsync(f => f.Guid.ToString() == context.Message.EditCategoryDto.Guid.ToLower() && f.TenantId == context.Message.TenantId);
            var parent= await _categories.FirstOrDefaultAsync(f => f.Id == context.Message.EditCategoryDto.ParentId && f.TenantId == context.Message.TenantId);
            if (category is null)
            {
                throw new AppException(5056,System.Net.HttpStatusCode.BadRequest,"Category not found!");
            }
            category.Icon = context.Message.EditCategoryDto.Icon ?? category.Icon;
            category.Name = context.Message.EditCategoryDto.Name ?? category.Name;
            category.Parent=parent;
            category.Description = context.Message.EditCategoryDto.Description ?? category.Description;
            _categories.Update(category);
            await _unitOfWork.SaveChangesAsync();
            await _mediator.Publish(new ProductCategoryCreatedEvent(context.Message.TenantId));
        }
    }
}