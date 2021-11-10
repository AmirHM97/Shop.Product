using System;
using System.Collections.Generic;
using Cloudware.Microservice.Product.Application.Command;
using Cloudware.Utilities.Formbuilder.Dtos;

namespace Cloudware.Microservice.Product.DTO
{
    public class EditProductCommandDto
    {
          public Guid Guid { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public string Image { get; set; }
        public string? Description { get; set; }
        public string? Story { get; set; }
        public long? BrandId { get; set; }
        public long? CategoryId { get; set; }
        public bool IsAvailable { get; set; }
        public long? ProvinceId { get; set; }
        public string? Province { get; set; }
        public long? CityId { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Mobile { get; set; }
        public string? OldRecordId{ get; set; }
        public long SupplierId { get; set; }
        public AddRecordDto? AddRecordDto{ get; set; }
        // public List<PropertyEditCommandDto>? Properties { get; set; } = new();
        public List<StockItemEditCommandDto>? StocksItems { get; set; } = new();
        public List<TechnicalPropertyEditCommandDto>? TechnicalPropertiesDto { get; set; } = new();
    }
}