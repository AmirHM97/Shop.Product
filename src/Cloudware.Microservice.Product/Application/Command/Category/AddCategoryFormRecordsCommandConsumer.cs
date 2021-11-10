using System.Threading.Tasks;
using Cloudware.Microservice.Product.Application.Events;
using Cloudware.Microservice.Product.Infrastructure.Services;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Formbuilder.Services;
using MassTransit;
using MassTransit.Mediator;

namespace Cloudware.Microservice.Product.Application.Command.Category
{
    public class AddCategoryFormRecordsCommandConsumer : IConsumer<AddCategoryFormRecordsCommand>, IMediatorConsumerType
    {
        private readonly IRecordsService _recordsService;
        private readonly ICategoryService _categoryService;
        private readonly IMediator _mediator;

        public AddCategoryFormRecordsCommandConsumer(ICategoryService categoryService, IRecordsService recordsService, IMediator mediator)
        {
            _categoryService = categoryService;
            _recordsService = recordsService;
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<AddCategoryFormRecordsCommand> context)
        {
            var recordId = await _recordsService.AddRecord(context.Message.AddCategoryFormRecordDto.AddRecordDto);
            await _categoryService.UpdateRecordIdAsync(context.Message.AddCategoryFormRecordDto.CategoryId, recordId);
            await _mediator.Publish(new ProductCategoryCreatedEvent(context.Message.AddCategoryFormRecordDto.AddRecordDto.TenantId));
        }
    }
}