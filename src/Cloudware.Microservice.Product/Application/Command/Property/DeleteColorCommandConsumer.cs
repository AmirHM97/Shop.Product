using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;


namespace Cloudware.Microservice.Product.Application.Command.Property
{
    public class DeleteColorCommandConsumer : IConsumer<DeleteColorCommand>, IMediatorConsumerType
    {

        private readonly DbSet<Color> _colors;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteColorCommandConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _colors = _unitOfWork.Set<Color>();
        }

        public async Task Consume(ConsumeContext<DeleteColorCommand> context)
        {
            var color = await _colors.FirstOrDefaultAsync(f => f.TenantId == context.Message.TenantId && f.Guid.ToString().ToLower() == context.Message.Guid);
            color.IsDeleted = true;
            _colors.Update(color);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}