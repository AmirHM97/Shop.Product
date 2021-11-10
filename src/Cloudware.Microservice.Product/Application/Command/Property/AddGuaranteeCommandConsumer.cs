using System;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Application.Command.Property
{
    public class AddGuaranteeCommandConsumer : IConsumer<AddGuaranteeCommand>, IMediatorConsumerType
    {
        private readonly DbSet<Guarantee> _guarantees;
        private readonly IUnitOfWork _unitOfWork;
        public AddGuaranteeCommandConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _guarantees = _unitOfWork.Set<Guarantee>();
        }
        public async Task Consume(ConsumeContext<AddGuaranteeCommand> context)
        {
            var guarantee = new Guarantee
            {
                BackImage = context.Message.AddGuaranteeDto.BackImage,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedDate = DateTimeOffset.UtcNow,
                Duration =context.Message.AddGuaranteeDto.Duration,
                FrontImage = context.Message.AddGuaranteeDto.FrontImage,
                Name = context.Message.AddGuaranteeDto.Name,
                 Guid=Guid.NewGuid(),
                 TenantId=context.Message.TenantId
            };
            await _guarantees.AddAsync(guarantee);
            await _unitOfWork.SaveChangesAsync();
            await context.RespondAsync(new AddGuaranteeCommandResponse(guarantee));
        }
    }
     public class AddGuaranteeCommandResponse{
        public Guarantee Guarantee { get; set; }

        public AddGuaranteeCommandResponse(Guarantee guarantee)
        {
            Guarantee = guarantee;
        }
    }
}