using System.Threading.Tasks;
using Cloudware.Microservice.Product.Application.Command.Property;
using Cloudware.Microservice.Product.Application.Query.Property;
using Cloudware.Microservice.Product.DTO.Property;
using Cloudware.Utilities.Configure.Microservice;
using Cloudware.Utilities.Table;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Cloudware.Microservice.Product.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GuaranteeController : ClwControllerBase
    {
        private readonly IMediator _mediator;

        public GuaranteeController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<ActionResult> AddOne(AddGuaranteeDto addGuaranteeDto)
        {
            // await _mediator.Publish(new AddGuaranteeCommand(addGuaranteeDto));
             var req=_mediator.CreateRequestClient<AddGuaranteeCommand>();
            var res= await req.GetResponse<AddGuaranteeCommandResponse>(new AddGuaranteeCommand(TenantId,addGuaranteeDto));
            return Ok(res.Message);
        }
         [HttpPost]
        public async Task<ActionResult> GetTable(SearchDto searchDto)
        {
            var req = _mediator.CreateRequestClient<GetGuaranteesTableQuery>();
            var res = await req.GetResponse<Application.Query.Property.GetGuaranteesTableQueryResponse>(new GetGuaranteesTableQuery(TenantId,searchDto));
            return Ok(res.Message.TableData);
        }
        [HttpGet]
        public async Task<ActionResult> GetMany(string? query="",int pageNumber = 1, int pageSize = 10)
        {
             query ??= "";
            var req = _mediator.CreateRequestClient<GetGuaranteesQuery>();
            var res = await req.GetResponse<GetGuaranteesQueryResponse>(new GetGuaranteesQuery(TenantId,query,pageSize, pageNumber));
            return Ok(res.Message);
        }
        [HttpGet]
        public async Task<ActionResult> GetOne(string id)
        {
            var req = _mediator.CreateRequestClient<GetGuaranteeByIdQuery>();
            var res = await req.GetResponse<GetGuaranteeByIdQueryResponse>(new GetGuaranteeByIdQuery(TenantId,id));
            return Ok(res.Message);
        }
        [HttpPut]
        public async Task<ActionResult> UpdateOne(EditGuaranteeDto editGuaranteeDto)
        {
            await _mediator.Publish(new EditGuaranteeCommand(TenantId,editGuaranteeDto.Id,editGuaranteeDto.Name,editGuaranteeDto.FrontImage,editGuaranteeDto.BackImage,editGuaranteeDto.Duration));
            return Ok();
        }
         [HttpDelete]
        public async Task<ActionResult> DeleteOne(string id)
        {
            await _mediator.Publish(new DeleteGuaranteeCommand(TenantId, id));

            return Ok();
        }
    }
}