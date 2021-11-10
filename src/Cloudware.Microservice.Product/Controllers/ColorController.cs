using System.Threading.Tasks;
using ClosedXML;
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
    public class ColorController : ClwControllerBase
    {
        private readonly IMediator _mediator;

        public ColorController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<ActionResult> AddOne(AddColorDto addColorDto)
        {
            var req = _mediator.CreateRequestClient<AddColorCommand>();
            var res = await req.GetResponse<AddColorCommandResponse>(new AddColorCommand(TenantId, addColorDto));
            return Ok(res.Message);
            // await _mediator.Publish(new AddColorCommand(addColorDto));
        }
        [HttpGet]
        public async Task<ActionResult> GetMany(string? query="", int pageNumber = 1, int pageSize = 10)
        {
             query ??= "";
            var req = _mediator.CreateRequestClient<GetColorsQuery>();
            var res = await req.GetResponse<GetColorsQueryResponse>(new GetColorsQuery(TenantId,query ,pageSize, pageNumber));
            return Ok(res.Message);
        }
        [HttpPost]
        public async Task<ActionResult> GetTable(SearchDto searchDto)
        {
            var req = _mediator.CreateRequestClient<GetColorsTableQuery>();
            var res = await req.GetResponse<GetColorsTableQueryResponse>(new GetColorsTableQuery(TenantId, searchDto));
            return Ok(res.Message.TableData);
        }
        [HttpGet]
        public async Task<ActionResult> GetOne(string id)
        {
            var req = _mediator.CreateRequestClient<GetColorByIdQuery>();
            var res = await req.GetResponse<GetColorByIdQueryResponse>(new GetColorByIdQuery(TenantId, id));
            return Ok(res.Message);
        }
        [HttpPut]
        public async Task<ActionResult> UpdateOne(EditColorDto editColorDto)
        {
            await _mediator.Publish(new EditColorCommand(TenantId, editColorDto.Id, editColorDto.Name, editColorDto.Code));

            return Ok();
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteOne(string id)
        {
            await _mediator.Publish(new DeleteColorCommand(TenantId, id));

            return Ok();
        }
    }
}