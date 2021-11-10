using Cloudware.Microservice.Product.Model.ReadDbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.DTO
{

    public class SearchDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public SearchType SearchType { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public string? Value { get; set; }
        public List<SearchItemDto>? SearchItems { get; set; }
        public List<ProductCategoryReadDb>? Categories { get; set; }

    }
    public class SearchItemDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    public class SortDto
    {
        public long Id { get; set; }
        public string Name { get; set; }

    }
    /// <summary>
    /// Search Type :  0 = Boolean | 1 = Selection | 2 = Number | 3 = Text
    /// </summary>
    public enum SearchType
    {
        Bool, Selection, Number, Text
    }
}
