using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Basket;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Query.Property
{
    public class GetPropertyQueryConsumer : IConsumer<GetPropertyQuery>, IMediatorConsumerType
    {
        private readonly IUnitOfWork _unitOfWork;


        private readonly DbSet<Color> _colors;
        private readonly DbSet<Size> _sizes;
        private readonly DbSet<Guarantee> _guarantees;
        public GetPropertyQueryConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _colors = _unitOfWork.Set<Color>();
            _sizes = _unitOfWork.Set<Size>();
            _guarantees = _unitOfWork.Set<Guarantee>();

        }
        public async Task Consume(ConsumeContext<GetPropertyQuery> context)
        {
            var skip = (context.Message.PageNumber - 1) * context.Message.PageSize;
            var properties = new List<GetPropertyQueryResponseItem>();
            switch (context.Message.PropertyType)
            {
                case PropertyType.Color:
                    properties = await _colors.Where(w=>!w.IsDeleted&&w.TenantId==context.Message.TenantId).Skip(skip).Take(context.Message.PageNumber).Select(s => new GetPropertyQueryResponseItem { PropertyId = s.Id, Name = s.Name, PropertyType = PropertyType.Color, Value = s.Code }).ToListAsync();
                    break;
                case PropertyType.Size:
                    properties = await _sizes.Where(w=>!w.IsDeleted&&w.TenantId==context.Message.TenantId).Skip(skip).Take(context.Message.PageNumber).Select(s => new GetPropertyQueryResponseItem { PropertyId = s.Id, Name = s.Name, PropertyType = PropertyType.Size }).ToListAsync();
                    break;
                case PropertyType.Guarantee:
                    properties = await _guarantees.Where(w=>!w.IsDeleted&&w.TenantId==context.Message.TenantId).Skip(skip).Take(context.Message.PageNumber).Select(s => new GetPropertyQueryResponseItem { PropertyId = s.Id, Name = s.Name, PropertyType = PropertyType.Guarantee }).ToListAsync();
                    break;
            }
            await context.RespondAsync(new GetPropertyQueryResponse(properties));
        }
        public class GetPropertyQueryResponse
        {
            public List<GetPropertyQueryResponseItem> Properties { get; set; }

            public GetPropertyQueryResponse(List<GetPropertyQueryResponseItem> properties)
            {
                Properties = properties;
            }
        }
        public class GetPropertyQueryResponseItem
        {
            public PropertyType PropertyType { get; set; }
            public long PropertyId { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}
