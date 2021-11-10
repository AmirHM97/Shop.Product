using Cloudware.Microservice.Product.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Infrastructure.EntityConfigurations
{
    public class ProductItemEntityTypeConfiguration : IEntityTypeConfiguration<ProductItem>
    {
        public void Configure(EntityTypeBuilder<ProductItem> builder)
        {
            builder.ToTable("Product");
            
            builder.HasKey(ci => ci.Id);
            
            builder.Property(ci => ci.Id).IsRequired();
            builder.Property(ci => ci.UserId).IsRequired();
            builder.Property(ci => ci.Code).IsRequired();
            builder.Property(ci => ci.Name).IsRequired();
            builder.Property(ci => ci.Weight).IsRequired();
            builder.Property(ci => ci.Dimensions).IsRequired();
            builder.Property(ci => ci.Description).IsRequired();

            builder.Property(ci => ci.Image).IsRequired();

            builder.HasOne(ci => ci.Category).WithMany(ci=>ci.ProductItems).HasForeignKey(ci => ci.CategoryId);
            builder.HasOne(ci => ci.Brand).WithMany(ci=>ci.ProductItems).HasForeignKey(ci => ci.BrandId);

            builder.HasMany(ci => ci.StocksItems).WithOne(ci=>ci.ProductItem).HasForeignKey(ci=>ci.ProductId);
            builder.HasMany(ci => ci.ImageUrls).WithOne(ci=>ci.ProductItem).HasForeignKey(ci=>ci.ProductItemId);
            // builder.HasMany(ci => ci.PropertyItems).WithOne(ci=>ci.ProductItem).HasForeignKey(ci=>ci.ProductItemId);

        }
    }
}
