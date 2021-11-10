// using Cloudware.Microservice.Product.DTO;
// using Cloudware.Microservice.Product.Infrastructure;
// using MediatR;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Options;
// using MongoDB.Driver;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;

// namespace Cloudware.Microservice.Product.Application.Query.Product
// {
//     public class GetListOfProductsByIdsQueryHandler : IRequestHandler<GetListOfProductsByIdsQuery, List<ProductItemsListCatalogDto>>
//     {
//         private readonly ProductReadDbContext _readDbContext;
//         public GetListOfProductsByIdsQueryHandler(IOptions<ProductSettings> settings)
//         {
//             
//         }
//         public async Task<List<ProductItemsListCatalogDto>> Handle(GetListOfProductsByIdsQuery request, CancellationToken cancellationToken)
//         {
//             var collection = _readDbContext.ProductItemsDataCollection;
//             var productsList =await collection.Find(w => request.ProductIds.Contains(w.ProductId))
//                 .Project(s=>new ProductItemsListCatalogDto {Id=s.ProductId,image=s.ImageUrl,Name=s.Name,Discount=s.StockItemsDto.Select(d=>d.Discount).FirstOrDefault(),Price=s.StockItemsDto.Select(p=>p.Price).FirstOrDefault()}).ToListAsync();
//             return productsList;
//           //  throw new NotImplementedException();
//         }
//     }
// }
