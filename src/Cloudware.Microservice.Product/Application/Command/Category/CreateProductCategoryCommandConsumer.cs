using Cloudware.Microservice.Product.Application.Events;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Model;
using Cloudware.Utilities.Common.Exceptions;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using MassTransit.Mediator;
//using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Command
{
    public class CreateProductCategoryCommandConsumer : IConsumer<CreateProductCategoryCommand>, IMediatorConsumerType
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ProductCategory> _productCategories;
        private readonly ILogger<CreateProductCategoryCommandConsumer> _logger;

        public CreateProductCategoryCommandConsumer(IMediator mediator, IUnitOfWork uow, ILogger<CreateProductCategoryCommandConsumer> logger)
        {
            _mediator = mediator;
            _uow = uow;
            _productCategories = _uow.Set<ProductCategory>();
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CreateProductCategoryCommand> context)
        {
            try
            {
                var category = await _productCategories.AddAsync(new ProductCategory
                {
                    CreatedDate=DateTimeOffset.UtcNow,
                    LastUpdatedDate=DateTimeOffset.UtcNow,
                    TenantId = context.Message.TenantId,
                    Name = context.Message.Name,
                    Description=context.Message.Description,
                    Icon=context.Message.Icon,
                    Guid=Guid.NewGuid(),
                    ParentId = context.Message.ParentId != 0 ? context.Message.ParentId : null
                });
                //var category = await _productCategories.AddAsync(new ProductCategory { Name = context.Message.Name, ParentId = context.Message.ParentId!=0? context.Message.ParentId:null });
                await _uow.SaveChangesAsync();
                await context.RespondAsync(new CreateProductCategoryCommandResponse(category.Entity.Id,category.Entity.Guid.ToString()));
                await _mediator.Publish(new ProductCategoryCreatedEvent(context.Message.TenantId));
            }
            catch (Exception e)
            {
                await context.RespondAsync(new CreateProductCategoryCommandResponse(0,""));
                _logger.LogError($"productCategory Creation failed with error {e.Message}");
                throw new AppException(105, HttpStatusCode.BadRequest);
            }
        }
        public class CreateProductCategoryCommandResponse
        {
            public long CategoryId { get; set; }
            public string Guid { get; set; }

            public CreateProductCategoryCommandResponse(long categoryId, string guid)
            {
                CategoryId = categoryId;
                Guid = guid;
            }
        }
        //public async Task<Unit> Handle(CreateProductCategoryCommand request, CancellationToken cancellationToken)
        //{
        //   var category= await _productCategories.AddAsync(new ProductCategory { Name = request.Name, ParentId = request.ParentId });
        //    await _mediator.Publish(new ProductCategoryCreatedEvent(category.Entity.Id));
        //    return Unit.Task.Result;
        //}
    }
}
