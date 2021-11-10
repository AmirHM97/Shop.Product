using Cloudware.Microservice.Product.Infrastructure.EntityConfigurations;
using Cloudware.Microservice.Product.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Infrastructure
{
    public class ProductContext : DbContext, IUnitOfWork
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
         
        }
        public DbSet<ProductItem> ProductItems { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<StockItem> Stocks { get; set; }
        public DbSet<StockProperty> StockPropertyItems { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Guarantee> Guarantees { get; set; }
        public DbSet<Size> Sizes { get; set; }
        // public DbSet<Property> Properties { get; set; }
        // public DbSet<PropertyItem> PropertyItems { get; set; }
        public DbSet<ImageUrlItems>  ImageUrls{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductBrandEntitytypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProductCategoryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProductItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StockItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StockPropertyItemEntitytypeConfiguration());
            // modelBuilder.ApplyConfiguration(new PropertyItemEntitytypeConfiguration());
            // modelBuilder.ApplyConfiguration(new PropertyEntitytypeConfiguration());
            modelBuilder.ApplyConfiguration(new ImageUrlItemsEntityTypeConfiguration());
        }
    }
}
