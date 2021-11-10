using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Application.Command;
using Cloudware.Microservice.Product.Application.Command.Category;
using Cloudware.Microservice.Product.Application.Events.Product;
using Cloudware.Microservice.Product.Application.Query;
using Cloudware.Microservice.Product.Application.Query.Category;
using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.DTO.Category;
using Cloudware.Microservice.Product.DTO.Property;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Configure.Microservice;
using Cloudware.Utilities.Formbuilder.Dtos;
using Cloudware.Utilities.Formbuilder.Services;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Cloudware.Microservice.Product.Application.Command.CreateProductCategoryCommandConsumer;

namespace Cloudware.Microservice.Product.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class CategoryController : ClwControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllCategories()
        {
            var req = _mediator.CreateRequestClient<GetAllProductCategoriesQuery>();
            var productCategoriesList = await req.GetResponse<GetAllProductCategoriesQueryResponse>(new GetAllProductCategoriesQuery(TenantId));

            return Ok(productCategoriesList.Message.ProductCategories);
        }
        [HttpGet]
        public async Task<ActionResult> GetOne(long id)
        {

            var req = _mediator.CreateRequestClient<GetCategoryByIdQuery>();
            var res = await req.GetResponse<GetCategoryByIdQueryResponse>(new GetCategoryByIdQuery(id, TenantId));
            return Ok(res.Message);
        }
        [HttpGet]
        public async Task<ActionResult> GetCategoryForm(long categoryId)
        {
            var req = _mediator.CreateRequestClient<GetCategoryFormByCategoryIdQuery>();
            var res = await req.GetResponse<GetCategoryFormByCategoryIdQueryResponse>(new GetCategoryFormByCategoryIdQuery(categoryId, TenantId));
            return Ok(res.Message);
        }
        [HttpGet]
        public async Task<ActionResult> GetCategoryFormWitRecords(long categoryId)
        {
            var req = _mediator.CreateRequestClient<GetCategoryFormWithRecordsQuery>();
            var res = await req.GetResponse<GetCategoryFormWithRecordsQueryResponse>(new GetCategoryFormWithRecordsQuery(TenantId, categoryId,null));
            return Ok(res.Message.Form);
        }
        [HttpPost]
        public async Task<ActionResult> CreateProductCategory(CreateCategoryDto createCategoryDto)
        {
            if (string.IsNullOrEmpty(createCategoryDto.TenantId))
            {
                createCategoryDto.TenantId = TenantId;
            }
            var req = _mediator.CreateRequestClient<CreateProductCategoryCommand>();
            var id = await req.GetResponse<CreateProductCategoryCommandResponse>(new CreateProductCategoryCommand(createCategoryDto.Name, createCategoryDto.ParentId, createCategoryDto.TenantId, createCategoryDto.Icon, createCategoryDto.Description));
            if (id.Message.CategoryId == 0)
            {
                // throw new AppException(204, HttpStatusCode.NoContent);
                return NoContent();
            }
            return Ok(new {Id= id.Message.CategoryId.ToString(),Guid=id.Message.Guid.ToString()} );
        }

        [HttpGet]
        public async Task<ActionResult> GetCategoryChildren(long? categoryId = 0,string? query="" ,int pageSize = 10, int pageNumber = 1)
        {
            query??="";
            var req = _mediator.CreateRequestClient<GetCategoryChildrenQuery>();
            var res = await req.GetResponse<GetCategoryChildrenQueryResponse>(new GetCategoryChildrenQuery(TenantId, categoryId ?? 0,query,pageSize, pageNumber));
            return Ok(res.Message);
        }
        [HttpPut]
        public async Task<ActionResult> EditCategory(EditCategoryDto editCategoryDto)
        {
            await _mediator.Publish(new EditCategoryCommand(editCategoryDto, TenantId));
            return Ok();

        }
        [HttpDelete]
        public async Task<ActionResult> DeleteCategory(string  guid)
        {
            await _mediator.Publish(new DeleteCategoryCommand(TenantId, guid));
            return Ok();

        }
        [HttpPost]
        public async Task<ActionResult> AddCategoryForm(AddCategoryFormDto addCategoryFormDto)
        {
            addCategoryFormDto.CreateFormDto.UserId = UserId;
            addCategoryFormDto.CreateFormDto.tenantId = TenantId;
            addCategoryFormDto.CreateFormDto.ServiceId = TenantId;
            await _mediator.Publish(new CreateCategoryFormCommand(addCategoryFormDto));
            return Ok();
        }
          [HttpPut]
        public async Task<ActionResult> EditCategoryForm(EditCategoryFormDto editCategoryDto)
        {
            await _mediator.Publish(new EditCategoryFormCommand(TenantId,editCategoryDto));
            return Ok();
        }
        [HttpPost]
        public async Task<ActionResult> AddCategoryFormRecord(AddCategoryFormRecordDto addCategoryFormRecordDto)
        {
            addCategoryFormRecordDto.AddRecordDto.UserId = UserId;
            addCategoryFormRecordDto.AddRecordDto.TenantId = TenantId;
            addCategoryFormRecordDto.AddRecordDto.ServiceId = TenantId;
            await _mediator.Publish(new AddCategoryFormRecordsCommand(addCategoryFormRecordDto));
            return Ok();
        }
        [HttpPost]
        public async Task<ActionResult> AddPropertyCategory(AddPropertyCategoryDto addPropertyCategoryDto)
        {
            await _mediator.Publish(new AddPropertyCategoryCommand(addPropertyCategoryDto, TenantId));
            return Ok();
        }
        [HttpGet]
        public async Task<ActionResult> GetCategoryList(string query = "", int pageSize = 10, int pageNumber = 1)
        {

            query ??= "";


            var req = _mediator.CreateRequestClient<GetCategoryListQuery>();
            var res = await req.GetResponse<GetCategoryListQueryResponse>(new GetCategoryListQuery(query, pageSize, pageNumber, TenantId));

            return Ok(res.Message);
        }
        [HttpGet]
        public async Task<ActionResult> GetCustomCategory()
        {
             var req = _mediator.CreateRequestClient<GetCustomCategoryQuery>();
            var res = await req.GetResponse<GetCustomCategoryQueryResponse>(new GetCustomCategoryQuery(TenantId));

            return Ok(res.Message.Categories);
        }
         [HttpGet]
        public async Task<ActionResult> GetCategoryProperties(long categoryId)
        {
            var req = _mediator.CreateRequestClient<GetCategoryPropertiesQuery>();
            var res = await req.GetResponse<GetCategoryPropertiesQueryResponse>(new GetCategoryPropertiesQuery(categoryId,TenantId));

            return Ok(res.Message);
        }
         [HttpGet]
        public async Task<ActionResult> GetParents(long categoryId)
        {
            var req = _mediator.CreateRequestClient<GetCategoryParentsQuery>();
            var res = await req.GetResponse<GetCategoryParentsQueryResponse>(new GetCategoryParentsQuery(TenantId,categoryId));

            return Ok(res.Message);
        }
         [HttpPost]
        public async Task<ActionResult> GetCategoryPropertiesTable(GetCategoryPropertiesTableDto getCategoryPropertiesTableDto)
        {
            var req = _mediator.CreateRequestClient<GetCategoryPropertiesTableQuery>();
            var res = await req.GetResponse<GetCategoryPropertiesTableQueryResponse>(new GetCategoryPropertiesTableQuery(getCategoryPropertiesTableDto.SearchDto,TenantId,getCategoryPropertiesTableDto.CategoryId));
            return Ok(res.Message);
        }
        // [HttpGet]
        // public async Task<ActionResult> Test(string? tenantId)
        // {

        //     var a = _mediator.Publish(new ProductRequestedEvent(1));
        //     await Task.WhenAll(a);
        //     return Ok();
        // }
    }
}