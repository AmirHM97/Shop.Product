using System;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Common.Exceptions;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Cloudware.Microservice.Product.Application.Command.Property
{
    public class EditColorCommandConsumer : IConsumer<EditColorCommand>, IMediatorConsumerType
    {
        private readonly DbSet<Color> _colors;
        private readonly IUnitOfWork _unitOfWork;

        public EditColorCommandConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _colors=_unitOfWork.Set<Color>();
        }

        public async Task Consume(ConsumeContext<EditColorCommand> context)
        {
            var color=await _colors.FirstOrDefaultAsync(f=>f.Guid.ToString().ToLower()==context.Message.Id.ToLower() &&f.TenantId==context.Message.TenantId);
            if (color is null)
            {
                throw new AppException(5056,System.Net.HttpStatusCode.BadRequest,"item not found!!!");
            }
            color.Name=context.Message.Name;
            color.Code=context.Message.Code;
            color.LastUpdatedDate=DateTimeOffset.UtcNow;

            _colors.Update(color);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}