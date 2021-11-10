using System;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Application.Command.Property
{
    public class AddSizeCommandConsumer : IConsumer<AddSizeCommand>, IMediatorConsumerType
    {
        private readonly DbSet<Size> _sizes;
        private readonly IUnitOfWork _unitOfWork;
        public AddSizeCommandConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _sizes = _unitOfWork.Set<Size>();
        }
        public async Task Consume(ConsumeContext<AddSizeCommand> context)
        {
            var size = new Size
            {
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedDate = DateTimeOffset.UtcNow,
                Name = context.Message.AddSizeDto.Size,
                Guid=Guid.NewGuid(),
                TenantId=context.Message.TenantId
            };
            await _sizes.AddAsync(size);
            await _unitOfWork.SaveChangesAsync();
            await context.RespondAsync(new AddSizeCommandResponse(size));

        }
    }
      public class AddSizeCommandResponse{
        public Size Size { get; set; }

        public AddSizeCommandResponse(Size size)
        {
            Size = size;
        }
    }
}