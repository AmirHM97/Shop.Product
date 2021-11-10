using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Application.Command.Property
{
    public class DeleteSizeCommandConsumer : IConsumer<DeleteSizeCommand>, IMediatorConsumerType
    {

        private readonly DbSet<Size> _sizes;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSizeCommandConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _sizes=_unitOfWork.Set<Size>();
        }

        public async Task Consume(ConsumeContext<DeleteSizeCommand> context)
        {
            var size=await _sizes.FirstOrDefaultAsync(f=>f.TenantId==context.Message.TenantId&&f.Guid.ToString().ToLower()==context.Message.Guid);
            size.IsDeleted=true;
            _sizes.Update(size);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}