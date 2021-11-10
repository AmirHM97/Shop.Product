using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure.Services;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Formbuilder.Entities;
using Cloudware.Utilities.Formbuilder.Services;
using MassTransit;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public class GetCategoryFormByCategoryIdQueryConsumer : IConsumer<GetCategoryFormByCategoryIdQuery>, IMediatorConsumerType
    {
        private readonly IRecordsService _recordsService;
        private readonly IFormManagementService _formManagementService;
        private readonly ICategoryService _categoryService;

        public GetCategoryFormByCategoryIdQueryConsumer(ICategoryService categoryService, IFormManagementService formManagementService)
        {
            _categoryService = categoryService;
            _formManagementService = formManagementService;
        }

        public async Task Consume(ConsumeContext<GetCategoryFormByCategoryIdQuery> context)
        {
            //  await _formManagementService.GetForm
            var formId = await _categoryService.GetCategoryFormId(context.Message.CategoryId, context.Message.TenantId);
            var form = await _formManagementService.GetForm(formId);
            await context.RespondAsync(new GetCategoryFormByCategoryIdQueryResponse(form));
        }
    }
    public class GetCategoryFormByCategoryIdQueryResponse
    {
        public FormsCollection Form { get; set; }

        public GetCategoryFormByCategoryIdQueryResponse(FormsCollection form)
        {
            Form = form;
        }
    }
}