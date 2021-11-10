using Cloudware.Microservice.Product.Application.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.DTO
{
    public class AdvancedSearchItemDto
    {
        //public SearchType SearchType { get; set; }
        public SearchOn SearchOn { get; set; }
        public List<long>? SelectionIds { get; set; }
        public string? SearchQuery { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
    }

    public class AdvancedSearchItemDtoV2
    {

        public List<long> CategoryIds { get; set; } = new List<long>();
        public List<long> BrandIds { get; set; } =new List<long> ();
        public bool IsAvailable { get; set; } = false;
        //todo fix name=>
        public bool DeliverdBySeller { get; set; } = false;
        public bool HaveDiscount { get; set; } = false;
        public List<Attributes> Attributes { get; set; } = new();
        public string SearchQuery { get; set; } = "";
        public decimal MinValue { get; set; } = 0;
        public decimal MaxValue { get; set; } = 0;
        public SortBy SortBy { get; set; } = SortBy.Newest;
       
    }
    public class Attributes
    {
        public int MyProperty { get; set; }
    }
    /// <summary>
    ///  0 = category | 1 = brand | 2 = Price | 3 = IsAvailable | 4 = Text | 5 = HaveDiscount | 6 = DeliveredBySeller
    /// </summary>
    public enum SearchOn
    {
        Category,Brand,Price,IsExist,Text,HaveDiscount,DeliveredBySeller
    }
}
