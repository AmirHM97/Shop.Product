using Cloudware.Microservice.Product.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Infrastructure.EntityConfigurations
{
    public class ImageUrlItemsEntityTypeConfiguration : IEntityTypeConfiguration<ImageUrlItems>
    {
        public void Configure(EntityTypeBuilder<ImageUrlItems> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.Property(ci => ci.Url).IsRequired();
            builder.Property(ci => ci.ProductItemId).IsRequired();
        }
    }
}
