using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Utilities.Contract.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query.Search_Filters
{
    public class SearchProductsQuery:IRequestType
    {
        public SearchProductsQuery(List<AdvancedSearchItemDto> searches, SortBy sortBy, int pageSize = 10, int pageNumber=1 )
        {
            Searches = searches;
            this.sortBy = sortBy;
            PageNumber = pageNumber;
            PageSize = pageSize;
            useV1 = true;
        }
        public SearchProductsQuery(AdvancedSearchItemDtoV2 searches, string tenantId, int pageSize = 10, int pageNumber = 1, bool addpagination = true)
        {
            SearchesV2 = searches;
            this.sortBy = sortBy;
            PageNumber = pageNumber;
            PageSize = pageSize;
            useV1 = false;
            TenantId = tenantId;
            Addpagination = addpagination;
        }
        public AdvancedSearchItemDtoV2 SearchesV2 { get; set; } = new();
        public bool useV1 { get; set; }
        public List<AdvancedSearchItemDto>? Searches { get; set; } = new();
        public SortBy? sortBy { get; set; } = SortBy.Newest;
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string TenantId { get; set; }
        public bool Addpagination { get; set; }
    }
}
