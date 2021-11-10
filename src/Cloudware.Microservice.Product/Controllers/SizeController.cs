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
    public class SizeController : ClwControllerBase
    {
        private readonly IMediator _mediator;

        public SizeController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<ActionResult> AddSize(AddSizeDto addSizeDto)
        {
            // await _mediator.Publish(new AddSizeCommand(addSizeDto));
            var req = _mediator.CreateRequestClient<AddSizeCommand>();
            var res = await req.GetResponse<AddSizeCommandResponse>(new AddSizeCommand(TenantId, addSizeDto));
            return Ok(res.Message);
        }
        [HttpPost]
        public async Task<ActionResult> GetTable(SearchDto searchDto)
        {
            var req = _mediator.CreateRequestClient<GetSizesTableQuery>();
            var res = await req.GetResponse<GetSizesTableQueryResponse>(new GetSizesTableQuery(TenantId, searchDto));
            return Ok(res.Message.TableData);
        }
        [HttpGet]
        public async Task<ActionResult> GetMany(string? query="",int pageNumber = 1, int pageSize = 10)
        {
             query ??= "";
            var req = _mediator.CreateRequestClient<GetSizesQuery>();
            var res = await req.GetResponse<GetSizesQueryResponse>(new GetSizesQuery(TenantId, query,pageSize, pageNumber));
            return Ok(res.Message);
        }
        [HttpGet]
        public async Task<ActionResult> GetOne(string id)
        {
            var req = _mediator.CreateRequestClient<GetSizeByIdQuery>();
            var res = await req.GetResponse<GetSizeByIdQueryResponse>(new GetSizeByIdQuery(TenantId, id));
            return Ok(res.Message);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateOne(EditSizeDto editSizeDto)
        {
            await _mediator.Publish(new EditSizeCommand(TenantId, editSizeDto.Id, editSizeDto.Size));
            return Ok();
        }
         [HttpDelete]
        public async Task<ActionResult> DeleteOne(string id)
        {
            await _mediator.Publish(new DeleteSizeCommand(TenantId, id));
            return Ok();
        }
    }
}