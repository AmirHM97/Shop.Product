using Cloudware.Utilities.Contract.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query.Search_Filters
{
    public class GetSearchSuggestionsQuery:IRequestType
    {
        public string SearchQuery { get; set; }
        public string TenantId { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public GetSearchSuggestionsQuery(string searchQuery, string tenantId, int pageSize, int pageNumber)
        {
            SearchQuery = searchQuery;
            TenantId = tenantId;
            PageSize = pageSize;
            PageNumber = pageNumber;
        }
    }
}
