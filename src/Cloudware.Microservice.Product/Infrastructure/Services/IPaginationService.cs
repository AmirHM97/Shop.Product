using System.Linq;

namespace Cloudware.Microservice.Product.Infrastructure.Services
{
    public interface IPaginationService
    {
        object Pagination<T>(IQueryable<T> items, int pageSize);
    }
}