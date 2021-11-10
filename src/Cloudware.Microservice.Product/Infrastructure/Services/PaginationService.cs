using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Infrastructure.Services
{
    public class PaginationService : IPaginationService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public PaginationService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        public object Pagination<T>(IQueryable<T> items, int pageSize)
        {
            var totalPages = (int)Math.Ceiling(items.Count() / (double)pageSize);

            var paginationHeader = new
            {
                TotalCount = items.Count(),
                TotalPages = totalPages
            };
            httpContextAccessor.HttpContext.Response.Headers.TryAdd("X-Pagination", JsonConvert.SerializeObject(paginationHeader));
            httpContextAccessor.HttpContext.Response.Headers.TryAdd("Access-Control-Expose-Headers", "X-Pagination");

            return paginationHeader;
        }
    }
}
