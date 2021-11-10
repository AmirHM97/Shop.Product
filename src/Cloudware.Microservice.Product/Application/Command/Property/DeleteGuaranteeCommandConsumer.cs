using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Application.Command.Property
{
    public class DeleteGuaranteeCommandConsumer : IConsumer<DeleteGuaranteeCommand>, IMediatorConsumerType
    {
        private readonly DbSet<Guarantee> _guarantees;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteGuaranteeCommandConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _guarantees = _unitOfWork.Set<Guarantee>();
        }

        public async Task Consume(ConsumeContext<DeleteGuaranteeCommand> context)
        {
            var guarantee = await _guarantees.FirstOrDefaultAsync(f => f.TenantId == context.Message.TenantId && f.Guid.ToString().ToLower() == context.Message.Guid);
            guarantee.IsDeleted = true;
            _guarantees.Update(guarantee);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}