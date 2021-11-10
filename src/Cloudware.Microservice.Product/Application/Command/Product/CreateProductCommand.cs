using Cloudware.Microservice.Product.Application.Query;
using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Basket;
using Cloudware.Utilities.Contract.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Utilities.Formbuilder.Dtos;

namespace Cloudware.Microservice.Product.Application.Command
{
    public class CreateProductCommand : IRequestType
    {
        public CreateProductCommand(CreateProductCommandDto dto, string userId, string tenantId)
        {
            UserId = userId;
            TenantId = tenantId;
            Code = dto.Code;
            Name = dto.Name;
            UserName = dto.UserName;
            UserImageUrl = dto.UserImageUrl;
            UserDescription = dto.UserDescription;
            ImageUrls = dto.ImageUrls??new();
            Image = dto.Image;
            Description = dto.Description;
            Story = dto.Story;
            BrandId = dto.BrandId;
            CategoryId = dto.CategoryId;
            IsActive = dto.IsAvailable;
            StocksItems = dto.StocksItems;
            // TechnicalProperties = dto.TechnicalProperties;
            ProvinceId = dto.ProvinceId;
            Province = dto.Province;
            CityId = dto.CityId;
            City = dto.City;
            Address = dto.Address;
            Mobile = dto.Mobile;
            TechnicalProperty=dto.TechnicalProperties;
            TechnicalProperties=dto.TechnicalPropertiesDto;
            Dimensions=dto.Dimensions;
            Weight=dto.Weight;
            SupplierId = dto.SupplierId;
        }
        public string TenantId { get; set; }
        public string UserId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? UserName { get; set; }
        public string? UserImageUrl { get; set; }
        public string? UserDescription { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public string Image { get; set; }
        public string? Dimensions { get; set; }
        public double? Weight { get; set; }
      
        public string? Description { get; set; }
        public string? Story { get; set; }
        public long? BrandId { get; set; }
        public long? CategoryId { get; set; }
        public long? ProvinceId { get; set; }
        public string? Province { get; set; }
        public long? CityId { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Mobile { get; set; }
        public long SupplierId { get; set; }
        public bool IsActive { get; set; }=true;
        //To Do
        // public List<Model.PropertyType> Properties { get; set; } = new();
        public List<StockItemCommandDto> StocksItems { get; set; } = new();
        public List<TechnicalPropertiesCommandDto> TechnicalProperties { get; set; } = new();
        public TechnicalPropertyCommandDto TechnicalProperty { get; set; } = new();


    }
    public class StockItemCommandDto
    {
        public bool? IsAvailable { get; set; }=true;
        public int Count { get; set; }
        public int MinCount { get; set; } = 0;
        public int MaxCount { get; set; } = 0;
        public decimal Price { get; set; }
        public double Discount { get; set; }
        public List<PropertyCommandDto> PropItems { get; set; } = new();
    }
    public class PropertyCommandDto
    {
        public string PropertyId { get; set; }//color guarantee
        public PropertyType PropertyType { get; set; }
        // public List<PropertyItemCommandDto> PropertyItems { get; set; } = new();
    }
    // public class PropertyItemCommandDto
    // {
    //     public long Id { get; set; }
    //     public string Name { get; set; }// red guarantee name
    //     public string? Value { get; set; }
    // }
    public class TechnicalPropertyCommandDto
    {
        public AddRecordDto AddRecord { get; set; }
        // public string Name { get; set; }
        // public string Value { get; set; }

    }
    public class TechnicalPropertiesCommandDto
    {
      
        public string Name { get; set; }
        public string Value { get; set; }

    }
}


