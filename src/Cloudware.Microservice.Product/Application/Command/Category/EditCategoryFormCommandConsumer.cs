using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure.Services;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Formbuilder.Services;
using MassTransit;

namespace Cloudware.Microservice.Product.Application.Command.Category
{
    public class EditCategoryFormCommandConsumer : IConsumer<EditCategoryFormCommand>, IMediatorConsumerType
    {
        private readonly IFormManagementService _formManagementService;
        private readonly ICategoryService _categoryService;

        public EditCategoryFormCommandConsumer(IFormManagementService formManagementService, ICategoryService categoryService)
        {
            _formManagementService = formManagementService;
            _categoryService = categoryService;
        }

        public async Task Consume(ConsumeContext<EditCategoryFormCommand> context)
        {
            context.Message.EditCategoryFormDto.EditFormDto.Id=await _categoryService.GetCategoryFormId(context.Message.EditCategoryFormDto.CategoryId,context.Message.TenantId);
           await _formManagementService.EditForm(context.Message.EditCategoryFormDto.EditFormDto);
        }
    }
}