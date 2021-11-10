using Cloudware.Microservice.Product.Application.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.DTO
{
    public class CreateProductCommandDto
    {
        public string UserId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? UserName { get; set; }
        public string? UserImageUrl { get; set; }
        public string? UserDescription { get; set; }
        public List<string> ImageUrls { get; set; }
        public string Image { get; set; }
        public double? Weight { get; set; }
        public string? Description { get; set; }
        public string? Story { get; set; }
        public long? BrandId { get; set; }
        public long? CategoryId { get; set; }
        public string? Dimensions { get; set; }
        public bool IsAvailable { get; set; }=true;
        public long? ProvinceId { get; set; }
        public string? Province { get; set; }
        public long? CityId { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Mobile { get; set; }
        public long SupplierId { get; set; }
        //To Do
        // public List<PropertyCommandDto> Properties { get; set; }
        public List<StockItemCommandDto> StocksItems { get; set; }
        public TechnicalPropertyCommandDto? TechnicalProperties { get; set; }
        public List<TechnicalPropertiesCommandDto>? TechnicalPropertiesDto { get; set; }
    }
}
