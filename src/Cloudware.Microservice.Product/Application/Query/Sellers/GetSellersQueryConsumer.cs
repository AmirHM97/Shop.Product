using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Infrastructure.Services;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Common.Exceptions;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query.Sellers
{
    public class GetSellersQueryConsumer : IConsumer<GetSellersQuery>, IMediatorConsumerType
    {
        //private readonly ProductReadDbContext _readDbContext;
        private readonly IProductReadDbContext _readDbContext;
        private readonly IPaginationService _paginationService;
        private readonly ILogger<GetSellersQueryConsumer> _logger;
        public GetSellersQueryConsumer( IPaginationService paginationService, ILogger<GetSellersQueryConsumer> logger, IProductReadDbContext readDbContext)
        {
          //  
            _paginationService = paginationService;
            _logger = logger;
            _readDbContext = readDbContext;
        }
        public async Task Consume(ConsumeContext<GetSellersQuery> context)
        {
            try
            {

                int skip = (context.Message.PageNumber - 1) * context.Message.PageSize;

                var sellersCollection = _readDbContext.SellersCollection;
               
                var sellers = await sellersCollection.Find(t =>t.TenantId==context.Message.TenantId).ToListAsync();
                _paginationService.Pagination(sellers.AsQueryable(), context.Message.PageSize);
                var sellersListWithPagination = sellers.Skip(skip).Take(context.Message.PageSize).ToList();

                await context.RespondAsync(new GetSellersQueryResponse(sellersListWithPagination));
            }
            catch (Exception e)
            {
                _logger.LogError($"Get sellers Operation Failed with error {e.Message}");
                _logger.LogDebug($"Get sellers Operation Failed with error {e.Message}");
                await context.RespondAsync(new GetSellersQueryResponse(new List<SellersCollection>()));
                throw new AppException();

            }
        }
        public class GetSellersQueryResponse
        {
            public List<SellersCollection> Sellers { get; set; }

            public GetSellersQueryResponse(List<SellersCollection> sellers)
            {
                Sellers = sellers;
            }
        }
    }
}
