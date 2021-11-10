using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Common.Exceptions;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
//using MongoDB.Bson;
//using MongoDB.Driver;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using MongoDB.Bson.Serialization;
using Cloudware.Microservice.Product.Infrastructure.Extensions;

namespace Cloudware.Microservice.Product.Application.Command
{
    public class UpdateSellersCollectionCommandConsumer : IConsumer<UpdateSellersCollectionCommand>, IMediatorConsumerType
    {
        private readonly DbSet<ProductItem> _productItems;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<UpdateSellersCollectionCommandConsumer> _logger;
        private readonly IProductReadDbContext _readDbContext;

        public UpdateSellersCollectionCommandConsumer(IUnitOfWork uow, ILogger<UpdateSellersCollectionCommandConsumer> logger, IProductReadDbContext readDbContext)
        {
            _uow = uow;
            _productItems = _uow.Set<ProductItem>();
            _logger = logger;
            _readDbContext = readDbContext;
        }

        [Obsolete]
        public async Task Consume(ConsumeContext<UpdateSellersCollectionCommand> context)
        {
            try
            {

                var productCollection = _readDbContext.ProductItemsDataCollection;
                var sellersCollection = _readDbContext.SellersCollection;
                #region comment
                //var group = new SellersCollection
                //{
                //    Name = new BsonDocument { { "$first", "$UserName" } }.ToString(),
                //    ImageUrl = new BsonDocument { { "$first", "$UserImageUrl" } }.ToString(),
                //    Description = new BsonDocument { { "$first", "$UserDescription" } }.ToString(),
                //    SellerId = new BsonDocument { { "$first", "$UserId" } }.ToString()
                //}; 
                //var sellers = await productCollection
                //    .Aggregate()
                //    .Group(groupByAlertId).Project(p => new SellersCollection
                //    {
                //        SellerId = p["UserId"].ToString(),
                //        Name = p["UserName"].ToString(),
                //        ImageUrl = p["UserImageUrl"].ToString(),
                //        Description = p["UserDescription"].ToString(),
                //    }).ToListAsync();
                // var rc = seller.ToJson<BsonDocument>();
                //var groupByUserId = new BsonDocument
                //  {
                //    { "_id", "$UserId" },
                //      { "UserName", new BsonDocument { { "$first", "$UserName" } } },
                //      { "UserImageUrl", new BsonDocument { { "$first", "$UserImageUrl" } } },
                //      { "UserDescription", new BsonDocument { { "$first", "$UserDescription" } } },
                //      { "UserId", new BsonDocument { { "$first", "$UserId" } } }
                //  };
                //var sellersData = await productCollection
                //    .Aggregate()
                //    .Group(groupByUserId).ToListAsync();
                //var selelrDto = (from item in sellersData
                //                 select BsonSerializer.Deserialize<SellersDto>(item)).ToList();
                //var sellerCollectionList = ProductExtensions.MapSellerDtoToSellerCollection(selelrDto);
                //  await sellersCollection.InsertManyAsync(sellerCollectionList);
                #endregion

                var sellersData = await productCollection
                   .Aggregate()
                   .Group(g => g.UserId, g => new SellersCollection
                   {
                       SellerId = g.Key,
                       Name = g.Select(s => s.UserName).First(),
                       Description = g.Select(s => s.UserDescription).First(),
                       ImageUrl = g.Select(s => s.UserImageUrl).First(),
                   }).ToListAsync();


                await sellersCollection.InsertManyAsync(sellersData);
                await context.RespondAsync(new UpdateSellersCollectionCommandResponse(1));
            }
            catch (Exception e)
            {
                _logger.LogError($"UpdateSellersCollectionCommand failed with error {e.Message} !!!!");
                throw new AppException(1500, HttpStatusCode.InternalServerError);
            }
        }
        public class UpdateSellersCollectionCommandResponse
        {
            public int Result { get; set; }

            public UpdateSellersCollectionCommandResponse(int result)
            {
                Result = result;
            }
        }
        public class SellersDto
        {
            public string _id { get; set; }
            public string UserName { get; set; }
            public string UserImageUrl { get; set; }
            public string UserDescription { get; set; }
            public string UserId { get; set; }
        }
    }
}
