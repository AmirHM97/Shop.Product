using System;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Common.Exceptions;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Application.Command.Property
{
    public class EditSizeCommandConsumer : IConsumer<EditSizeCommand>, IMediatorConsumerType
    {
        private readonly DbSet<Size> _sizes;
        private readonly IUnitOfWork _unitOfWork;

        public EditSizeCommandConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _sizes = _unitOfWork.Set<Size>();
        }
        public async Task Consume(ConsumeContext<EditSizeCommand> context)
        {
            var size = await _sizes.FirstOrDefaultAsync(f => f.Guid.ToString().ToLower() == context.Message.Id.ToLower() && f.TenantId == context.Message.TenantId);
            if (size is null)
            {
                throw new AppException(5056,System.Net.HttpStatusCode.BadRequest,"item not found!!!");
            }
            size.Name = context.Message.Size;
            size.LastUpdatedDate = DateTimeOffset.UtcNow;
            _sizes.Update(size);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}