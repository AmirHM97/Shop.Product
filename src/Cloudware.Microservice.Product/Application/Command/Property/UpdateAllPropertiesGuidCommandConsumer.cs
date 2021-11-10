using System;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Application.Command.Property
{
    public class UpdateAllPropertiesGuidCommandConsumer : IConsumer<UpdateAllPropertiesGuidCommand>, IMediatorConsumerType
    {
        private readonly DbSet<Size> _sizes;
        private readonly DbSet<Guarantee> _guarantees;
        private readonly DbSet<Color> _colors;

        private readonly IUnitOfWork _unitOfWork;

        public UpdateAllPropertiesGuidCommandConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _colors = _unitOfWork.Set<Color>();
            _guarantees = _unitOfWork.Set<Guarantee>();
            _sizes = _unitOfWork.Set<Size>();

        }

        public async Task Consume(ConsumeContext<UpdateAllPropertiesGuidCommand> context)
        {
            var colors = await _colors.ToListAsync();
            colors = colors.Select(s => { s.Guid = Guid.NewGuid(); return s; }).ToList();
            _colors.UpdateRange(colors);
            var gs = await _guarantees.ToListAsync();
            gs = gs.Select(s => { s.Guid = Guid.NewGuid(); return s; }).ToList();
            _guarantees.UpdateRange(gs);

            var ss = await _sizes.ToListAsync();
            ss = ss.Select(s => { s.Guid = Guid.NewGuid(); return s; }).ToList();
            _sizes.UpdateRange(ss);
            await _unitOfWork.SaveChangesAsync();
            
        }
    }
}