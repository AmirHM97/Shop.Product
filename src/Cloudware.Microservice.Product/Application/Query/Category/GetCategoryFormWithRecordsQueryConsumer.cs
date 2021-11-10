using System.Threading.Tasks;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Infrastructure.Services;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Formbuilder.Services;
using MassTransit;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Cloudware.Microservice.Product.Application.Query.Category
{
    public class GetCategoryFormWithRecordsQueryConsumer : IConsumer<GetCategoryFormWithRecordsQuery>, IMediatorConsumerType
    {
        private readonly IRecordsService _recordsService;
        private readonly ICategoryService _categoryService;
        private readonly IProductReadDbContext _productReadDbContext;

        public GetCategoryFormWithRecordsQueryConsumer(IRecordsService recordsService, ICategoryService categoryService, IProductReadDbContext productReadDbContext)
        {
            _recordsService = recordsService;
            _categoryService = categoryService;
            _productReadDbContext = productReadDbContext;
        }

        public async Task Consume(ConsumeContext<GetCategoryFormWithRecordsQuery> context)
        {
            var category = await _categoryService.GetOneAsync(context.Message.CategoryId, context.Message.TenantId);
            if (string.IsNullOrEmpty(category.FormId))
            {
                await context.RespondAsync(new GetCategoryFormWithRecordsQueryResponse(new()));
            }
            else
            {

                var res = await _recordsService.GetFormWithRecordWithRecordId(context.Message.TenantId, context.Message.RecordId, category.FormId);
                await context.RespondAsync(new GetCategoryFormWithRecordsQueryResponse(res));
            }
        }
    }
    public class GetCategoryFormWithRecordsQueryResponse
    {
        public FormWithRecord Form { get; set; }

        public GetCategoryFormWithRecordsQueryResponse(FormWithRecord form)
        {
            Form = form;
        }
    }
}

