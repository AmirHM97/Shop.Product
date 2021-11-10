using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Model;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Contract.Basket;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Infrastructure.Services
{
    public interface IPropertyService
    {
        Task<List<PropertyItemReadDb>> GetPropertiesByPropertyType(PropertyCategory item, List<long> props);
    }

    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Color> _colors;
        private readonly DbSet<Guarantee> _guarantees;
        private readonly DbSet<Size> _sizes;
        public PropertyService(IUnitOfWork uow)
        {
            _uow = uow;
            _colors = _uow.Set<Color>();
            _guarantees = _uow.Set<Guarantee>();
            _sizes = _uow.Set<Size>();
        }
        public async Task<List<PropertyItemReadDb>> GetPropertiesByPropertyType(PropertyCategory item, List<long> props)
        {
            return item.PropertyType switch
            {
                PropertyType.Color => await _colors.Where(w => props.Contains(w.Id)).Select(s => new PropertyItemReadDb
                {
                    Id = s.Id,
                    Name = s.Name,
                    Value = s.Code,
                }).ToListAsync(),
                PropertyType.Size => await _sizes.Where(w => props.Contains(w.Id)).Select(s => new PropertyItemReadDb
                {
                    Id = s.Id,
                    Name = s.Name,
                }).ToListAsync(),
                PropertyType.Guarantee => await _guarantees.Where(w => props.Contains(w.Id)).Select(s => new PropertyItemReadDb
                {
                    Id = s.Id,
                    Name = s.Name,
                }).ToListAsync(),
                _ => new(),
            };
        }
    }
}
