using System.Threading.Tasks;
using Cloudware.Microservice.Product.Application.Events;
using Cloudware.Microservice.Product.Infrastructure.Services;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Formbuilder.Services;
using MassTransit;
using MassTransit.Mediator;

namespace Cloudware.Microservice.Product.Application.Command.Category
{
    public class CreateCategoryFormCommandConsumer : IConsumer<CreateCategoryFormCommand>, IMediatorConsumerType
    {
        private readonly IFormManagementService _formManagementService;
        private readonly ICategoryService _categoryService;
        private readonly IMediator _mediator;

        public CreateCategoryFormCommandConsumer(IFormManagementService formManagementService, ICategoryService categoryService, IMediator mediator)
        {
            _formManagementService = formManagementService;
            _categoryService = categoryService;
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<CreateCategoryFormCommand> context)
        {
            
            var formId = await _formManagementService.CreateForm(context.Message.AddCategoryFormDto.CreateFormDto);
            await _categoryService.UpdateFormIdAsync(context.Message.AddCategoryFormDto.CategoryId, formId);
            await _mediator.Publish(new ProductCategoryCreatedEvent(context.Message.AddCategoryFormDto.CreateFormDto.tenantId));
        }
    }
}