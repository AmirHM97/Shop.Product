using Cloudware.Microservice.Product.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Infrastructure.EntityConfigurations
{
    public class StockPropertyItemEntitytypeConfiguration : IEntityTypeConfiguration<StockProperty>
    {
        public void Configure(EntityTypeBuilder<StockProperty> builder)
        {
            builder.ToTable("StockProperty");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.StockId).IsRequired();

            // builder.HasOne(ci => ci.PropItem).WithMany(ci=>ci.StockPropertyItems).HasForeignKey(ci => ci.PropItemId).OnDelete(DeleteBehavior.ClientNoAction);

        }
    }
}
