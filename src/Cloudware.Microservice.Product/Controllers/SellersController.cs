using Cloudware.Microservice.Product.Application.Query.Sellers;
using Cloudware.Microservice.Product.Infrastructure.Filter;
using Cloudware.Utilities.Configure.Microservice;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Cloudware.Microservice.Product.Application.Query.Sellers.GetSellersQueryConsumer;

namespace Cloudware.Microservice.Product.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiResultFilterAttributeV2]
    public class SellersController : ClwControllerBase 
    {
        private readonly ILogger<SellersController> _logger;
        private readonly IRequestClient<GetSellersQuery> _getSellersQuery;

        public SellersController(IRequestClient<GetSellersQuery> getSellersQuery, ILogger<SellersController> logger)
        {
            _getSellersQuery = getSellersQuery;
            _logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult<GetSellersQueryResponse>> GetSellers(int pageNumber = 1, int pageSize = 10)
        {
            var sellers = await _getSellersQuery.GetResponse<GetSellersQueryResponse>(new GetSellersQuery(pageSize, pageNumber,TenantId));
            if (sellers.Message.Sellers.Count==0)
            {
                return NoContent();
            }
            return Ok(sellers.Message);
        }
        [HttpGet]
        public async Task<ActionResult<GetSellersQueryResponse>> GetSellerbyId(string SellerId)
        {
            //var sellers = await _getSellersQuery.GetResponse<GetSellersQueryResponse>(new GetSellersQuery(pageSize, pageNumber));
            //if (sellers.Message.Sellers.Count == 0)
            //{
            //    return NoContent();
            //}
            return Ok();
        }
    }
}
