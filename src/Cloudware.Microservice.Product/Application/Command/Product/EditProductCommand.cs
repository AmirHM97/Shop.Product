using Cloudware.Microservice.Product.Application.Query;
using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Basket;
using Cloudware.Utilities.Formbuilder.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Command
{
    public class EditProductCommand : IRequestType
    {
        public EditProductCommand(EditProductCommandDto editProductCommandDto, string userId, string tenantId)
        {
            Id = editProductCommandDto.Guid;
            UserId = userId;
            Code = editProductCommandDto.Code;
            Name = editProductCommandDto.Name;
            ImageUrls = editProductCommandDto.ImageUrls;
            Image = editProductCommandDto.Image;
            Description = editProductCommandDto.Description;
            Story = editProductCommandDto.Story;
            BrandId = editProductCommandDto.BrandId;
            CategoryId = editProductCommandDto.CategoryId;
            IsAvailable = editProductCommandDto.IsAvailable;
            ProvinceId = editProductCommandDto.ProvinceId;
            Province = editProductCommandDto.Province;
            CityId = editProductCommandDto.CityId;
            City = editProductCommandDto.City;
            Address = editProductCommandDto.Address;
            Mobile = editProductCommandDto.Mobile;
            OldRecordId = editProductCommandDto.OldRecordId??"";
            AddRecordDto = editProductCommandDto.AddRecordDto??new();
            StocksItems = editProductCommandDto.StocksItems??new();
            TechnicalPropertiesDto = editProductCommandDto.TechnicalPropertiesDto??new();
            TenantId = tenantId;
            SupplierId = editProductCommandDto.SupplierId;
        }



        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string TenantId { get; set; }
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
        public string? OldRecordId { get; set; }
        public long SupplierId { get; set; }
        public AddRecordDto? AddRecordDto { get; set; }
        // public List<PropertyEditCommandDto>? Properties { get; set; } = new();
        public List<StockItemEditCommandDto>? StocksItems { get; set; } = new();

        public List<TechnicalPropertyEditCommandDto>? TechnicalPropertiesDto { get; set; } = new();
    }
    // public class PropertyEditCommandDto
    // {
    //     public long? Id { get; set; }
    //     public string Name { get; set; }//color guarantee
    //     public PropertyType PropertyType { get; set; }
    //     public List<PropertyItemEditCommandDto> PropertyItems { get; set; } = new();


    // }
    // public class PropertyItemEditCommandDto
    // {
    //     public long Id { get; set; }
    //     public string Name { get; set; }// red guarantee name
    //     public string? Value { get; set; }
    //      public bool IsAdded { get; set; }
    //      public bool IsEdited { get; set; }
    // }
    public class StockItemEditCommandDto
    {
        public string Id { get; set; }
        public long Count { get; set; }
        public decimal Price { get; set; }
        public double Discount { get; set; }
        public bool IsAvailable { get; set; }=true;
        public bool IsDeleted { get; set; }=false;
        public List<PropertyCommandDto> PropItems { get; set; } = new();
    }
    public class TechnicalPropertyEditCommandDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
