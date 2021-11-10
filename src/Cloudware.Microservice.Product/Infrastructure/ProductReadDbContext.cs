using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Common.Setting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Infrastructure
{
    public interface IProductReadDbContext
    {
        IMongoCollection<ProductCategoryReadDbCollection> ProductCategoriesDataCollection { get; }
        IMongoCollection<ProductItemReadDbCollection> ProductItemsDataCollection { get; }
        IMongoCollection<ProductCategoryNormalizedCollection> ProductCategoryNormalizedCollection { get; }
        IMongoCollection<SellersCollection> SellersCollection { get; }
        IMongoCollection<CustomCategoryCollection> CustomCategoryCollection { get; }

        IMongoCollection<T> GetCollection<T>(string name);
    }

    public class ProductReadDbContext : IProductReadDbContext
    {
        private readonly IMongoDatabase _database = null;
        public ProductReadDbContext(AppSettings config)
        {
            var mongoUrl = MongoUrl.Create(config.ConnectionStrings.MongoDb.Remove(config.ConnectionStrings.MongoDb.LastIndexOf('/')));
            var client = new MongoClient(mongoUrl);
            _database = client.GetDatabase(config.ConnectionStrings.MongoDb.Split('/')[3].Replace(".", "-"));
        }
        public IMongoCollection<ProductItemReadDbCollection> ProductItemsDataCollection
        {
            get
            {
                return _database.GetCollection<ProductItemReadDbCollection>("ProductItemReadDataModel");
            }
        }

        public IMongoCollection<ProductCategoryReadDbCollection> ProductCategoriesDataCollection
        {
            get
            {
                return _database.GetCollection<ProductCategoryReadDbCollection>("ProductCategoryReadDataModel");
            }
        }

        public IMongoCollection<SellersCollection> SellersCollection
        {
            get
            {
                return _database.GetCollection<SellersCollection>("SellersCollection");
            }
        }

        public IMongoCollection<ProductCategoryNormalizedCollection> ProductCategoryNormalizedCollection
        {
            get
            {
                return _database.GetCollection<ProductCategoryNormalizedCollection>("ProductCategoryNormalizedCollection");
            }
        }
        public IMongoCollection<CustomCategoryCollection> CustomCategoryCollection
        {
            get
            {
                return _database.GetCollection<CustomCategoryCollection>("CustomCategoryCollection");
            }
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }
}
