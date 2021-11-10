using Cloudware.Microservice.Product.DTO;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query
{
    public class GetAllProductsByFiltersQuery :IRequestType
    {
        public string TenantId { get; set; }
        public long? CategoryId { get; set; }
        public long? BrandId { get; set; }
        public SortBy? sortBy { get; set; }
        
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllProductsByFiltersQuery(long? categoryId, long? brandId, SortBy? sortBy, string tenantId , int pageSize = 10, int pageNumber = 1)
        {
            CategoryId = categoryId;
            BrandId = brandId;
            this.sortBy = sortBy;
            PageSize = pageSize;
            PageNumber = pageNumber;
            TenantId = tenantId;
        }
    }
    public enum SortBy
    {
        Latest,
        SpecialOffer,
        MostViewed,
        Newest,
        Hottest,
        MostExpensive,
        LeastExpensive
    }
}
