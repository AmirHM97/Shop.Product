
using Cloudware.Utilities.Common.Setting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Infrastructure
{
    public class ProductDbContextFactory : IDesignTimeDbContextFactory<ProductContext>
    {
        

        public ProductContext CreateDbContext(string[] args)
        {
            // IConfigurationRoot configuration = new ConfigurationBuilder()
            // .SetBasePath(Directory.GetCurrentDirectory())
            // .AddJsonFile("appsettings.json")
            // .Build();
            // var connection = configuration.GetSection("ConnectionStrings:MsSql");
            var optionsBuilder = new DbContextOptionsBuilder<ProductContext>();
            optionsBuilder.UseSqlServer();
            return new ProductContext(optionsBuilder.Options);
        }


    }
}
