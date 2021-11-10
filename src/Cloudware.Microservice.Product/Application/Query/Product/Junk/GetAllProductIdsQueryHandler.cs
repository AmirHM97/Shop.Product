// using Cloudware.Microservice.Product.DTO;
// using Cloudware.Microservice.Product.Infrastructure;
// using Cloudware.Microservice.Product.Model;
// using MediatR;
// using Microsoft.EntityFrameworkCore;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;

// namespace Cloudware.Microservice.Product.Application.Query
// {
//     public class GetAllProductIdsQueryHandler : IRequestHandler<GetAllProductIdsQuery, List<long>>
//     {
//         private readonly IUnitOfWork _uow;
//         private readonly DbSet<ProductItem> _productItem;

//         public GetAllProductIdsQueryHandler(IUnitOfWork uow)
//         {
//             _uow = uow;
//             _productItem = _uow.Set<ProductItem>();
//         }

//         public async Task<List<long>> Handle(GetAllProductIdsQuery request, CancellationToken cancellationToken)
//         {
//             return await _productItem.OrderBy(o => o.Id).Select(s => s.Id).ToListAsync();

//         }
//     }
// }
