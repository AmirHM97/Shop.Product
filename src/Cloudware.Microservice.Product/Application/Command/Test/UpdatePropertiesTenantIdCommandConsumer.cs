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

namespace Cloudware.Microservice.Product.Application.Command.Test
{
    public class UpdatePropertiesTenantIdCommandConsumer : IConsumer<UpdatePropertiesTenantIdCommand>, IMediatorConsumerType
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSet<Color> _colors;
        private readonly DbSet<Size> _sizes;
        private readonly DbSet<Guarantee> _guarantees;
        public UpdatePropertiesTenantIdCommandConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _colors = _unitOfWork.Set<Color>();
            _sizes = _unitOfWork.Set<Size>();
            _guarantees = _unitOfWork.Set<Guarantee>();

        }
        public async Task Consume(ConsumeContext<UpdatePropertiesTenantIdCommand> context)
        {
            var color = await _colors.ToListAsync();
            color = color.Select(s => { s.TenantId = context.Message.TenantId; return s; }).ToList();
            var s = await _sizes.ToListAsync();
            s = s.Select(s => { s.TenantId = context.Message.TenantId; return s; }).ToList();
            var g = await _guarantees.ToListAsync();
            g = g.Select(s => { s.TenantId = context.Message.TenantId; return s; }).ToList();
            await _unitOfWork.SaveChangesAsync();
        }
    }
}