using Cloudware.Microservice.Product.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Infrastructure.EntityConfigurations
{
    public class ProductCategoryEntityTypeConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Name).IsRequired();

            builder.HasMany(ci => ci.Children).WithOne(ci=>ci.Parent).HasForeignKey(ci=>ci.ParentId);
            // builder.HasMany(ci => ci.Properties).WithOne(ci=>ci.ProductCategory).HasForeignKey(ci=>ci.ProductItemId);
            //builder.HasOne(ci => ci.Parent).WithOne();
        }
    }
}
