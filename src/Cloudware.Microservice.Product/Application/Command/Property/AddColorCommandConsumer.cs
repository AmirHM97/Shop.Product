using System;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Application.Command.Property
{
    public class AddColorCommandConsumer : IConsumer<AddColorCommand>, IMediatorConsumerType
    {
        private readonly DbSet<Color> _colors;
        private readonly IUnitOfWork _unitOfWork;

        public AddColorCommandConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _colors = _unitOfWork.Set<Color>();
        }


        public async Task Consume(ConsumeContext<AddColorCommand> context)
        {
            var color = new Color
            {
                Code = context.Message.AddColorDto.ColorCode,
                Name = context.Message.AddColorDto.Name,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedDate = DateTimeOffset.UtcNow,
                Guid=Guid.NewGuid(),
                TenantId=context.Message.TenantId
            };
            await _colors.AddAsync(color);
            await _unitOfWork.SaveChangesAsync();
            await context.RespondAsync(new AddColorCommandResponse(color));
        }
    }
    public class AddColorCommandResponse{
        public Color Color { get; set; }

        public AddColorCommandResponse(Color color)
        {
            Color = color;
        }
    }
}