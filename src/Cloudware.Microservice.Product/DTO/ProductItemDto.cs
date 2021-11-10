using Cloudware.Microservice.Product.Model;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.DTO
{
    public class ProductItemDto
    {
        public ProductItemReadDbCollection ProductData { get; set; }
        public List<ProductItemsListCatalogDto> RelatedProducts { get; set; }
        public ProductItemDto(ProductItemReadDbCollection productData, List<ProductItemsListCatalogDto> relatedProducts)
        {
            ProductData = productData;
            RelatedProducts = relatedProducts;
        }
        public ProductItemDto()
        {

        }
        // public long Id { get; set; }
        // public long UserId { get; set; }
        // public string UserName { get; set; }
        // public string UserImageUrl { get; set; }
        // public string UserDescription { get; set; }
        // public string Name { get; set; }
        // public string ImageUrl { get; set; }
        // public DateTime CreatedDate { get; set; }
        // public DateTime LastUpdatedDate { get; set; }
        // public List<string> ImageUrls { get; set; }
        // public long? ViewCount { get; set; }
        // public double Weight { get; set; }
        // public string Description { get; set; }
        // public string Story { get; set; }
        // public bool IsAvailable { get; set; }
        // public long Dimensions { get; set; }
        // public long BrandId { get; set; }
        // public string BrandName { get; set; }
        // public long CategoryId { get; set; }
        // public string CategoryName { get; set; }
        // public List<PropertyDto> PropertiesDto { get; set; }
        // public List<StockItemDto>  StockItemsDto{ get; set; }
        // public List<TechnicalPropertyDto>  TechnicalProperties { get; set; }
    }
}
