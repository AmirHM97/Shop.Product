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
     public class EditGuaranteeCommandConsumer : IConsumer<EditGuaranteeCommand>, IMediatorConsumerType
    {
        private readonly DbSet<Guarantee> _guarantees;
        private readonly IUnitOfWork _unitOfWork;

        public EditGuaranteeCommandConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _guarantees=_unitOfWork.Set<Guarantee>();
        }

        public async Task Consume(ConsumeContext<EditGuaranteeCommand> context)
        {
            var guarantee=await _guarantees.FirstOrDefaultAsync(f=>f.Guid.ToString().ToLower()==context.Message.Id.ToLower()&&f.TenantId==context.Message.TenantId);
            if (guarantee is null)
            {
                throw new AppException(5056,System.Net.HttpStatusCode.BadRequest,"item not found!!!");
            }
            guarantee.Name=context.Message.Name;
            guarantee.BackImage=context.Message.BackImage;
            guarantee.FrontImage=context.Message.FrontImage;
            guarantee.Duration=context.Message.Duration;
            guarantee.LastUpdatedDate=DateTimeOffset.UtcNow;
            _guarantees.Update(guarantee);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}