//  if (string.IsNullOrEmpty(category.FormId))
//           {
//               var a=@" {""id"": ""615d5805434073fac53e1fd9"",
//     ""recordId"": null,
//     ""title"": null,
//     ""description"": ""string"",
//     ""icon"": ""string"",
//     ""type"": ""Editable"",
//     ""formData"": [
//       {
//         ""id"": ""cf6fddf9-b2da-41f3-b941-d480c9e2685c"",
//         ""title"": ""نمایشگر"",
//         ""order"": 1,
//         ""sectionData"": [
//           {
//             ""cols"": 1,
//             ""components"": [
//               {
//                 ""id"": ""c00f96f0-c2ad-4699-b2a4-64d300fe2fd8"",
//                 ""name"": ""CPU"",
//                 ""order"": 0,
//                 ""type"": ""Radio"",
//                 ""attributes"": {
//                   ""title"": ""CPU"",
//                   ""placeholder"": ""CPU"",
//                   ""sideColor"": null,
//                   ""backGroundcColor"": null,
//                   ""color"": null,
//                   ""selectedValue"": """",
//                   ""isRequired"": true,
//                   ""isHidden"": false,
//                   ""componentData"": [
//                     {
//                       ""id"": ""4e6b2d9a-f409-4f7c-a706-2518badeaa6f"",
//                       ""name"": ""Intel core i9""
//                     },
//                     {
//                       ""id"": ""d9d9f67d-4ec9-4c9a-b418-c8157c6702b3"",
//                       ""name"": ""Intel core i7""
//                     },
//                     {
//                       ""id"": ""e157f190-6517-46b2-84b9-e225c659811e"",
//                       ""name"": ""Intel core i5""
//                     }
//                   ],
//                   ""searchedData"": []
//                 },
//                 ""relations"": null
//               }
//             ]
//           },
//           {
//             ""cols"": 1,
//             ""components"": [
//               {
//                 ""id"": ""14ff1646-34dd-4e03-9316-f7155d92c6a2"",
//                 ""name"": ""Text"",
//                 ""order"": 1,
//                 ""type"": ""BaseText"",
//                 ""attributes"": {
//                   ""title"": ""Text"",
//                   ""placeholder"": ""Text"",
//                   ""sideColor"": null,
//                   ""backGroundcColor"": null,
//                   ""color"": null,
//                   ""selectedValue"": """",
//                   ""isRequired"": true,
//                   ""isHidden"": false,
//                   ""componentData"": [],
//                   ""searchedData"": []
//                 },
//                 ""relations"": null
//               }
//             ]
//           },
//           {
//             ""cols"": 1,
//             ""components"": [
//               {
//                 ""id"": ""d15466e2-eea0-4068-9c6c-b5ddd981d875"",
//                 ""name"": ""TouchBar"",
//                 ""order"": 2,
//                 ""type"": ""CheckBox"",
//                 ""attributes"": {
//                   ""title"": ""TouchBar"",
//                   ""placeholder"": ""TouchBar"",
//                   ""sideColor"": null,
//                   ""backGroundcColor"": null,
//                   ""color"": null,
//                   ""selectedValue"": """",
//                   ""isRequired"": true,
//                   ""isHidden"": false,
//                   ""componentData"": [],
//                   ""searchedData"": []
//                 },
//                 ""relations"": null
//               }
//             ]
//           }
//         ]
//       },
//       {
//         ""id"": ""efd054fe-7682-40b9-a3d8-272196f250c2"",
//         ""title"": ""سی پی یو"",
//         ""order"": 2,
//         ""sectionData"": [
//           {
//             ""cols"": 1,
//             ""components"": [
//               {
//                 ""id"": ""f88ebb7b-87d1-41a1-8fac-2de63194800f"",
//                 ""name"": ""CPU"",
//                 ""order"": 3,
//                 ""type"": ""Radio"",
//                 ""attributes"": {
//                   ""title"": ""CPU"",
//                   ""placeholder"": ""CPU"",
//                   ""sideColor"": null,
//                   ""backGroundcColor"": null,
//                   ""color"": null,
//                   ""selectedValue"": """",
//                   ""isRequired"": true,
//                   ""isHidden"": false,
//                   ""componentData"": [
//                     {
//                       ""id"": ""d6f04b24-19f8-4779-8fb0-67016494f97a"",
//                       ""name"": ""Intel core i9""
//                     },
//                     {
//                       ""id"": ""8f627c7d-934c-4a57-8c71-b048e55564fd"",
//                       ""name"": ""Intel core i7""
//                     },
//                     {
//                       ""id"": ""9e216796-f86a-4233-8734-18bfc001ce00"",
//                       ""name"": ""Intel core i5""
//                     }
//                   ],
//                   ""searchedData"": []
//                 },
//                 ""relations"": null
//               }
//             ]
//           },
//           {
//             ""cols"": 1,
//             ""components"": [
//               {
//                 ""id"": ""fe7f34e7-8d9c-4808-91cb-bcb34ef89e4a"",
//                 ""name"": ""Text"",
//                 ""order"": 4,
//                 ""type"": ""BaseText"",
//                 ""attributes"": {
//                   ""title"": ""Text"",
//                   ""placeholder"": ""Text"",
//                   ""sideColor"": null,
//                   ""backGroundcColor"": null,
//                   ""color"": null,
//                   ""selectedValue"": """",
//                   ""isRequired"": true,
//                   ""isHidden"": false,
//                   ""componentData"": [],
//                   ""searchedData"": []
//                 },
//                 ""relations"": null
//               }
//             ]
//           },
//           {
//             ""cols"": 1,
//             ""components"": [
//               {
//                 ""id"": ""959fef4e-2ee1-4857-846a-0d4b3cc936ae"",
//                 ""name"": ""TouchBar"",
//                 ""order"": 5,
//                 ""type"": ""CheckBox"",
//                 ""attributes"": {
//                   ""title"": ""TouchBar"",
//                   ""placeholder"": ""TouchBar"",
//                   ""sideColor"": null,
//                   ""backGroundcColor"": null,
//                   ""color"": null,
//                   ""selectedValue"": """",
//                   ""isRequired"": true,
//                   ""isHidden"": false,
//                   ""componentData"": [],
//                   ""searchedData"": []
//                 },
//                 ""relations"": null
//               }
//             ]
//           }
//         ]
//       }
//     ],
//     ""defaultValues"": [
//       {
//         ""id"": ""c00f96f0-c2ad-4699-b2a4-64d300fe2fd8"",
//         ""value"": """"
//       },
//       {
//         ""id"": ""14ff1646-34dd-4e03-9316-f7155d92c6a2"",
//         ""value"": """"
//       },
//       {
//         ""id"": ""d15466e2-eea0-4068-9c6c-b5ddd981d875"",
//         ""value"": """"
//       },
//       {
//         ""id"": ""f88ebb7b-87d1-41a1-8fac-2de63194800f"",
//         ""value"": """"
//       },
//       {
//         ""id"": ""fe7f34e7-8d9c-4808-91cb-bcb34ef89e4a"",
//         ""value"": """"
//       },
//       {
//         ""id"": ""959fef4e-2ee1-4857-846a-0d4b3cc936ae"",
//         ""value"": """"
//       }
//     ]
//   }";
//              var form=JsonConvert.DeserializeObject<FormWithRecord>(a);
//               await context.RespondAsync(new GetCategoryFormWithRecordsQueryResponse(form));
//           }
//           else
//           {
//               var res = await _recordsService.GetFormWithRecordWithRecordId(context.Message.TenantId, null, category.FormId);
//               await context.RespondAsync(new GetCategoryFormWithRecordsQueryResponse(res));
//           }