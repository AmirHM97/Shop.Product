using Cloudware.Microservice.Product.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Infrastructure.EntityConfigurations
{
    public class StockItemEntityTypeConfiguration : IEntityTypeConfiguration<StockItem>
    {
        public void Configure(EntityTypeBuilder<StockItem> builder)
        {
            builder.ToTable("Stock");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Count).IsRequired();
            builder.Property(ci => ci.Price).IsRequired(); 
            builder.Property(ci => ci.ProductId).IsRequired();

            builder.HasMany(ci => ci.StockPropertyItems).WithOne(ci=>ci.Stock).HasForeignKey(ci=>ci.StockId);

        }
    }
}
