

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudware.Microservice.Product.Application.Command;
using Cloudware.Microservice.Product.Application.Command.Category;
using Cloudware.Microservice.Product.Application.Command.Product;
using Cloudware.Microservice.Product.Application.Command.Property;
using Cloudware.Microservice.Product.Application.Command.Test;
using Cloudware.Microservice.Product.Application.Events;
using Cloudware.Microservice.Product.Application.Query.Category;
using Cloudware.Microservice.Product.DTO.Test;
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Infrastructure.Extensions;
using Cloudware.Microservice.Product.Model;
using Cloudware.Microservice.Product.Model.ReadDbModel;
using Cloudware.Utilities.Configure.Microservice;
using Cloudware.Utilities.Contract.Product;
using Cloudware.Utilities.Formbuilder.Entities;
using Cloudware.Utilities.Formbuilder.Services;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using src.Product;

namespace Cloudware.Microservice.Product.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ClwControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ProductContext _productContext;
        // private readonly DbSet<ProductCategory> _productCategories;
        private readonly DbSet<ProductItem> _productItems;
        private readonly DbSet<ImageUrlItems> _imageUrlItems;
        private readonly DbSet<ProductCategory> _productCategory;
        private readonly DbSet<ProductBrand> _productBrand;
        // private readonly DbSet<Property> _property;
        // private readonly DbSet<PropertyItem> _propertyItem;
        private readonly DbSet<StockItem> _stockItem;
        private readonly IRequestClient<GetProductForInvoiceQuery> requestClient;
        private readonly IMediator _mediator;
        private readonly IProductReadDbContext _productReadDbContext;
        private readonly IRecordsService _recordsService;
        private readonly IFormManagementService _formManagementService;

        public TestController(IUnitOfWork uow, IMediator mediator, IProductReadDbContext productReadDbContext, IRecordsService recordsService, IFormManagementService formManagementService, IRequestClient<GetProductForInvoiceQuery> requestClient, ProductContext productContext)
        {
            _uow = uow;
            _productItems = _uow.Set<ProductItem>();
            _imageUrlItems = _uow.Set<ImageUrlItems>();
            _productCategory = _uow.Set<ProductCategory>();
            _productBrand = _uow.Set<ProductBrand>();
            _stockItem = _uow.Set<StockItem>();
            // _property = _uow.Set<Property>();
            // _propertyItem = _uow.Set<PropertyItem>();
            _mediator = mediator;
            _productReadDbContext = productReadDbContext;
            _recordsService = recordsService;
            _formManagementService = formManagementService;
            this.requestClient = requestClient;
            _productContext = productContext;
        }
        [HttpGet]
        public async Task<ActionResult> UpdateGuid()
        {
            await _mediator.Publish(new UpdateProductGuidPropertyCommand());
            return Ok();
        }
        [HttpGet]
        public async Task<ActionResult> UpdateCategoryGuid()
        {
            await _mediator.Publish(new UpdateCategoryGuidCommand());
            return Ok();
        }
        [HttpGet]
        public async Task<ActionResult> UpdateCategory()
        {
            await _mediator.Publish(new ProductCategoryCreatedEvent("364cbb18-b044-4339-b5c9-de772c2f011d"));
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult> UpdateAvailability()
        {
            var co = _productReadDbContext.ProductItemsDataCollection;
            var p = await co.Find(_ => true).ToListAsync();
            foreach (var item in p)
            {
                item.StockItemsDto = item.StockItemsDto.Select(s => { s.IsAvailable = true; return s; }).ToList();

                await co.ReplaceOneAsync(s => s.ProductId == item.ProductId, item);
            }
            return Ok();
        }
        [HttpGet]
        public async Task<ActionResult> testa(long id)
        {
            var product = await _productItems.FirstOrDefaultAsync(f => f.Id == id);
            var records = await _recordsService.GetRecordById(product.TechnicalPropertyRecordId, product.TenantId);
            var form = await _formManagementService.GetForm(product.TechnicalPropertyFormId);
            foreach (var item in records.RecordsData)
            {
                switch (item.FieldType)
                {
                    case FieldType.Textarea:
                        {
                            // technicalProperties.Add(new TechnicalPropertyReadDb
                            // {
                            //     Name = form.Groups.SelectMany(s => s.FormFields).Where(w => w.Id == item.FieldId).Select(s => s.DisplayName).FirstOrDefault(),
                            //     Value = item.Value,
                            // });
                        }
                        break;
                    case FieldType.CheckBox:
                        {
                            // technicalProperties.Add(new TechnicalPropertyReadDb
                            // {
                            //     Name = form.Groups.SelectMany(s => s.FormFields).Where(w => w.Id == item.FieldId).Select(s => s.DisplayName).FirstOrDefault(),
                            //     Value = item.Value == ""true"" ? ""دارد"" : ""ندارد"",
                            // });
                        }
                        break;
                    case FieldType.Radio:
                        {
                            var a = form.Groups.SelectMany(s => s.FormFields).ToList();
                            // technicalProperties.Add(new TechnicalPropertyReadDb
                            // {
                            //     Name = form.Groups.SelectMany(s => s.FormFields).Where(w => w.Id == item.FieldId).Select(s => s.DisplayName).FirstOrDefault(),
                            //     Value = form.Groups.SelectMany(s => s.FormFields).Where(w => w.Id == item.FieldId).SelectMany(s => s.SelectionRow).Where(w => w.Id == item.SelectionIds.FirstOrDefault()).Select(s => s.Value).FirstOrDefault(),
                            // });
                        }
                        break;
                        // default: break;
                }
            }
            return Ok();
        }
        [HttpPost]
        public async Task<ActionResult> Get(long productId, long stockId)
        {
            await _mediator.Publish(new UpdateReadDbCategoryCommand("364cbb18-b044-4339-b5c9-de772c2f011d"));
            // var res = await requestClient.GetResponse<GetProductForInvoiceQueryResponse>(new GetProductForInvoiceQuery(
            //       new List<GetProductForInvoiceQueryItem>
            //     {
            //         new GetProductForInvoiceQueryItem{ProductId=5544,StockId=7883},
            //         new GetProductForInvoiceQueryItem{ProductId=5552,StockId=7912},
            //     }
            // ));
            return Ok();
        }
        [HttpGet]
        public async Task<ActionResult> testNew()
        {
            // var t = new List<ProductCategory2>{
            //          new ProductCategory2{Id="1",Name="GAG",ParentId="5"}
            //                      };
            // foreach (var item in t)
            // {
            //     item.Id = "54";
            // }
            await _mediator.Publish(new UpdateAllPropertiesGuidCommand());
            return Ok();
        }
        [HttpPost]
        public async Task<ActionResult> TestEditProductCategory(TestEditProductCategoryDto testEditProductCategoryDto)
        {
            await _mediator.Publish(new TestEditProductCategoryCommand(testEditProductCategoryDto.Ids, testEditProductCategoryDto.CategoryId));
            return Ok();
        }
        [HttpGet]
        public async Task<ActionResult> UpdatePropsTenant()
        {
            await _mediator.Publish(new UpdatePropertiesTenantIdCommand("364cbb18-b044-4339-b5c9-de772c2f011d"));
            return Ok();
        }
        [HttpGet]
        public async Task<ActionResult> GetCategoryIds(string tenant, [FromQuery] List<long> catId)
        {


            //var findFluent =await _productReadDbContext.ProductCategoriesDataCollection.Find(Builders<ProductCategoryReadDbCollection>.Filter.ElemMatch(
            //     foo => foo.ProductCategories,
            // foobar => foobar.CategoryId == catId.FirstOrDefault())).FirstOrDefaultAsync();
            // var cats = await _productReadDbContext.ProductCategoriesDataCollection.Find(f => f.ProductCategories.a(s => s.CategoryId == catId.FirstOrDefault())).Project(p => p.ProductCategories).FirstOrDefaultAsync();

            var req = _mediator.CreateRequestClient<GetCategoryChildrenIdQuery>();
            var res = await req.GetResponse<GetCategoryChildrenIdQueryResponse>(new GetCategoryChildrenIdQuery(catId, tenant));


            var collection = _productReadDbContext.ProductCategoriesDataCollection.AsQueryable();
            var rep = await _productReadDbContext.ProductItemsDataCollection.Find(f => res.Message.CategoryIds.Contains(f.CategoryId)).ToListAsync();
            // / var cats = await collection.Where(w => w.ClientId == tenant).FirstOrDefaultAsync();
            // var av = cats.ProductCategories.FirstOrDefault(w => w.CategoryId == catId);
            // var a = ProductExtensions.GetCategoriesChildrenIdsRead(av, tenant, catId);
            return Ok(rep);
        }

        [HttpGet]
        public async Task<ActionResult> AddSeed(string tenant)
        {
            var data = @"[
        {
            ""Id"": ""1"",
            ""Name"": ""ای کالا"",
            ""parentId"": ""0""
        },
        {
            ""Id"": ""7390"",
            ""Name"": ""موبایل و لوازم جانبی"",
            ""parentId"": ""0""
        },
        {
            ""Id"": ""7391"",
            ""Name"": ""موبایل"",
            ""parentId"": ""7390""
        },
        {
            ""Id"": ""7392"",
            ""Name"": ""لوازم جانبی"",
            ""parentId"": ""7390""
        },
        {
            ""Id"": ""7394"",
            ""Name"": ""سامسونگ"",
            ""parentId"": ""7391""
        },
        {
            ""Id"": ""7395"",
            ""Name"": ""شیائومی"",
            ""parentId"": ""7391""
        },
        {
            ""Id"": ""7396"",
            ""Name"": ""جی پلاس"",
            ""parentId"": ""7391""
        },
        {
            ""Id"": ""7397"",
            ""Name"": ""هوآوی"",
            ""parentId"": ""7391""
        },
        {
            ""Id"": ""7398"",
            ""Name"": ""موتورلا"",
            ""parentId"": ""7391""
        },
        {
            ""Id"": ""7399"",
            ""Name"": ""بلک بری"",
            ""parentId"": ""7391""
        },
        {
            ""Id"": ""7400"",
            ""Name"": ""کامپیوتر، لب تاپ، تبلت و لوازم جانبی"",
            ""parentId"": ""0""
        },
        {
            ""Id"": ""7401"",
            ""Name"": ""کامپیوتر"",
            ""parentId"": ""7400""
        },
        {
            ""Id"": ""7402"",
            ""Name"": ""تبلت"",
            ""parentId"": ""7400""
        },
        {
            ""Id"": ""7403"",
            ""Name"": ""لپ تاپ"",
            ""parentId"": ""7400""
        },
        {
            ""Id"": ""7404"",
            ""Name"": ""لوازم جانبی کامپیوتر و لپ تاپ"",
            ""parentId"": ""7400""
        },
        {
            ""Id"": ""7405"",
            ""Name"": ""لوازم التحریر و لوازم اداری"",
            ""parentId"": ""0""
        },
        {
            ""Id"": ""7406"",
            ""Name"": ""ملزومات اداری"",
            ""parentId"": ""7405""
        },
        {
            ""Id"": ""7407"",
            ""Name"": ""ماشین های اداری"",
            ""parentId"": ""7405""
        },
        {
            ""Id"": ""7408"",
            ""Name"": ""لوازم خانگی"",
            ""parentId"": ""0""
        },
        {
            ""Id"": ""7410"",
            ""Name"": ""صوتی و تصویری"",
            ""parentId"": ""7408""
        },
        {
            ""Id"": ""7411"",
            ""Name"": ""لوازم برقی خانه و آشپزخانه"",
            ""parentId"": ""7408""
        },
        {
            ""Id"": ""7412"",
            ""Name"": ""اجاق گاز"",
            ""parentId"": ""7408""
        },
        {
            ""Id"": ""7413"",
            ""Name"": ""تهیه و نگهداری نوشیدنی"",
            ""parentId"": ""7408""
        },
        {
            ""Id"": ""7414"",
            ""Name"": ""صنایع دستی"",
            ""parentId"": ""0""
        },
        {
            ""Id"": ""7415"",
            ""Name"": ""فرش دستباف"",
            ""parentId"": ""7414""
        },
        {
            ""Id"": ""7416"",
            ""Name"": ""گلیم"",
            ""parentId"": ""7414""
        },
        {
            ""Id"": ""7417"",
            ""Name"": ""تابلو فرش"",
            ""parentId"": ""7414""
        },
        {
            ""Id"": ""7418"",
            ""Name"": ""بهداشتی و آرایشی"",
            ""parentId"": ""0""
        },
        {
            ""Id"": ""7419"",
            ""Name"": ""بهداشتی"",
            ""parentId"": ""7418""
        },
        {
            ""Id"": ""7420"",
            ""Name"": ""آرایشی"",
            ""parentId"": ""7418""
        },
        {
            ""Id"": ""7421"",
            ""Name"": ""پوشاک و کفش"",
            ""parentId"": ""0""
        },
        {
            ""Id"": ""7422"",
            ""Name"": ""پوشاک"",
            ""parentId"": ""7421""
        },
        {
            ""Id"": ""7423"",
            ""Name"": ""کیف، کفش و چرم"",
            ""parentId"": ""7421""
        },
        {
            ""Id"": ""7424"",
            ""Name"": ""لباس مردانه"",
            ""parentId"": ""7422""
        },
        {
            ""Id"": ""7425"",
            ""Name"": ""لباس زنانه"",
            ""parentId"": ""7422""
        },
        {
            ""Id"": ""7426"",
            ""Name"": ""لباس بچه گانه"",
            ""parentId"": ""7422""
        },
        {
            ""Id"": ""7427"",
            ""Name"": ""کیف"",
            ""parentId"": ""7423""
        },
        {
            ""Id"": ""7428"",
            ""Name"": ""کفش"",
            ""parentId"": ""7423""
        },
        {
            ""Id"": ""7429"",
            ""Name"": ""ورزش و سرگرمی"",
            ""parentId"": ""0""
        },
        {
            ""Id"": ""7430"",
            ""Name"": ""تجهیزات ورزشی"",
            ""parentId"": ""7429""
        },
        {
            ""Id"": ""7431"",
            ""Name"": ""پوشاک ورزشی"",
            ""parentId"": ""7429""
        },
        {
            ""Id"": ""7432"",
            ""Name"": ""کیف و کفش ورزشی"",
            ""parentId"": ""7429""
        },
        {
            ""Id"": ""7433"",
            ""Name"": ""اسباب بازی و سرگرمی"",
            ""parentId"": ""7429""
        },
        {
            ""Id"": ""7435"",
            ""Name"": ""مبلمان اداری"",
            ""parentId"": ""7405""
        },
        {
            ""Id"": ""7436"",
            ""Name"": ""میز اداری و میز کنفرانس"",
            ""parentId"": ""7435""
        },
        {
            ""Id"": ""7437"",
            ""Name"": ""فایل و کتابخانه"",
            ""parentId"": ""7435""
        },
        {
            ""Id"": ""7442"",
            ""Name"": ""گیرنده دیجیتال تلویزیون"",
            ""parentId"": ""7410""
        },
        {
            ""Id"": ""7449"",
            ""Name"": ""رادیو"",
            ""parentId"": ""7410""
        },
        {
            ""Id"": ""7454"",
            ""Name"": ""هواپز"",
            ""parentId"": ""7411""
        },
        {
            ""Id"": ""7455"",
            ""Name"": ""چرخ خیاطی"",
            ""parentId"": ""7411""
        },
        {
            ""Id"": ""7456"",
            ""Name"": ""زودپز"",
            ""parentId"": ""7411""
        },
        {
            ""Id"": ""7457"",
            ""Name"": ""توستر"",
            ""parentId"": ""7411""
        },
        {
            ""Id"": ""7458"",
            ""Name"": ""سرخ کن"",
            ""parentId"": ""7411""
        },
        {
            ""Id"": ""7461"",
            ""Name"": ""اجاق گاز مبله"",
            ""parentId"": ""7412""
        },
        {
            ""Id"": ""7463"",
            ""Name"": ""اجاق گاز صفحه ای و رومیزی"",
            ""parentId"": ""7412""
        },
        {
            ""Id"": ""7465"",
            ""Name"": ""آبمیوه گیری"",
            ""parentId"": ""7413""
        },
        {
            ""Id"": ""7466"",
            ""Name"": ""آب مرکبات گیری"",
            ""parentId"": ""7413""
        },
        {
            ""Id"": ""7467"",
            ""Name"": ""چای ساز"",
            ""parentId"": ""7413""
        },
        {
            ""Id"": ""7468"",
            ""Name"": ""قهوه ساز"",
            ""parentId"": ""7413""
        },
        {
            ""Id"": ""7469"",
            ""Name"": ""اسپرسوساز"",
            ""parentId"": ""7413""
        },
        {
            ""Id"": ""7470"",
            ""Name"": ""آب سرد کن"",
            ""parentId"": ""7413""
        },
        {
            ""Id"": ""7471"",
            ""Name"": ""مخلوط کن"",
            ""parentId"": ""7413""
        },
        {
            ""Id"": ""7473"",
            ""Name"": ""سماور"",
            ""parentId"": ""7413""
        },
        {
            ""Id"": ""7474"",
            ""Name"": ""لوازم آماده سازی غذا"",
            ""parentId"": ""7408""
        },
        {
            ""Id"": ""7475"",
            ""Name"": ""غذاساز"",
            ""parentId"": ""7474""
        },
        {
            ""Id"": ""7476"",
            ""Name"": ""خردکن"",
            ""parentId"": ""7474""
        },
        {
            ""Id"": ""7477"",
            ""Name"": ""چرخ گوشت"",
            ""parentId"": ""7474""
        },
        {
            ""Id"": ""7478"",
            ""Name"": ""گوشت کوب برقی"",
            ""parentId"": ""7474""
        },
        {
            ""Id"": ""7479"",
            ""Name"": ""آسیاب"",
            ""parentId"": ""7474""
        },
        {
            ""Id"": ""7481"",
            ""Name"": ""همزن"",
            ""parentId"": ""7474""
        },
        {
            ""Id"": ""7483"",
            ""Name"": ""شستشو و نظافت"",
            ""parentId"": ""7408""
        },
        {
            ""Id"": ""7484"",
            ""Name"": ""جاروبرقی"",
            ""parentId"": ""7483""
        },
        {
            ""Id"": ""7485"",
            ""Name"": ""جاروشارژی"",
            ""parentId"": ""7483""
        },
        {
            ""Id"": ""7486"",
            ""Name"": ""اتوبخار"",
            ""parentId"": ""7483""
        },
        {
            ""Id"": ""7487"",
            ""Name"": ""اتوپرس"",
            ""parentId"": ""7483""
        },
        {
            ""Id"": ""7488"",
            ""Name"": ""بخارشوی"",
            ""parentId"": ""7483""
        },
        {
            ""Id"": ""7489"",
            ""Name"": ""ماشین لباسشویی"",
            ""parentId"": ""7483""
        },
        {
            ""Id"": ""7491"",
            ""Name"": ""سرمایشی و گرمایشی"",
            ""parentId"": ""7408""
        },
        {
            ""Id"": ""7492"",
            ""Name"": ""شوفاژ برقی"",
            ""parentId"": ""7491""
        },
        {
            ""Id"": ""7493"",
            ""Name"": ""کولرآبی"",
            ""parentId"": ""7491""
        },
        {
            ""Id"": ""7494"",
            ""Name"": ""هیتربرقی-فن هیتر"",
            ""parentId"": ""7491""
        },
        {
            ""Id"": ""7495"",
            ""Name"": ""کولرگازی"",
            ""parentId"": ""7491""
        },
        {
            ""Id"": ""7496"",
            ""Name"": ""پنکه"",
            ""parentId"": ""7491""
        },
        {
            ""Id"": ""7501"",
            ""Name"": ""آیفون"",
            ""parentId"": ""7391""
        },
        {
            ""Id"": ""7502"",
            ""Name"": ""فکس"",
            ""parentId"": ""7407""
        },
        {
            ""Id"": ""7503"",
            ""Name"": ""رول فکس"",
            ""parentId"": ""7407""
        },
        {
            ""Id"": ""7504"",
            ""Name"": ""پرینتر"",
            ""parentId"": ""7407""
        },
        {
            ""Id"": ""7505"",
            ""Name"": ""کارتریج"",
            ""parentId"": ""7407""
        },
        {
            ""Id"": ""7506"",
            ""Name"": ""دستگاه کپی"",
            ""parentId"": ""7407""
        },
        {
            ""Id"": ""7507"",
            ""Name"": ""کاغذ خرد کن"",
            ""parentId"": ""7407""
        },
        {
            ""Id"": ""7508"",
            ""Name"": ""ماشین حساب"",
            ""parentId"": ""7407""
        },
        {
            ""Id"": ""7509"",
            ""Name"": ""میزو صندلی تحریر"",
            ""parentId"": ""7435""
        },
        {
            ""Id"": ""7510"",
            ""Name"": ""مبل اداری"",
            ""parentId"": ""7435""
        },
        {
            ""Id"": ""7511"",
            ""Name"": ""صندلی اداری"",
            ""parentId"": ""7435""
        },
        {
            ""Id"": ""7512"",
            ""Name"": ""چراغ مطالعه"",
            ""parentId"": ""7435""
        },
        {
            ""Id"": ""7513"",
            ""Name"": ""نوشت افزار"",
            ""parentId"": ""7405""
        },
        {
            ""Id"": ""7514"",
            ""Name"": ""کاغذ و پاکت"",
            ""parentId"": ""7405""
        },
        {
            ""Id"": ""7515"",
            ""Name"": ""ماژیک"",
            ""parentId"": ""7513""
        },
        {
            ""Id"": ""7516"",
            ""Name"": ""تراش"",
            ""parentId"": ""7513""
        },
        {
            ""Id"": ""7517"",
            ""Name"": ""پاکن و غلط گیر"",
            ""parentId"": ""7513""
        },
        {
            ""Id"": ""7518"",
            ""Name"": ""اتود و نوک"",
            ""parentId"": ""7513""
        },
        {
            ""Id"": ""7519"",
            ""Name"": ""خودنویس"",
            ""parentId"": ""7513""
        },
        {
            ""Id"": ""7520"",
            ""Name"": ""جوهر و سایر اقلام"",
            ""parentId"": ""7513""
        },
        {
            ""Id"": ""7521"",
            ""Name"": ""گچ"",
            ""parentId"": ""7513""
        },
        {
            ""Id"": ""7522"",
            ""Name"": ""نشانگر کتاب"",
            ""parentId"": ""7513""
        },
        {
            ""Id"": ""7523"",
            ""Name"": ""مداد و مداد رنگی"",
            ""parentId"": ""7513""
        },
        {
            ""Id"": ""7524"",
            ""Name"": ""خودکار و روان نویس"",
            ""parentId"": ""7513""
        },
        {
            ""Id"": ""7525"",
            ""Name"": ""دفاتر مدرسه ایی"",
            ""parentId"": ""7513""
        },
        {
            ""Id"": ""7526"",
            ""Name"": ""دفاتر اداری"",
            ""parentId"": ""7513""
        },
        {
            ""Id"": ""7527"",
            ""Name"": ""جامدادی"",
            ""parentId"": ""7513""
        },
        {
            ""Id"": ""7528"",
            ""Name"": ""کیف و کوله"",
            ""parentId"": ""7513""
        },
        {
            ""Id"": ""7529"",
            ""Name"": ""کاغذ"",
            ""parentId"": ""7514""
        },
        {
            ""Id"": ""7530"",
            ""Name"": ""پاکت"",
            ""parentId"": ""7514""
        },
        {
            ""Id"": ""7531"",
            ""Name"": ""ملزومات اداری"",
            ""parentId"": ""7405""
        },
        {
            ""Id"": ""7532"",
            ""Name"": ""ست اداری رو میزی"",
            ""parentId"": ""7531""
        },
        {
            ""Id"": ""7533"",
            ""Name"": ""تخته وایت برد"",
            ""parentId"": ""7531""
        },
        {
            ""Id"": ""7534"",
            ""Name"": ""ابزار بایگانی"",
            ""parentId"": ""7531""
        },
        {
            ""Id"": ""7535"",
            ""Name"": ""سطل زباله اداری"",
            ""parentId"": ""7531""
        },
        {
            ""Id"": ""7536"",
            ""Name"": ""قیچی و کاتر"",
            ""parentId"": ""7531""
        },
        {
            ""Id"": ""7537"",
            ""Name"": ""چسب"",
            ""parentId"": ""7531""
        },
        {
            ""Id"": ""7538"",
            ""Name"": ""مهر و استمپ"",
            ""parentId"": ""7531""
        },
        {
            ""Id"": ""7539"",
            ""Name"": ""پلنر و سالنامه"",
            ""parentId"": ""7531""
        },
        {
            ""Id"": ""7540"",
            ""Name"": ""تلویزیون"",
            ""parentId"": ""7410""
        },
        {
            ""Id"": ""7543"",
            ""Name"": ""ساندویچ ساز"",
            ""parentId"": ""7413""
        },
        {
            ""Id"": ""7544"",
            ""Name"": ""تخم مرغ پز"",
            ""parentId"": ""7411""
        },
        {
            ""Id"": ""7545"",
            ""Name"": ""زودپز برقی"",
            ""parentId"": ""7456""
        },
        {
            ""Id"": ""7546"",
            ""Name"": ""زودپز"",
            ""parentId"": ""7456""
        },
        {
            ""Id"": ""7547"",
            ""Name"": ""آون توستر"",
            ""parentId"": ""7411""
        },
        {
            ""Id"": ""7548"",
            ""Name"": ""یخچال و فریزر"",
            ""parentId"": ""7411""
        },
        {
            ""Id"": ""7551"",
            ""Name"": ""سینمای خانگی و ساندبار"",
            ""parentId"": ""7410""
        },
        {
            ""Id"": ""7552"",
            ""Name"": ""سماور گازی"",
            ""parentId"": ""7473""
        },
        {
            ""Id"": ""7553"",
            ""Name"": ""سماور برقی"",
            ""parentId"": ""7473""
        },
        {
            ""Id"": ""7554"",
            ""Name"": ""ماشین ظرفشویی"",
            ""parentId"": ""7483""
        },
        {
            ""Id"": ""7555"",
            ""Name"": ""مایکروویو-مایکروفر سولاردام"",
            ""parentId"": ""7411""
        },
        {
            ""Id"": ""7556"",
            ""Name"": ""پلوپز و مولتی کوکر"",
            ""parentId"": ""7411""
        },
        {
            ""Id"": ""7557"",
            ""Name"": ""منگنه و سوزن منگنه"",
            ""parentId"": ""7531""
        },
        {
            ""Id"": ""7558"",
            ""Name"": ""بخاری برقی"",
            ""parentId"": ""7491""
        },
        {
            ""Id"": ""7559"",
            ""Name"": ""هدفون,هدست و هندزفری"",
            ""parentId"": ""7392""
        },
        {
            ""Id"": ""7560"",
            ""Name"": ""پاوربانک (شارژر همراه )"",
            ""parentId"": ""7392""
        },
        {
            ""Id"": ""7562"",
            ""Name"": ""کابل و مبدل"",
            ""parentId"": ""7392""
        },
        {
            ""Id"": ""7564"",
            ""Name"": ""کیف و کاور گوشی"",
            ""parentId"": ""7392""
        },
        {
            ""Id"": ""7565"",
            ""Name"": ""باتری موبایل"",
            ""parentId"": ""7392""
        },
        {
            ""Id"": ""7566"",
            ""Name"": ""شارژر و آداپتور"",
            ""parentId"": ""7392""
        },
        {
            ""Id"": ""7567"",
            ""Name"": ""اسپیکر"",
            ""parentId"": ""7392""
        },
        {
            ""Id"": ""7568"",
            ""Name"": ""کارت حافظه"",
            ""parentId"": ""7392""
        },
        {
            ""Id"": ""7569"",
            ""Name"": ""دانگل"",
            ""parentId"": ""7392""
        },
        {
            ""Id"": ""7570"",
            ""Name"": ""پایه نگهدارنده"",
            ""parentId"": ""7392""
        },
        {
            ""Id"": ""7573"",
            ""Name"": ""لوازم جانبی تبلت"",
            ""parentId"": ""7400""
        },
        {
            ""Id"": ""7574"",
            ""Name"": ""کنسول بازی"",
            ""parentId"": ""0""
        },
        {
            ""Id"": ""7576"",
            ""Name"": ""پرگار و خط کش"",
            ""parentId"": ""7513""
        },
        {
            ""Id"": ""7577"",
            ""Name"": ""گوشی تلفن و سانترال"",
            ""parentId"": ""7407""
        },
        {
            ""Id"": ""7578"",
            ""Name"": ""مانیتور"",
            ""parentId"": ""7401""
        },
        {
            ""Id"": ""7579"",
            ""Name"": ""ماوس"",
            ""parentId"": ""7401""
        },
        {
            ""Id"": ""7580"",
            ""Name"": ""کیبورد"",
            ""parentId"": ""7401""
        },
        {
            ""Id"": ""7581"",
            ""Name"": ""لوازم جانبی گوشی تلفن و سانترال"",
            ""parentId"": ""7407""
        },
        {
            ""Id"": ""7583"",
            ""Name"": ""ظروف آشپزخانه"",
            ""parentId"": ""0""
        },
        {
            ""Id"": ""7584"",
            ""Name"": ""فر توکار"",
            ""parentId"": ""7411""
        },
        {
            ""Id"": ""7585"",
            ""Name"": ""رنده برقی"",
            ""parentId"": ""7474""
        },
        {
            ""Id"": ""7586"",
            ""Name"": ""ابزار نقاشی و طراحی"",
            ""parentId"": ""7405""
        },
        {
            ""Id"": ""7587"",
            ""Name"": ""قلم مو"",
            ""parentId"": ""7586""
        },
        {
            ""Id"": ""7588"",
            ""Name"": ""بوم نقاشی"",
            ""parentId"": ""7586""
        },
        {
            ""Id"": ""7589"",
            ""Name"": ""رنگ روغن"",
            ""parentId"": ""7586""
        },
        {
            ""Id"": ""7590"",
            ""Name"": ""سه پایه"",
            ""parentId"": ""7586""
        },
        {
            ""Id"": ""7591"",
            ""Name"": ""رنگ اکریلیک"",
            ""parentId"": ""7586""
        },
        {
            ""Id"": ""7592"",
            ""Name"": ""کاغذ طراحی"",
            ""parentId"": ""7586""
        },
        {
            ""Id"": ""7593"",
            ""Name"": ""پالت یکبار مصرف"",
            ""parentId"": ""7586""
        },
        {
            ""Id"": ""7594"",
            ""Name"": ""کاردک"",
            ""parentId"": ""7586""
        },
        {
            ""Id"": ""7595"",
            ""Name"": ""فلش مموری"",
            ""parentId"": ""7392""
        },
        {
            ""Id"": ""7596"",
            ""Name"": ""محو کن"",
            ""parentId"": ""7586""
        },
        {
            ""Id"": ""7600"",
            ""Name"": ""زیورآلات و اکسسوری"",
            ""parentId"": ""7414""
        },
        {
            ""Id"": ""7601"",
            ""Name"": ""زیورآلات"",
            ""parentId"": ""7600""
        },
        {
            ""Id"": ""7602"",
            ""Name"": ""اکسسوری"",
            ""parentId"": ""7600""
        },
        {
            ""Id"": ""7603"",
            ""Name"": ""پایه خنک کننده"",
            ""parentId"": ""7404""
        },
        {
            ""Id"": ""7604"",
            ""Name"": ""کیف تبلت"",
            ""parentId"": ""7573""
        },
        {
            ""Id"": ""7605"",
            ""Name"": ""سرویس پخت و پز( قابله - تابه )"",
            ""parentId"": ""7583""
        },
        {
            ""Id"": ""7606"",
            ""Name"": ""محافظ صفحه نمایش"",
            ""parentId"": ""7392""
        },
        {
            ""Id"": ""7607"",
            ""Name"": ""محافظ صفحه تبلت"",
            ""parentId"": ""7573""
        },
        {
            ""Id"": ""7608"",
            ""Name"": ""ویدئو پرژکتور"",
            ""parentId"": ""7390""
        },
        {
            ""Id"": ""7609"",
            ""Name"": ""چرم"",
            ""parentId"": ""7421""
        },
        {
            ""Id"": ""7610"",
            ""Name"": ""چرم مردانه"",
            ""parentId"": ""7609""
        },
        {
            ""Id"": ""7611"",
            ""Name"": ""چرم زنانه"",
            ""parentId"": ""7609""
        },
        {
            ""Id"": ""7612"",
            ""Name"": ""چرم بچه گانه"",
            ""parentId"": ""7609""
        },
        {
            ""Id"": ""7613"",
            ""Name"": ""ساعت و مچ بند هوشمند"",
            ""parentId"": ""7392""
        },
        {
            ""Id"": ""7614"",
            ""Name"": ""ویدئو پروژکتور"",
            ""parentId"": ""7407""
        },
        {
            ""Id"": ""7615"",
            ""Name"": ""پایه و پرده ویدئو پروژکتور"",
            ""parentId"": ""7407""
        },
        {
            ""Id"": ""7616"",
            ""Name"": ""پاستل و مداد شمعی"",
            ""parentId"": ""7586""
        },
        {
            ""Id"": ""7617"",
            ""Name"": ""گواش"",
            ""parentId"": ""7586""
        },
        {
            ""Id"": ""7618"",
            ""Name"": ""رنگ انگشتی"",
            ""parentId"": ""7586""
        },
        {
            ""Id"": ""7619"",
            ""Name"": ""آبرنگ"",
            ""parentId"": ""7586""
        },
        {
            ""Id"": ""7620"",
            ""Name"": ""رنگ صورت"",
            ""parentId"": ""7586""
        },
        {
            ""Id"": ""7621"",
            ""Name"": ""خمیر بازی"",
            ""parentId"": ""7586""
        },
        {
            ""Id"": ""7622"",
            ""Name"": ""خمیر شنی"",
            ""parentId"": ""7586""
        },
        {
            ""Id"": ""7623"",
            ""Name"": ""کاور مهدکودک"",
            ""parentId"": ""7586""
        },
        {
            ""Id"": ""7624"",
            ""Name"": ""ظرف غذای کودک"",
            ""parentId"": ""7513""
        }
    ]";
            var conv = JsonConvert.DeserializeObject<List<ProductCategory2>>(data);
            ICollection<ProductCategory> c = conv.Select(s => new ProductCategory
            {
                Id = Int64.Parse(s.Id),
                Guid = Guid.NewGuid(),
                Name = s.Name,
                ParentId = Int64.Parse(s.ParentId)
            }).ToList();

            var a = ProductExtensions.GetCategoriesChildrenForSeed(c, 0, tenant);
            using (var context = _productContext)
            {
                try
                {
                    context.ChangeTracker.AutoDetectChangesEnabled = false;

                    // Make many calls in a loop
                    await context.ProductCategories.AddRangeAsync(a);
                    await context.SaveChangesAsync();
                }
                finally
                {
                    context.ChangeTracker.AutoDetectChangesEnabled = true;
                }
            }
            // try
            // {
            //     await _productCategories.AddRangeAsync(a);
            //     await _uow.SaveChangesAsync();
            // }
            // catch (System.Exception ex)
            // {
            //     // TODO
            // }
            await _mediator.Publish(new ProductCategoryCreatedEvent(tenant));
            return Ok();
        }
        [HttpGet]
        public async Task<ActionResult> AddSeedBrand(string tenant)
        {
            var d = @" [
        {
            ""Id"": ""1"",
            ""Name"": ""ای کالا""
        }]";
            var conv = JsonConvert.DeserializeObject<List<br>>(d);
            ICollection<ProductBrand> c = conv.Select(s => new ProductBrand
            {
                // Id = Int64.Parse(s.Id),
                Guid = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedDate = DateTimeOffset.UtcNow,
                IsDeleted = false,
                TenantId = tenant,
                Name = s.Name,
            }).ToList();

            using (var context = _productContext)
            {
                try
                {
                    context.ChangeTracker.AutoDetectChangesEnabled = false;

                    // Make many calls in a loop
                    await context.ProductBrands.AddRangeAsync(c);
                    await context.SaveChangesAsync();
                }
                finally
                {
                    context.ChangeTracker.AutoDetectChangesEnabled = true;
                }
            }
            return Ok();
        }
        //  [HttpGet]
        //     {
        //         var data = @""[
        //     {
        //         """"Id"""": """"1"""",
        //         """"ParentId"""": """"0"""",
        //         """"Name"""": """"موادغذایی""""
        //     },
        //     {
        //         """"Id"""": """"2"""",
        //         """"ParentId"""": """"0"""",
        //         """"Name"""": """"صنایع دستی""""
        //     },
        //     {
        //         """"Id"""": """"3"""",
        //         """"ParentId"""": """"0"""",
        //         """"Name"""": """"مد و پوشاک""""
        //     },
        //     {
        //         """"Id"""": """"4"""",
        //         """"ParentId"""": """"0"""",
        //         """"Name"""": """"زیبایی و سلامت""""
        //     },
        //     {
        //         """"Id"""": """"5"""",
        //         """"ParentId"""": """"0"""",
        //         """"Name"""": """"عطاری""""
        //     },
        //     {
        //         """"Id"""": """"6"""",
        //         """"ParentId"""": """"0"""",
        //         """"Name"""": """"کالای دیجیتال""""
        //     },
        //     {
        //         """"Id"""": """"7"""",
        //         """"ParentId"""": """"0"""",
        //         """"Name"""": """"فرهنگی ،سرگرمی""""
        //     },
        //     {
        //         """"Id"""": """"8"""",
        //         """"ParentId"""": """"0"""",
        //         """"Name"""": """"خانه و آشپزخانه""""
        //     },
        //     {
        //         """"Id"""": """"9"""",
        //         """"ParentId"""": """"0"""",
        //         """"Name"""": """"ورزش و سفر""""
        //     },
        // {
        //         """"Id"""": """"10"""",
        //         """"ParentId"""": """"0"""",
        //         """"Name"""": """"تجهیزات سالمندی""""
        //     },
        // {
        //         """"Id"""": """"175"""",
        //         """"ParentId"""": """"10"""",
        //         """"Name"""": """"تجهیزات سنجنش و کنترل سلامت""""
        //     },
        // {
        //         """"Id"""": """"176"""",
        //         """"ParentId"""": """"10"""",
        //         """"Name"""": """"تجهیزات کمک درمانی""""
        //     },
        // {
        //         """"Id"""": """"177"""",
        //         """"ParentId"""": """"10"""",
        //         """"Name"""": """"تجهیزات و ملزومات پزشکی مراقبت در منزل""""
        //     },
        // {
        //         """"Id"""": """"178"""",
        //         """"ParentId"""": """"10"""",
        //         """"Name"""": """"تجهیزات طبی ارتوپدی""""
        //     },
        // {
        //         """"Id"""": """"179"""",
        //         """"ParentId"""": """"10"""",
        //         """"Name"""": """"تجهیزات ورزشی و فیزیوتراپی""""
        //     },
        //     {
        //         """"Id"""": """"11"""",
        //         """"ParentId"""": """"1"""",
        //         """"Name"""": """"نوشیدنی ها""""
        //     },
        //     {
        //         """"Id"""": """"12"""",
        //         """"ParentId"""": """"1"""",
        //         """"Name"""": """"کالای اساسی و خوارو بار""""
        //     },
        //     {
        //         """"Id"""": """"13"""",
        //         """"ParentId"""": """"1"""",
        //         """"Name"""": """"شیرینی ،آجیل و خشکبار""""
        //     },
        //     {
        //         """"Id"""": """"14"""",
        //         """"ParentId"""": """"1"""",
        //         """"Name"""": """"شور و ترشی""""
        //     },
        //     {
        //         """"Id"""": """"15"""",
        //         """"ParentId"""": """"1"""",
        //         """"Name"""": """"صبحانه""""
        //     },
        //     {
        //         """"Id"""": """"16"""",
        //         """"ParentId"""": """"1"""",
        //         """"Name"""": """"ادویه و چاشنی""""
        //     },
        //     {
        //         """"Id"""": """"17"""",
        //         """"ParentId"""": """"1"""",
        //         """"Name"""": """"لبنیات""""
        //     },
        //     {
        //         """"Id"""": """"18"""",
        //         """"ParentId"""": """"1"""",
        //         """"Name"""": """"میوه و سبزی""""
        //     },
        //     {
        //         """"Id"""": """"19"""",
        //         """"ParentId"""": """"2"""",
        //         """"Name"""": """"محصولات چرمی""""
        //     },
        //     {
        //         """"Id"""": """"20"""",
        //         """"ParentId"""": """"2"""",
        //         """"Name"""": """"محصولات چوبی و حصیری""""
        //     },
        //     {
        //         """"Id"""": """"21"""",
        //         """"ParentId"""": """"2"""",
        //         """"Name"""": """"شیشه و آبگینه""""
        //     },
        //     {
        //         """"Id"""": """"22"""",
        //         """"ParentId"""": """"2"""",
        //         """"Name"""": """"سفال ،سرامیک""""
        //     },
        //     {
        //         """"Id"""": """"23"""",
        //         """"ParentId"""": """"2"""",
        //         """"Name"""": """"بافتنی و رودوزی""""
        //     },
        //     {
        //         """"Id"""": """"24"""",
        //         """"ParentId"""": """"2"""",
        //         """"Name"""": """"سنگ های قیمتی""""
        //     },
        //     {
        //         """"Id"""": """"25"""",
        //         """"ParentId"""": """"2"""",
        //         """"Name"""": """"کادو و هدیه """"
        //     },
        //     {
        //         """"Id"""": """"26"""",
        //         """"ParentId"""": """"2"""",
        //         """"Name"""": """"هنرهای تجسمی""""
        //     },
        //     {
        //         """"Id"""": """"27"""",
        //         """"ParentId"""": """"3"""",
        //         """"Name"""": """"زنانه""""
        //     },
        //     {
        //         """"Id"""": """"28"""",
        //         """"ParentId"""": """"3"""",
        //         """"Name"""": """"مردانه""""
        //     },
        //     {
        //         """"Id"""": """"29"""",
        //         """"ParentId"""": """"3"""",
        //         """"Name"""": """"بچه گانه""""
        //     },
        //     {
        //         """"Id"""": """"30"""",
        //         """"ParentId"""": """"4"""",
        //         """"Name"""": """"آرایشی""""
        //     },
        //     {
        //         """"Id"""": """"31"""",
        //         """"ParentId"""": """"4"""",
        //         """"Name"""": """"بهداشتی""""
        //     },
        //     {
        //         """"Id"""": """"32"""",
        //         """"ParentId"""": """"5"""",
        //         """"Name"""": """"مرهم و روغن های گیاهی""""
        //     },
        //     {
        //         """"Id"""": """"33"""",
        //         """"ParentId"""": """"5"""",
        //         """"Name"""": """"عرقیات و گلاب""""
        //     },
        //     {
        //         """"Id"""": """"34"""",
        //         """"ParentId"""": """"5"""",
        //         """"Name"""": """"گیاهان دارویی""""
        //     },
        //     {
        //         """"Id"""": """"35"""",
        //         """"ParentId"""": """"5"""",
        //         """"Name"""": """"سایر""""
        //     },
        //     {
        //         """"Id"""": """"36"""",
        //         """"ParentId"""": """"6"""",
        //         """"Name"""": """"گوشی موبایل و تبلت""""
        //     },
        //     {
        //         """"Id"""": """"37"""",
        //         """"ParentId"""": """"6"""",
        //         """"Name"""": """"کامپیوتر و لبتاپ""""
        //     },
        //     {
        //         """"Id"""": """"38"""",
        //         """"ParentId"""": """"6"""",
        //         """"Name"""": """"دوربین""""
        //     },
        //     {
        //         """"Id"""": """"39"""",
        //         """"ParentId"""": """"7"""",
        //         """"Name"""": """"بازی و سرگرمی""""
        //     },
        //     {
        //         """"Id"""": """"40"""",
        //         """"ParentId"""": """"7"""",
        //         """"Name"""": """"فرهنگی و آموزشی""""
        //     },
        //     {
        //         """"Id"""": """"41"""",
        //         """"ParentId"""": """"8"""",
        //         """"Name"""": """"فرش و تابلو فرش""""
        //     },
        //     {
        //         """"Id"""": """"42"""",
        //         """"ParentId"""": """"8"""",
        //         """"Name"""": """"دکوری تزپینی""""
        //     },
        //     {
        //         """"Id"""": """"43"""",
        //         """"ParentId"""": """"8"""",
        //         """"Name"""": """"لوازم آشپزخانه""""
        //     },
        //     {
        //         """"Id"""": """"44"""",
        //         """"ParentId"""": """"8"""",
        //         """"Name"""": """"گل و گیاه""""
        //     },
        //     {
        //         """"Id"""": """"45"""",
        //         """"ParentId"""": """"8"""",
        //         """"Name"""": """"نور و روشنایی""""
        //     },
        //     {
        //         """"Id"""": """"46"""",
        //         """"ParentId"""": """"8"""",
        //         """"Name"""": """"پرده و مبلمان""""
        //     },
        //     {
        //         """"Id"""": """"47"""",
        //         """"ParentId"""": """"8"""",
        //         """"Name"""": """"کالای خواب""""
        //     },
        //     {
        //         """"Id"""": """"48"""",
        //         """"ParentId"""": """"11"""",
        //         """"Name"""": """"چای و دمنوش""""
        //     },
        //     {
        //         """"Id"""": """"49"""",
        //         """"ParentId"""": """"11"""",
        //         """"Name"""": """"قهوه و محصولا کافین دار""""
        //     },
        //     {
        //         """"Id"""": """"50"""",
        //         """"ParentId"""": """"11"""",
        //         """"Name"""": """"مد و نوشیدنی های گازدار""""
        //     },
        //     {
        //         """"Id"""": """"51"""",
        //         """"ParentId"""": """"11"""",
        //         """"Name"""": """"آبمیوه و شربت""""
        //     },
        //     {
        //         """"Id"""": """"52"""",
        //         """"ParentId"""": """"12"""",
        //         """"Name"""": """"برنج""""
        //     },
        //     {
        //         """"Id"""": """"53"""",
        //         """"ParentId"""": """"12"""",
        //         """"Name"""": """"قند و شکر""""
        //     },
        //     {
        //         """"Id"""": """"54"""",
        //         """"ParentId"""": """"12"""",
        //         """"Name"""": """"نان و غلات""""
        //     },
        //     {
        //         """"Id"""": """"55"""",
        //         """"ParentId"""": """"12"""",
        //         """"Name"""": """"روغن""""
        //     },
        //     {
        //         """"Id"""": """"56"""",
        //         """"ParentId"""": """"12"""",
        //         """"Name"""": """"حبوبات """"
        //     },
        //     {
        //         """"Id"""": """"57"""",
        //         """"ParentId"""": """"12"""",
        //         """"Name"""": """"رشته و ماکارانی""""
        //     },
        //     {
        //         """"Id"""": """"58"""",
        //         """"ParentId"""": """"12"""",
        //         """"Name"""": """"مواد پروتینی """"
        //     },
        //     {
        //         """"Id"""": """"59"""",
        //         """"ParentId"""": """"12"""",
        //         """"Name"""": """"غذای آماده و نیمه آماده""""
        //     },
        //     {
        //         """"Id"""": """"60"""",
        //         """"ParentId"""": """"13"""",
        //         """"Name"""": """"کیک و شیرینی""""
        //     },
        //     {
        //         """"Id"""": """"61"""",
        //         """"ParentId"""": """"13"""",
        //         """"Name"""": """"دسر""""
        //     },
        //     {
        //         """"Id"""": """"62"""",
        //         """"ParentId"""": """"13"""",
        //         """"Name"""": """"آجیل و سایر خشکبار""""
        //     },
        //     {
        //         """"Id"""": """"63"""",
        //         """"ParentId"""": """"13"""",
        //         """"Name"""": """"میوه خشک""""
        //     },
        //     {
        //         """"Id"""": """"64"""",
        //         """"ParentId"""": """"13"""",
        //         """"Name"""": """"خرما """"
        //     },
        //     {
        //         """"Id"""": """"65"""",
        //         """"ParentId"""": """"13"""",
        //         """"Name"""": """"گز و سوهان""""
        //     },
        //     {
        //         """"Id"""": """"66"""",
        //         """"ParentId"""": """"13"""",
        //         """"Name"""": """"تنقلات""""
        //     },
        //     {
        //         """"Id"""": """"67"""",
        //         """"ParentId"""": """"14"""",
        //         """"Name"""": """"ترشی""""
        //     },
        //     {
        //         """"Id"""": """"68"""",
        //         """"ParentId"""": """"14"""",
        //         """"Name"""": """"شور""""
        //     },
        //     {
        //         """"Id"""": """"69"""",
        //         """"ParentId"""": """"14"""",
        //         """"Name"""": """"لواشک و ترشک""""
        //     },
        //     {
        //         """"Id"""": """"70"""",
        //         """"ParentId"""": """"14"""",
        //         """"Name"""": """"زیتون""""
        //     },
        //     {
        //         """"Id"""": """"71"""",
        //         """"ParentId"""": """"15"""",
        //         """"Name"""": """"عسل""""
        //     },
        //     {
        //         """"Id"""": """"72"""",
        //         """"ParentId"""": """"15"""",
        //         """"Name"""": """"ادره""""
        //     },
        //     {
        //         """"Id"""": """"73"""",
        //         """"ParentId"""": """"15"""",
        //         """"Name"""": """"مربا""""
        //     },
        //     {
        //         """"Id"""": """"74"""",
        //         """"ParentId"""": """"15"""",
        //         """"Name"""": """"شیره""""
        //     },
        //     {
        //         """"Id"""": """"75"""",
        //         """"ParentId"""": """"15"""",
        //         """"Name"""": """"حلواشکری """"
        //     },
        //     {
        //         """"Id"""": """"76"""",
        //         """"ParentId"""": """"16"""",
        //         """"Name"""": """"ادویه و چاشنی""""
        //     },
        //     {
        //         """"Id"""": """"77"""",
        //         """"ParentId"""": """"16"""",
        //         """"Name"""": """"زعفران""""
        //     },
        //     {
        //         """"Id"""": """"78"""",
        //         """"ParentId"""": """"16"""",
        //         """"Name"""": """"زرشک""""
        //     },
        //     {
        //         """"Id"""": """"79"""",
        //         """"ParentId"""": """"16"""",
        //         """"Name"""": """"رب""""
        //     },
        //     {
        //         """"Id"""": """"80"""",
        //         """"ParentId"""": """"16"""",
        //         """"Name"""": """"آبلیموه و آبغوره وسرکه """"
        //     },
        //     {
        //         """"Id"""": """"81"""",
        //         """"ParentId"""": """"16"""",
        //         """"Name"""": """"سس""""
        //     },
        //     {
        //         """"Id"""": """"82"""",
        //         """"ParentId"""": """"16"""",
        //         """"Name"""": """"سایرچاشنی ها""""
        //     },
        //     {
        //         """"Id"""": """"83"""",
        //         """"ParentId"""": """"17"""",
        //         """"Name"""": """"شیر""""
        //     },
        //     {
        //         """"Id"""": """"84"""",
        //         """"ParentId"""": """"17"""",
        //         """"Name"""": """"ماست""""
        //     },
        //     {
        //         """"Id"""": """"85"""",
        //         """"ParentId"""": """"17"""",
        //         """"Name"""": """"دوغ""""
        //     },
        //     {
        //         """"Id"""": """"86"""",
        //         """"ParentId"""": """"17"""",
        //         """"Name"""": """"خامه و سرشیر""""
        //     },
        //     {
        //         """"Id"""": """"87"""",
        //         """"ParentId"""": """"17"""",
        //         """"Name"""": """"کشک""""
        //     },
        //     {
        //         """"Id"""": """"88"""",
        //         """"ParentId"""": """"17"""",
        //         """"Name"""": """"کره و پنیر""""
        //     },
        //     {
        //         """"Id"""": """"89"""",
        //         """"ParentId"""": """"18"""",
        //         """"Name"""": """"میوه""""
        //     },
        //     {
        //         """"Id"""": """"90"""",
        //         """"ParentId"""": """"18"""",
        //         """"Name"""": """"سبزی""""
        //     },
        //     {
        //         """"Id"""": """"91"""",
        //         """"ParentId"""": """"18"""",
        //         """"Name"""": """"صیفی جات""""
        //     },
        //     {
        //         """"Id"""": """"92"""",
        //         """"ParentId"""": """"19"""",
        //         """"Name"""": """"حصیر بافی""""
        //     },
        //     {
        //         """"Id"""": """"93"""",
        //         """"ParentId"""": """"19"""",
        //         """"Name"""": """"معرق، منبتو خاتم کاری""""
        //     },
        //     {
        //         """"Id"""": """"94"""",
        //         """"ParentId"""": """"19"""",
        //         """"Name"""": """"خراطی""""
        //     },
        //     {
        //         """"Id"""": """"95"""",
        //         """"ParentId"""": """"19"""",
        //         """"Name"""": """"سایر محصولات چوبی وحصیری""""
        //     },
        //     {
        //         """"Id"""": """"96"""",
        //         """"ParentId"""": """"21"""",
        //         """"Name"""": """"سفال ،سرامیک""""
        //     },
        //     {
        //         """"Id"""": """"97"""",
        //         """"ParentId"""": """"21"""",
        //         """"Name"""": """"کاشی""""
        //     },
        //     {
        //         """"Id"""": """"98"""",
        //         """"ParentId"""": """"21"""",
        //         """"Name"""": """"چینی""""
        //     },
        //     {
        //         """"Id"""": """"99"""",
        //         """"ParentId"""": """"23"""",
        //         """"Name"""": """"قلاب بافی""""
        //     },
        //     {
        //         """"Id"""": """"100"""",
        //         """"ParentId"""": """"23"""",
        //         """"Name"""": """"تریکو بافی""""
        //     },
        //     {
        //         """"Id"""": """"101"""",
        //         """"ParentId"""": """"23"""",
        //         """"Name"""": """"مکرومه بافی""""
        //     },
        //     {
        //         """"Id"""": """"102"""",
        //         """"ParentId"""": """"23"""",
        //         """"Name"""": """"سوزن دوزی""""
        //     },
        //     {
        //         """"Id"""": """"103"""",
        //         """"ParentId"""": """"23"""",
        //         """"Name"""": """"گلدوزی""""
        //     },
        //     {
        //         """"Id"""": """"104"""",
        //         """"ParentId"""": """"23"""",
        //         """"Name"""": """"کاموابافی""""
        //     },
        //     {
        //         """"Id"""": """"105"""",
        //         """"ParentId"""": """"26"""",
        //         """"Name"""": """"هنرهای تجسمی""""
        //     },
        //     {
        //         """"Id"""": """"106"""",
        //         """"ParentId"""": """"26"""",
        //         """"Name"""": """"تذهیب و خوشنویسی""""
        //     },
        //     {
        //         """"Id"""": """"107"""",
        //         """"ParentId"""": """"26"""",
        //         """"Name"""": """"طراحی و نقاشی""""
        //     },
        //     {
        //         """"Id"""": """"108"""",
        //         """"ParentId"""": """"26"""",
        //         """"Name"""": """"مجسمه سازی""""
        //     },
        //     {
        //         """"Id"""": """"109"""",
        //         """"ParentId"""": """"26"""",
        //         """"Name"""": """"نگارگری""""
        //     },
        //     {
        //         """"Id"""": """"110"""",
        //         """"ParentId"""": """"26"""",
        //         """"Name"""": """"فرش و قالیچه""""
        //     },
        //     {
        //         """"Id"""": """"111"""",
        //         """"ParentId"""": """"27"""",
        //         """"Name"""": """"لباس زنانه""""
        //     },
        //     {
        //         """"Id"""": """"112"""",
        //         """"ParentId"""": """"27"""",
        //         """"Name"""": """"کیف و کفش زنانه""""
        //     },
        //     {
        //         """"Id"""": """"113"""",
        //         """"ParentId"""": """"27"""",
        //         """"Name"""": """"جواهرات و زیورآلات""""
        //     },
        //     {
        //         """"Id"""": """"114"""",
        //         """"ParentId"""": """"27"""",
        //         """"Name"""": """"ساعت زنانه""""
        //     },
        //     {
        //         """"Id"""": """"115"""",
        //         """"ParentId"""": """"27"""",
        //         """"Name"""": """"اکسسوری زنانه""""
        //     },
        //     {
        //         """"Id"""": """"116"""",
        //         """"ParentId"""": """"28"""",
        //         """"Name"""": """"لباس مردانه""""
        //     },
        //     {
        //         """"Id"""": """"117"""",
        //         """"ParentId"""": """"28"""",
        //         """"Name"""": """"کیف و کفش مردانه""""
        //     },
        //     {
        //         """"Id"""": """"118"""",
        //         """"ParentId"""": """"28"""",
        //         """"Name"""": """"جواهرات و زیورآلات""""
        //     },
        //     {
        //         """"Id"""": """"119"""",
        //         """"ParentId"""": """"29"""",
        //         """"Name"""": """"لباس بچگانه""""
        //     },
        //     {
        //         """"Id"""": """"120"""",
        //         """"ParentId"""": """"29"""",
        //         """"Name"""": """"کیف و کفش بچگانه""""
        //     },
        //     {
        //         """"Id"""": """"121"""",
        //         """"ParentId"""": """"29"""",
        //         """"Name"""": """"اکسسوری بچگانه""""
        //     },
        //     {
        //         """"Id"""": """"122"""",
        //         """"ParentId"""": """"30"""",
        //         """"Name"""": """"آرایش صورت""""
        //     },
        //     {
        //         """"Id"""": """"123"""",
        //         """"ParentId"""": """"30"""",
        //         """"Name"""": """"آرایش چشم و ابرو""""
        //     },
        //     {
        //         """"Id"""": """"124"""",
        //         """"ParentId"""": """"30"""",
        //         """"Name"""": """"آرایش لب""""
        //     },
        //     {
        //         """"Id"""": """"125"""",
        //         """"ParentId"""": """"30"""",
        //         """"Name"""": """"آرایش مو""""
        //     },
        //     {
        //         """"Id"""": """"126"""",
        //         """"ParentId"""": """"31"""",
        //         """"Name"""": """"بهداشت شخصی""""
        //     },
        //     {
        //         """"Id"""": """"127"""",
        //         """"ParentId"""": """"31"""",
        //         """"Name"""": """"مراقب و بهداشت پوست""""
        //     },
        //     {
        //         """"Id"""": """"128"""",
        //         """"ParentId"""": """"31"""",
        //         """"Name"""": """"مراقبت وبهداشت مو""""
        //     },
        //     {
        //         """"Id"""": """"129"""",
        //         """"ParentId"""": """"31"""",
        //         """"Name"""": """"الکل و ضدعفونی کننده""""
        //     },
        //     {
        //         """"Id"""": """"130"""",
        //         """"ParentId"""": """"32"""",
        //         """"Name"""": """"روغن های گیاهی""""
        //     },
        //     {
        //         """"Id"""": """"131"""",
        //         """"ParentId"""": """"32"""",
        //         """"Name"""": """"پماد و مرهم""""
        //     },
        //     {
        //         """"Id"""": """"132"""",
        //         """"ParentId"""": """"32"""",
        //         """"Name"""": """"آرایش لب""""
        //     },
        //     {
        //         """"Id"""": """"133"""",
        //         """"ParentId"""": """"32"""",
        //         """"Name"""": """"آرایش مو""""
        //     },
        //     {
        //         """"Id"""": """"134"""",
        //         """"ParentId"""": """"36"""",
        //         """"Name"""": """"گوشی موبایل""""
        //     },
        //     {
        //         """"Id"""": """"135"""",
        //         """"ParentId"""": """"36"""",
        //         """"Name"""": """"تبلت""""
        //     },
        //     {
        //         """"Id"""": """"136"""",
        //         """"ParentId"""": """"36"""",
        //         """"Name"""": """"لوازم جانبی""""
        //     },
        //     {
        //         """"Id"""": """"137"""",
        //         """"ParentId"""": """"37"""",
        //         """"Name"""": """"کامپیوتر""""
        //     },
        //     {
        //         """"Id"""": """"138"""",
        //         """"ParentId"""": """"37"""",
        //         """"Name"""": """"لبتاپ""""
        //     },
        //     {
        //         """"Id"""": """"139"""",
        //         """"ParentId"""": """"38"""",
        //         """"Name"""": """"انواع دوربین""""
        //     },
        //     {
        //         """"Id"""": """"140"""",
        //         """"ParentId"""": """"38"""",
        //         """"Name"""": """"لوازم جانبی""""
        //     },
        //     {
        //         """"Id"""": """"141"""",
        //         """"ParentId"""": """"39"""",
        //         """"Name"""": """"اسباب بازی""""
        //     },
        //     {
        //         """"Id"""": """"142"""",
        //         """"ParentId"""": """"39"""",
        //         """"Name"""": """"بازی فکری""""
        //     },
        //     {
        //         """"Id"""": """"143"""",
        //         """"ParentId"""": """"39"""",
        //         """"Name"""": """"سایر بازی و سرگرمی""""
        //     },
        //     {
        //         """"Id"""": """"144"""",
        //         """"ParentId"""": """"40"""",
        //         """"Name"""": """"کتاب """"
        //     },
        //     {
        //         """"Id"""": """"145"""",
        //         """"ParentId"""": """"40"""",
        //         """"Name"""": """"بسته های آموزشی""""
        //     },
        //     {
        //         """"Id"""": """"146"""",
        //         """"ParentId"""": """"40"""",
        //         """"Name"""": """"لوازم التحریر""""
        //     },
        //     {
        //         """"Id"""": """"147"""",
        //         """"ParentId"""": """"40"""",
        //         """"Name"""": """"سایر محصولات فرهنگی""""
        //     },
        //     {
        //         """"Id"""": """"148"""",
        //         """"ParentId"""": """"41"""",
        //         """"Name"""": """"فرش و قالیچه""""
        //     },
        //     {
        //         """"Id"""": """"149"""",
        //         """"ParentId"""": """"41"""",
        //         """"Name"""": """"پادری و روفرشی""""
        //     },
        //     {
        //         """"Id"""": """"150"""",
        //         """"ParentId"""": """"41"""",
        //         """"Name"""": """"تابلوفرش""""
        //     },
        //     {
        //         """"Id"""": """"151"""",
        //         """"ParentId"""": """"41"""",
        //         """"Name"""": """"گلیم و جاجیم""""
        //     },
        //     {
        //         """"Id"""": """"152"""",
        //         """"ParentId"""": """"42"""",
        //         """"Name"""": """"مجسمه  و  تندیس""""
        //     },
        //     {
        //         """"Id"""": """"153"""",
        //         """"ParentId"""": """"42"""",
        //         """"Name"""": """"شلف و استند""""
        //     },
        //     {
        //         """"Id"""": """"154"""",
        //         """"ParentId"""": """"42"""",
        //         """"Name"""": """"آینه و تابلو""""
        //     },
        //     {
        //         """"Id"""": """"155"""",
        //         """"ParentId"""": """"42"""",
        //         """"Name"""": """"سایر لوازم تزئینی""""
        //     },
        //     {
        //         """"Id"""": """"156"""",
        //         """"ParentId"""": """"42"""",
        //         """"Name"""": """"لوازم تهیه و سرور چایی""""
        //     },
        //     {
        //         """"Id"""": """"157"""",
        //         """"ParentId"""": """"42"""",
        //         """"Name"""": """"سایر لوازم آشپزخانه""""
        //     },
        //     {
        //         """"Id"""": """"158"""",
        //         """"ParentId"""": """"43"""",
        //         """"Name"""": """"سفره و ظروف یکبار مصرف""""
        //     },
        //     {
        //         """"Id"""": """"159"""",
        //         """"ParentId"""": """"43"""",
        //         """"Name"""": """"انواع سرویس های غذاخوری""""
        //     },
        //     {
        //         """"Id"""": """"160"""",
        //         """"ParentId"""": """"43"""",
        //         """"Name"""": """"لوازم پخت و پز""""
        //     },
        //     {
        //         """"Id"""": """"161"""",
        //         """"ParentId"""": """"43"""",
        //         """"Name"""": """"ظروف سرو و پذیرایی""""
        //     },
        //     {
        //         """"Id"""": """"162"""",
        //         """"ParentId"""": """"45"""",
        //         """"Name"""": """"لوستر و آباژور""""
        //     },
        //     {
        //         """"Id"""": """"163"""",
        //         """"ParentId"""": """"45"""",
        //         """"Name"""": """"انواع لامپ""""
        //     },
        //     {
        //         """"Id"""": """"164"""",
        //         """"ParentId"""": """"45"""",
        //         """"Name"""": """"سایر لوازم روشنایی""""
        //     },
        //     {
        //         """"Id"""": """"165"""",
        //         """"ParentId"""": """"46"""",
        //         """"Name"""": """"مبل""""
        //     },
        //     {
        //         """"Id"""": """"166"""",
        //         """"ParentId"""": """"46"""",
        //         """"Name"""": """"میز و صندلی""""
        //     },
        //     {
        //         """"Id"""": """"167"""",
        //         """"ParentId"""": """"46"""",
        //         """"Name"""": """"پرده و ملزومات""""
        //     },
        //     {
        //         """"Id"""": """"168"""",
        //         """"ParentId"""": """"46"""",
        //         """"Name"""": """"تخت خواب و کمد""""
        //     },
        //     {
        //         """"Id"""": """"169"""",
        //         """"ParentId"""": """"47"""",
        //         """"Name"""": """"تشک و بالش""""
        //     },
        //     {
        //         """"Id"""": """"170"""",
        //         """"ParentId"""": """"47"""",
        //         """"Name"""": """"روتختی و پتو""""
        //     },
        //     {
        //         """"Id"""": """"171"""",
        //         """"ParentId"""": """"47"""",
        //         """"Name"""": """"سایرکالای خواب""""
        //     },
        //     {
        //         """"Id"""": """"172"""",
        //         """"ParentId"""": """"9"""",
        //         """"Name"""": """"پوشاک و لوازم ورزشی""""
        //     },
        //     {
        //         """"Id"""": """"173"""",
        //         """"ParentId"""": """"9"""",
        //         """"Name"""": """"تجهیزات سفر""""
        //     },
        //     {
        //         """"Id"""": """"174"""",
        //         """"ParentId"""": """"9"""",
        //         """"Name"""": """"کوه نوردی و کمپینگ""""
        //     }
        // ]"";
        //         var conv = JsonConvert.DeserializeObject<List<ProductCategory2>>(data);
        //         ICollection<ProductCategory> c = conv.Select(s => new ProductCategory
        //         {
        //             Id = Int64.Parse(s.Id),
        //             Name = s.Name,
        //             ParentId = Int64.Parse(s.ParentId)
        //         }).ToList();

        //         var a = ProductExtensions.GetCategoriesChildrenForSeed(c, 0, ""364cbb18-b044-4339-b5c9-de772c2f011d"");
        //         using (var context = _productContext)
        //         {
        //             try
        //             {
        //                 context.ChangeTracker.AutoDetectChangesEnabled = false;

        //                 // Make many calls in a loop
        //                 await context.ProductCategories.AddRangeAsync(a);
        //                 await context.SaveChangesAsync();
        //             }
        //             finally
        //             {
        //                 context.ChangeTracker.AutoDetectChangesEnabled = true;
        //             }
        //         }


        //         // try
        //         // {
        //         //     await _productCategories.AddRangeAsync(a);
        //         //     await _uow.SaveChangesAsync();
        //         // }
        //         // catch (System.Exception ex)
        //         // {
        //         //     // TODO
        //         // }
        //         await _mediator.Publish(new ProductCategoryCreatedEvent(tenant));
        //         return Ok();
        //     }
        public class sheet1
        {
            public List<ProductCategory2> ProductCategory { get; set; }


        }
        public class ProductCategory2
        {
            public string Id { get; set; }
            public string ParentId { get; set; }
            public string Name { get; set; }
        }
        public class br
        {
            public string Id { get; set; }
            public string Name { get; set; }


        }
    }
}
#region brands 
//   var d = @"[
//     {
//         ""Id"": ""1"",
//         ""Name"": ""کالا""
//     },
//     {
//         ""Id"": ""2"",
//         ""Name"": ""ماکروسافت""
//     },
//     {
//         ""Id"": ""3"",
//         ""Name"": ""اکسیر""
//     },
//     {
//         ""Id"": ""4"",
//         ""Name"": ""نایک""
//     },
//     {
//         ""Id"": ""5"",
//         ""Name"": ""نرمک""
//     },
//     {
//         ""Id"": ""6"",
//         ""Name"": ""صنایع دستی""
//     },
//     {
//         ""Id"": ""7"",
//         ""Name"": ""رز مارال""
//     },
//     {
//         ""Id"": ""8"",
//         ""Name"": ""بهسوز""
//     },
//     {
//         ""Id"": ""9"",
//         ""Name"": ""وارنا""
//     },
//     {
//         ""Id"": ""10"",
//         ""Name"": ""اطلس مهر شفا""
//     },
//     {
//         ""Id"": ""11"",
//         ""Name"": ""ساجیم""
//     },
//     {
//         ""Id"": ""12"",
//         ""Name"": ""ستاره بانو""
//     },
//     {
//         ""Id"": ""13"",
//         ""Name"": ""شفالیچ""
//     },
//     {
//         ""Id"": ""14"",
//         ""Name"": ""تیواس""
//     },
//     {
//         ""Id"": ""15"",
//         ""Name"": ""دهکده سبز""
//     },
//     {
//         ""Id"": ""16"",
//         ""Name"": ""محصولات دکتر روازاده""
//     },
//     {
//         ""Id"": ""17"",
//         ""Name"": ""shirazleather""
//     },
//     {
//         ""Id"": ""18"",
//         ""Name"": ""کروچا""
//     },
//     {
//         ""Id"": ""19"",
//         ""Name"": ""جعبه لوکس""
//     },
//     {
//         ""Id"": ""20"",
//         ""Name"": ""نپق""
//     },
//     {
//         ""Id"": ""21"",
//         ""Name"": ""آرتان""
//     },
//     {
//         ""Id"": ""22"",
//         ""Name"": ""به چیپس""
//     },
//     {
//         ""Id"": ""23"",
//         ""Name"": ""گوهر بیشه""
//     },
//     {
//         ""Id"": ""24"",
//         ""Name"": ""پوشاک آوان""
//     },
//     {
//         ""Id"": ""25"",
//         ""Name"": ""گالری الی""
//     },
//     {
//         ""Id"": ""26"",
//         ""Name"": ""بشری""
//     },
//     {
//         ""Id"": ""27"",
//         ""Name"": ""کیف دوزک""
//     },
//     {
//         ""Id"": ""28"",
//         ""Name"": ""gallery wood""
//     },
//     {
//         ""Id"": ""29"",
//         ""Name"": ""گالری کیمیا""
//     },
//     {
//         ""Id"": ""30"",
//         ""Name"": ""مس سفیدگری""
//     },
//     {
//         ""Id"": ""31"",
//         ""Name"": ""تکنو شکلات""
//     },
//     {
//         ""Id"": ""32"",
//         ""Name"": ""صنایع دستی میرزاآقائی""
//     },
//     {
//         ""Id"": ""33"",
//         ""Name"": ""Top_Notch""
//     },
//     {
//         ""Id"": ""34"",
//         ""Name"": ""گالری راسپینا""
//     },
//     {
//         ""Id"": ""35"",
//         ""Name"": ""هنرکده شادان""
//     },
//     {
//         ""Id"": ""36"",
//         ""Name"": ""خانه روسری اریکا""
//     },
//     {
//         ""Id"": ""37"",
//         ""Name"": ""پَرسازه""
//     },
//     {
//         ""Id"": ""38"",
//         ""Name"": ""ارم""
//     },
//     {
//         ""Id"": ""39"",
//         ""Name"": ""آیسا""
//     },
//     {
//         ""Id"": ""40"",
//         ""Name"": ""دکوراسیون اعتماد""
//     },
//     {
//         ""Id"": ""41"",
//         ""Name"": ""چرم گواشیر""
//     },
//     {
//         ""Id"": ""42"",
//         ""Name"": ""چرم فرهنگ""
//     },
//     {
//         ""Id"": ""43"",
//         ""Name"": ""زعفران نگین""
//     },
//     {
//         ""Id"": ""44"",
//         ""Name"": ""عروسک تک""
//     },
//     {
//         ""Id"": ""45"",
//         ""Name"": ""عسل گنجوان""
//     },
//     {
//         ""Id"": ""46"",
//         ""Name"": ""درنیکا""
//     },
//     {
//         ""Id"": ""47"",
//         ""Name"": ""کارنو""
//     },
//     {
//         ""Id"": ""48"",
//         ""Name"": ""دست سازه نهان""
//     },
//     {
//         ""Id"": ""49"",
//         ""Name"": ""چرم هنرآفرین""
//     },
//     {
//         ""Id"": ""50"",
//         ""Name"": ""زعفران ممتاز قائنات""
//     },
//     {
//         ""Id"": ""51"",
//         ""Name"": ""تولیدی خاتون""
//     },
//     {
//         ""Id"": ""52"",
//         ""Name"": ""معرق وین وود""
//     },
//     {
//         ""Id"": ""53"",
//         ""Name"": ""مزون ژوپ""
//     },
//     {
//         ""Id"": ""54"",
//         ""Name"": ""چرم سادات""
//     },
//     {
//         ""Id"": ""55"",
//         ""Name"": ""سیتو""
//     },
//     {
//         ""Id"": ""56"",
//         ""Name"": ""خام فروشی میلاد""
//     },
//     {
//         ""Id"": ""57"",
//         ""Name"": ""ادویه ممتاز کدبانو""
//     },
//     {
//         ""Id"": ""58"",
//         ""Name"": ""گیاهان دارویی فریمان""
//     },
//     {
//         ""Id"": ""59"",
//         ""Name"": ""شهر گلاب""
//     },
//     {
//         ""Id"": ""60"",
//         ""Name"": ""بدلیجات سارا""
//     },
//     {
//         ""Id"": ""61"",
//         ""Name"": ""گالری زیورآلات حسنا""
//     },
//     {
//         ""Id"": ""62"",
//         ""Name"": ""مزون روزالیا""
//     },
//     {
//         ""Id"": ""63"",
//         ""Name"": ""Best Baby""
//     },
//     {
//         ""Id"": ""64"",
//         ""Name"": ""Suny charm""
//     },
//     {
//         ""Id"": ""65"",
//         ""Name"": ""خشکبار و زعفران اکبری""
//     },
//     {
//         ""Id"": ""66"",
//         ""Name"": ""برنج گلستان""
//     },
//     {
//         ""Id"": ""67"",
//         ""Name"": ""لوازم آشپزخانه ایمان""
//     },
//     {
//         ""Id"": ""68"",
//         ""Name"": ""آرال چرم""
//     },
//     {
//         ""Id"": ""69"",
//         ""Name"": ""ناز خواب فروغ""
//     },
//     {
//         ""Id"": ""70"",
//         ""Name"": ""روجین""
//     },
//     {
//         ""Id"": ""71"",
//         ""Name"": ""اشک گل""
//     },
//     {
//         ""Id"": ""72"",
//         ""Name"": ""دهکده غذایی وارش""
//     },
//     {
//         ""Id"": ""73"",
//         ""Name"": ""عسل شفا بخش""
//     },
//     {
//         ""Id"": ""74"",
//         ""Name"": ""دایلکس الیکس لدورا""
//     },
//     {
//         ""Id"": ""75"",
//         ""Name"": ""کِشته""
//     },
//     {
//         ""Id"": ""76"",
//         ""Name"": ""استودیو روزن""
//     },
//     {
//         ""Id"": ""77"",
//         ""Name"": ""سهیلا""
//     },
//     {
//         ""Id"": ""78"",
//         ""Name"": ""گالری رامو""
//     },
//     {
//         ""Id"": ""79"",
//         ""Name"": ""مشکات""
//     },
//     {
//         ""Id"": ""80"",
//         ""Name"": ""محصولات غذایی خانجون""
//     },
//     {
//         ""Id"": ""81"",
//         ""Name"": ""معرق سرو""
//     },
//     {
//         ""Id"": ""82"",
//         ""Name"": ""مژ بریژ""
//     },
//     {
//         ""Id"": ""83"",
//         ""Name"": ""طلا و جواهر سازی دریک""
//     },
//     {
//         ""Id"": ""84"",
//         ""Name"": ""گروه تعاونی پاد""
//     },
//     {
//         ""Id"": ""85"",
//         ""Name"": ""تک کیف""
//     },
//     {
//         ""Id"": ""86"",
//         ""Name"": ""محصولات سلام""
//     },
//     {
//         ""Id"": ""87"",
//         ""Name"": ""پترگان""
//     },
//     {
//         ""Id"": ""88"",
//         ""Name"": ""کلانزه""
//     },
//     {
//         ""Id"": ""89"",
//         ""Name"": ""گالری هنری هانیل""
//     },
//     {
//         ""Id"": ""90"",
//         ""Name"": ""صنایع دستی اقاقیا""
//     },
//     {
//         ""Id"": ""91"",
//         ""Name"": ""ناربلا مکرومه""
//     },
//     {
//         ""Id"": ""92"",
//         ""Name"": ""قِل قِلی جان""
//     },
//     {
//         ""Id"": ""93"",
//         ""Name"": ""عقیق سرا""
//     },
//     {
//         ""Id"": ""94"",
//         ""Name"": ""عسل ناظمی""
//     },
//     {
//         ""Id"": ""95"",
//         ""Name"": ""نقشینه""
//     },
//     {
//         ""Id"": ""96"",
//         ""Name"": ""زعفران وزیری""
//     },
//     {
//         ""Id"": ""97"",
//         ""Name"": ""بانو""
//     },
//     {
//         ""Id"": ""98"",
//         ""Name"": ""خیاطی شیک دوخت""
//     },
//     {
//         ""Id"": ""99"",
//         ""Name"": ""شاپرک""
//     },
//     {
//         ""Id"": ""100"",
//         ""Name"": ""محصولات بهداشتی جلوه""
//     },
//     {
//         ""Id"": ""101"",
//         ""Name"": ""صنایع چوبی رویال""
//     },
//     {
//         ""Id"": ""102"",
//         ""Name"": ""نیلی نور""
//     },
//     {
//         ""Id"": ""103"",
//         ""Name"": ""پویا تویز""
//     },
//     {
//         ""Id"": ""104"",
//         ""Name"": ""چرم مریم""
//     },
//     {
//         ""Id"": ""105"",
//         ""Name"": ""Perfume Life""
//     },
//     {
//         ""Id"": ""106"",
//         ""Name"": ""Tabie Shop فروشگاه طبیعی""
//     },
//     {
//         ""Id"": ""107"",
//         ""Name"": ""صنایع دستی حمیده""
//     },
//     {
//         ""Id"": ""108"",
//         ""Name"": ""دست سازه های ویانا""
//     },
//     {
//         ""Id"": ""109"",
//         ""Name"": ""کوکباز""
//     },
//     {
//         ""Id"": ""110"",
//         ""Name"": ""جلوه""
//     },
//     {
//         ""Id"": ""111"",
//         ""Name"": ""ساینا""
//     },
//     {
//         ""Id"": ""112"",
//         ""Name"": ""رزق حلال""
//     },
//     {
//         ""Id"": ""113"",
//         ""Name"": ""محصولات غذایی پاپونک""
//     },
//     {
//         ""Id"": ""114"",
//         ""Name"": ""Lusy Candle""
//     },
//     {
//         ""Id"": ""115"",
//         ""Name"": ""زعفران گل سرخ""
//     },
//     {
//         ""Id"": ""116"",
//         ""Name"": ""صنایع دستی نگارفرش""
//     },
//     {
//         ""Id"": ""117"",
//         ""Name"": ""کوکباز""
//     },
//     {
//         ""Id"": ""118"",
//         ""Name"": ""پیشنهاد هفتگی""
//     },
//     {
//         ""Id"": ""119"",
//         ""Name"": ""بسته های ویژه""
//     }
// ]";

#endregion
#region comment
//     var data = @"[
//   {
//     ""Id"": ""277"",
//     ""Name"": ""مواد غذایی"",
//     ""ParentId"": ""0""
// },
// {
//     ""Id"": ""278"",
//     ""Name"": ""صنایع دستی"",
//     ""ParentId"": ""0""
// },
// {
//     ""Id"": ""279"",
//     ""Name"": ""مد و پوشاک"",
//     ""ParentId"": ""0""
// },
// {
//     ""Id"": ""280"",
//     ""Name"": ""زیبایی و بهداشتی"",
//     ""ParentId"": ""0""
// },
// {
//     ""Id"": ""281"",
//     ""Name"": ""عطاری"",
//     ""ParentId"": ""0""
// },
// {
//     ""Id"": ""282"",
//     ""Name"": ""کالای دیجیتال"",
//     ""ParentId"": ""0""
// },
// {
//     ""Id"": ""283"",
//     ""Name"": ""فرهنگی ،سرگرمی"",
//     ""ParentId"": ""0""
// },
// {
//     ""Id"": ""284"",
//     ""Name"": ""خانه و آشپزخانه"",
//     ""ParentId"": ""0""
// },
// {
//     ""Id"": ""285"",
//     ""Name"": ""ورزش و سفر"",
//     ""ParentId"": ""0""
// },
// {
//     ""Id"": ""286"",
//     ""Name"": ""نوشیدنی ها"",
//     ""ParentId"": ""277""
// },
// {
//     ""Id"": ""287"",
//     ""Name"": ""عرقیات و گلاب"",
//     ""ParentId"": ""281""
// },
// {
//     ""Id"": ""288"",
//     ""Name"": ""گیاهان دارویی"",
//     ""ParentId"": ""281""
// },
// {
//     ""Id"": ""289"",
//     ""Name"": ""سایر"",
//     ""ParentId"": ""281""
// },
// {
//     ""Id"": ""290"",
//     ""Name"": ""گوشی موبایل و تبلت"",
//     ""ParentId"": ""282""
// },
// {
//     ""Id"": ""291"",
//     ""Name"": ""کامپیوتر و لبتاپ"",
//     ""ParentId"": ""282""
// },
// {
//     ""Id"": ""292"",
//     ""Name"": ""دوربین"",
//     ""ParentId"": ""282""
// },
// {
//     ""Id"": ""293"",
//     ""Name"": ""بازی و سرگرمی"",
//     ""ParentId"": ""283""
// },
// {
//     ""Id"": ""294"",
//     ""Name"": ""مرهم و روغن های گیاهی"",
//     ""ParentId"": ""281""
// },
// {
//     ""Id"": ""295"",
//     ""Name"": ""فرهنگی و آموزشی"",
//     ""ParentId"": ""283""
// },
// {
//     ""Id"": ""296"",
//     ""Name"": ""دکوری تزپینی"",
//     ""ParentId"": ""284""
// },
// {
//     ""Id"": ""297"",
//     ""Name"": ""لوازم آشپزخانه"",
//     ""ParentId"": ""284""
// },
// {
//     ""Id"": ""298"",
//     ""Name"": ""گل و گیاه"",
//     ""ParentId"": ""284""
// },
// {
//     ""Id"": ""299"",
//     ""Name"": ""نور و روشنایی"",
//     ""ParentId"": ""284""
// },
// {
//     ""Id"": ""300"",
//     ""Name"": ""پرده و مبلمان"",
//     ""ParentId"": ""284""
// },
// {
//     ""Id"": ""301"",
//     ""Name"": ""کالای خواب"",
//     ""ParentId"": ""284""
// },
// {
//     ""Id"": ""302"",
//     ""Name"": ""پوشاک و لوازم ورزشی"",
//     ""ParentId"": ""285""
// },
// {
//     ""Id"": ""303"",
//     ""Name"": ""فرش و تابلو فرش"",
//     ""ParentId"": ""284""
// },
// {
//     ""Id"": ""304"",
//     ""Name"": ""بهداشتی"",
//     ""ParentId"": ""280""
// },
// {
//     ""Id"": ""305"",
//     ""Name"": ""آرایشی"",
//     ""ParentId"": ""280""
// },
// {
//     ""Id"": ""306"",
//     ""Name"": ""بچه گانه"",
//     ""ParentId"": ""279""
// },
// {
//     ""Id"": ""307"",
//     ""Name"": ""کالای اساسی و خوارو بار"",
//     ""ParentId"": ""277""
// },
// {
//     ""Id"": ""308"",
//     ""Name"": ""شیرینی ،آجیل و خشکبار"",
//     ""ParentId"": ""277""
// },
// {
//     ""Id"": ""309"",
//     ""Name"": ""شور و ترشی"",
//     ""ParentId"": ""277""
// },
// {
//     ""Id"": ""310"",
//     ""Name"": ""صبحانه"",
//     ""ParentId"": ""277""
// },
// {
//     ""Id"": ""311"",
//     ""Name"": ""ادویه و چاشنی"",
//     ""ParentId"": ""277""
// },
// {
//     ""Id"": ""312"",
//     ""Name"": ""لبنیات"",
//     ""ParentId"": ""277""
// },
// {
//     ""Id"": ""313"",
//     ""Name"": ""میوه و سبزی"",
//     ""ParentId"": ""277""
// },
// {
//     ""Id"": ""314"",
//     ""Name"": ""محصولات چرمی"",
//     ""ParentId"": ""278""
// },
// {
//     ""Id"": ""315"",
//     ""Name"": ""محصولات چوبی و حصیری"",
//     ""ParentId"": ""278""
// },
// {
//     ""Id"": ""316"",
//     ""Name"": ""شیشه و آبگینه"",
//     ""ParentId"": ""278""
// },
// {
//     ""Id"": ""317"",
//     ""Name"": ""سفال ،سرامیک"",
//     ""ParentId"": ""278""
// },
// {
//     ""Id"": ""318"",
//     ""Name"": ""بافتنی و رودوزی"",
//     ""ParentId"": ""278""
// },
// {
//     ""Id"": ""319"",
//     ""Name"": ""سنگ های قیمتی"",
//     ""ParentId"": ""278""
// },
// {
//     ""Id"": ""320"",
//     ""Name"": ""کادو و هدیه"",
//     ""ParentId"": ""278""
// },
// {
//     ""Id"": ""321"",
//     ""Name"": ""هنرهای تجسمی"",
//     ""ParentId"": ""278""
// },
// {
//     ""Id"": ""322"",
//     ""Name"": ""زنانه"",
//     ""ParentId"": ""279""
// },
// {
//     ""Id"": ""323"",
//     ""Name"": ""مردانه"",
//     ""ParentId"": ""279""
// },
// {
//     ""Id"": ""324"",
//     ""Name"": ""تجهیزات سفر"",
//     ""ParentId"": ""285""
// },
// {
//     ""Id"": ""325"",
//     ""Name"": ""کوه نوردی و کمپینگ"",
//     ""ParentId"": ""285""
// },
// {
//     ""Id"": ""326"",
//     ""Name"": ""چای و دمنوش"",
//     ""ParentId"": ""286""
// },
// {
//     ""Id"": ""327"",
//     ""Name"": ""لبتاپ"",
//     ""ParentId"": ""291""
// },
// {
//     ""Id"": ""328"",
//     ""Name"": ""کامپیوتر"",
//     ""ParentId"": ""291""
// },
// {
//     ""Id"": ""329"",
//     ""Name"": ""لوازم جانبی"",
//     ""ParentId"": ""290""
// },
// {
//     ""Id"": ""330"",
//     ""Name"": ""تبلت"",
//     ""ParentId"": ""290""
// },
// {
//     ""Id"": ""331"",
//     ""Name"": ""گوشی موبایل"",
//     ""ParentId"": ""290""
// },
// {
//     ""Id"": ""332"",
//     ""Name"": ""آرایش مو"",
//     ""ParentId"": ""294""
// },
// {
//     ""Id"": ""333"",
//     ""Name"": ""آرایش لب"",
//     ""ParentId"": ""294""
// },
// {
//     ""Id"": ""334"",
//     ""Name"": ""پماد و مرهم"",
//     ""ParentId"": ""294""
// },
// {
//     ""Id"": ""335"",
//     ""Name"": ""روغن های گیاهی"",
//     ""ParentId"": ""294""
// },
// {
//     ""Id"": ""336"",
//     ""Name"": ""الکل و ضدعفونی کننده"",
//     ""ParentId"": ""304""
// },
// {
//     ""Id"": ""337"",
//     ""Name"": ""مراقبت وبهداشت مو"",
//     ""ParentId"": ""304""
// },
// {
//     ""Id"": ""338"",
//     ""Name"": ""مراقب و بهداشت پوست"",
//     ""ParentId"": ""304""
// },
// {
//     ""Id"": ""339"",
//     ""Name"": ""بهداشت شخصی"",
//     ""ParentId"": ""304""
// },
// {
//     ""Id"": ""340"",
//     ""Name"": ""آرایش مو"",
//     ""ParentId"": ""305""
// },
// {
//     ""Id"": ""341"",
//     ""Name"": ""آرایش لب"",
//     ""ParentId"": ""305""
// },
// {
//     ""Id"": ""342"",
//     ""Name"": ""آرایش چشم و ابرو"",
//     ""ParentId"": ""305""
// },
// {
//     ""Id"": ""343"",
//     ""Name"": ""آرایش صورت"",
//     ""ParentId"": ""305""
// },
// {
//     ""Id"": ""344"",
//     ""Name"": ""اکسسوری بچگانه"",
//     ""ParentId"": ""306""
// },
// {
//     ""Id"": ""345"",
//     ""Name"": ""کیف و کفش بچگانه"",
//     ""ParentId"": ""306""
// },
// {
//     ""Id"": ""346"",
//     ""Name"": ""لباس بچگانه"",
//     ""ParentId"": ""306""
// },
// {
//     ""Id"": ""347"",
//     ""Name"": ""اکسسوری مردانه"",
//     ""ParentId"": ""323""
// },
// {
//     ""Id"": ""348"",
//     ""Name"": ""کیف و کفش مردانه"",
//     ""ParentId"": ""323""
// },
// {
//     ""Id"": ""349"",
//     ""Name"": ""لباس مردانه"",
//     ""ParentId"": ""323""
// },
// {
//     ""Id"": ""350"",
//     ""Name"": ""اکسسوری زنانه"",
//     ""ParentId"": ""322""
// },
// {
//     ""Id"": ""351"",
//     ""Name"": ""ساعت زنانه"",
//     ""ParentId"": ""322""
// },
// {
//     ""Id"": ""352"",
//     ""Name"": ""جواهرات و زیورآلات"",
//     ""ParentId"": ""322""
// },
// {
//     ""Id"": ""353"",
//     ""Name"": ""کیف و کفش زنانه"",
//     ""ParentId"": ""322""
// },
// {
//     ""Id"": ""354"",
//     ""Name"": ""انواع دوربین"",
//     ""ParentId"": ""292""
// },
// {
//     ""Id"": ""355"",
//     ""Name"": ""لباس زنانه"",
//     ""ParentId"": ""322""
// },
// {
//     ""Id"": ""356"",
//     ""Name"": ""لوازم جانبی"",
//     ""ParentId"": ""292""
// },
// {
//     ""Id"": ""357"",
//     ""Name"": ""بازی فکری"",
//     ""ParentId"": ""293""
// },
// {
//     ""Id"": ""358"",
//     ""Name"": ""تشک و بالش"",
//     ""ParentId"": ""301""
// },
// {
//     ""Id"": ""359"",
//     ""Name"": ""تخت خواب و کمد"",
//     ""ParentId"": ""300""
// },
// {
//     ""Id"": ""360"",
//     ""Name"": ""پرده و ملزومات"",
//     ""ParentId"": ""300""
// },
// {
//     ""Id"": ""361"",
//     ""Name"": ""میز و صندلی"",
//     ""ParentId"": ""300""
// },
// {
//     ""Id"": ""362"",
//     ""Name"": ""مبل"",
//     ""ParentId"": ""300""
// },
// {
//     ""Id"": ""363"",
//     ""Name"": ""سایر لوازم روشنایی"",
//     ""ParentId"": ""299""
// },
// {
//     ""Id"": ""364"",
//     ""Name"": ""انواع لامپ"",
//     ""ParentId"": ""299""
// },
// {
//     ""Id"": ""365"",
//     ""Name"": ""لوستر و آباژور"",
//     ""ParentId"": ""299""
// },
// {
//     ""Id"": ""366"",
//     ""Name"": ""ظروف سرو و پذیرایی"",
//     ""ParentId"": ""297""
// },
// {
//     ""Id"": ""367"",
//     ""Name"": ""لوازم پخت و پز"",
//     ""ParentId"": ""297""
// },
// {
//     ""Id"": ""368"",
//     ""Name"": ""انواع سرویس های غذاخوری"",
//     ""ParentId"": ""297""
// },
// {
//     ""Id"": ""369"",
//     ""Name"": ""سفره و ظروف یکبار مصرف"",
//     ""ParentId"": ""297""
// },
// {
//     ""Id"": ""370"",
//     ""Name"": ""سایر لوازم آشپزخانه"",
//     ""ParentId"": ""296""
// },
// {
//     ""Id"": ""371"",
//     ""Name"": ""لوازم تهیه و سرور چایی"",
//     ""ParentId"": ""296""
// },
// {
//     ""Id"": ""372"",
//     ""Name"": ""سایر لوازم تزئینی"",
//     ""ParentId"": ""296""
// },
// {
//     ""Id"": ""373"",
//     ""Name"": ""آینه و تابلو"",
//     ""ParentId"": ""296""
// },
// {
//     ""Id"": ""374"",
//     ""Name"": ""شلف و استند"",
//     ""ParentId"": ""296""
// },
// {
//     ""Id"": ""375"",
//     ""Name"": ""مجسمه و تندیس"",
//     ""ParentId"": ""296""
// },
// {
//     ""Id"": ""376"",
//     ""Name"": ""گلیم و جاجیم"",
//     ""ParentId"": ""303""
// },
// {
//     ""Id"": ""377"",
//     ""Name"": ""تابلوفرش"",
//     ""ParentId"": ""303""
// },
// {
//     ""Id"": ""378"",
//     ""Name"": ""پادری و روفرشی"",
//     ""ParentId"": ""303""
// },
// {
//     ""Id"": ""379"",
//     ""Name"": ""فرش و قالیچه"",
//     ""ParentId"": ""303""
// },
// {
//     ""Id"": ""380"",
//     ""Name"": ""سایر محصولات فرهنگی"",
//     ""ParentId"": ""295""
// },
// {
//     ""Id"": ""381"",
//     ""Name"": ""لوازم التحریر"",
//     ""ParentId"": ""295""
// },
// {
//     ""Id"": ""382"",
//     ""Name"": ""بسته های آموزشی"",
//     ""ParentId"": ""295""
// },
// {
//     ""Id"": ""383"",
//     ""Name"": ""کتاب"",
//     ""ParentId"": ""295""
// },
// {
//     ""Id"": ""384"",
//     ""Name"": ""سایر بازی و سرگرمی"",
//     ""ParentId"": ""293""
// },
// {
//     ""Id"": ""385"",
//     ""Name"": ""اسباب بازی"",
//     ""ParentId"": ""293""
// },
// {
//     ""Id"": ""386"",
//     ""Name"": ""فرش و قالیچه"",
//     ""ParentId"": ""321""
// },
// {
//     ""Id"": ""387"",
//     ""Name"": ""نگارگری"",
//     ""ParentId"": ""321""
// },
// {
//     ""Id"": ""388"",
//     ""Name"": ""مجسمه سازی"",
//     ""ParentId"": ""321""
// },
// {
//     ""Id"": ""389"",
//     ""Name"": ""حلواشکری"",
//     ""ParentId"": ""310""
// },
// {
//     ""Id"": ""390"",
//     ""Name"": ""شیره"",
//     ""ParentId"": ""310""
// },
// {
//     ""Id"": ""391"",
//     ""Name"": ""مربا"",
//     ""ParentId"": ""310""
// },
// {
//     ""Id"": ""392"",
//     ""Name"": ""ادره"",
//     ""ParentId"": ""310""
// },
// {
//     ""Id"": ""393"",
//     ""Name"": ""عسل"",
//     ""ParentId"": ""310""
// },
// {
//     ""Id"": ""394"",
//     ""Name"": ""زیتون"",
//     ""ParentId"": ""309""
// },
// {
//     ""Id"": ""395"",
//     ""Name"": ""لواشک و ترشک"",
//     ""ParentId"": ""309""
// },
// {
//     ""Id"": ""396"",
//     ""Name"": ""شور"",
//     ""ParentId"": ""309""
// },
// {
//     ""Id"": ""397"",
//     ""Name"": ""ترشی"",
//     ""ParentId"": ""309""
// },
// {
//     ""Id"": ""398"",
//     ""Name"": ""تنقلات"",
//     ""ParentId"": ""308""
// },
// {
//     ""Id"": ""399"",
//     ""Name"": ""گز و سوهان"",
//     ""ParentId"": ""308""
// },
// {
//     ""Id"": ""400"",
//     ""Name"": ""خرما"",
//     ""ParentId"": ""308""
// },
// {
//     ""Id"": ""401"",
//     ""Name"": ""میوه خشک"",
//     ""ParentId"": ""308""
// },
// {
//     ""Id"": ""402"",
//     ""Name"": ""آجیل و سایر خشکبار"",
//     ""ParentId"": ""308""
// },
// {
//     ""Id"": ""403"",
//     ""Name"": ""دسر"",
//     ""ParentId"": ""308""
// },
// {
//     ""Id"": ""404"",
//     ""Name"": ""کیک و شیرینی"",
//     ""ParentId"": ""308""
// },
// {
//     ""Id"": ""405"",
//     ""Name"": ""غذای آماده و نیمه آماده"",
//     ""ParentId"": ""307""
// },
// {
//     ""Id"": ""406"",
//     ""Name"": ""مواد پروتینی"",
//     ""ParentId"": ""307""
// },
// {
//     ""Id"": ""407"",
//     ""Name"": ""رشته و ماکارانی"",
//     ""ParentId"": ""307""
// },
// {
//     ""Id"": ""408"",
//     ""Name"": ""حبوبات"",
//     ""ParentId"": ""307""
// },
// {
//     ""Id"": ""409"",
//     ""Name"": ""روغن"",
//     ""ParentId"": ""307""
// },
// {
//     ""Id"": ""410"",
//     ""Name"": ""نان و غلات"",
//     ""ParentId"": ""307""
// },
// {
//     ""Id"": ""411"",
//     ""Name"": ""قند و شکر"",
//     ""ParentId"": ""307""
// },
// {
//     ""Id"": ""412"",
//     ""Name"": ""برنج"",
//     ""ParentId"": ""307""
// },
// {
//     ""Id"": ""413"",
//     ""Name"": ""آبمیوه و شربت"",
//     ""ParentId"": ""286""
// },
// {
//     ""Id"": ""414"",
//     ""Name"": ""مد و نوشیدنی های گازدار"",
//     ""ParentId"": ""286""
// },
// {
//     ""Id"": ""415"",
//     ""Name"": ""قهوه و محصولا کافین دار"",
//     ""ParentId"": ""286""
// },
// {
//     ""Id"": ""416"",
//     ""Name"": ""ادویه و چاشنی"",
//     ""ParentId"": ""311""
// },
// {
//     ""Id"": ""417"",
//     ""Name"": ""زعفران"",
//     ""ParentId"": ""311""
// },
// {
//     ""Id"": ""418"",
//     ""Name"": ""زرشک"",
//     ""ParentId"": ""311""
// },
// {
//     ""Id"": ""419"",
//     ""Name"": ""رب"",
//     ""ParentId"": ""311""
// },
// {
//     ""Id"": ""420"",
//     ""Name"": ""طراحی و نقاشی"",
//     ""ParentId"": ""321""
// },
// {
//     ""Id"": ""421"",
//     ""Name"": ""تذهیب و خوشنویسی"",
//     ""ParentId"": ""321""
// },
// {
//     ""Id"": ""422"",
//     ""Name"": ""هنرهای تجسمی"",
//     ""ParentId"": ""321""
// },
// {
//     ""Id"": ""423"",
//     ""Name"": ""کاموابافی"",
//     ""ParentId"": ""318""
// },
// {
//     ""Id"": ""424"",
//     ""Name"": ""گلدوزی"",
//     ""ParentId"": ""318""
// },
// {
//     ""Id"": ""425"",
//     ""Name"": ""سوزن دوزی"",
//     ""ParentId"": ""318""
// },
// {
//     ""Id"": ""426"",
//     ""Name"": ""مکرومه بافی"",
//     ""ParentId"": ""318""
// },
// {
//     ""Id"": ""427"",
//     ""Name"": ""تریکو بافی"",
//     ""ParentId"": ""318""
// },
// {
//     ""Id"": ""428"",
//     ""Name"": ""قلاب بافی"",
//     ""ParentId"": ""318""
// },
// {
//     ""Id"": ""429"",
//     ""Name"": ""چینی"",
//     ""ParentId"": ""316""
// },
// {
//     ""Id"": ""430"",
//     ""Name"": ""کاشی"",
//     ""ParentId"": ""316""
// },
// {
//     ""Id"": ""431"",
//     ""Name"": ""سفال ،سرامیک"",
//     ""ParentId"": ""316""
// },
// {
//     ""Id"": ""432"",
//     ""Name"": ""سایر محصولات چوبی وحصیری"",
//     ""ParentId"": ""314""
// },
// {
//     ""Id"": ""433"",
//     ""Name"": ""روتختی و پتو"",
//     ""ParentId"": ""301""
// },
// {
//     ""Id"": ""434"",
//     ""Name"": ""خراطی"",
//     ""ParentId"": ""314""
// },
// {
//     ""Id"": ""435"",
//     ""Name"": ""حصیر بافی"",
//     ""ParentId"": ""314""
// },
// {
//     ""Id"": ""436"",
//     ""Name"": ""صیفی جات"",
//     ""ParentId"": ""313""
// },
// {
//     ""Id"": ""437"",
//     ""Name"": ""سبزی"",
//     ""ParentId"": ""313""
// },
// {
//     ""Id"": ""438"",
//     ""Name"": ""میوه"",
//     ""ParentId"": ""313""
// },
// {
//     ""Id"": ""439"",
//     ""Name"": ""کره و پنیر"",
//     ""ParentId"": ""312""
// },
// {
//     ""Id"": ""440"",
//     ""Name"": ""کشک"",
//     ""ParentId"": ""312""
// },
// {
//     ""Id"": ""441"",
//     ""Name"": ""خامه و سرشیر"",
//     ""ParentId"": ""312""
// },
// {
//     ""Id"": ""442"",
//     ""Name"": ""دوغ"",
//     ""ParentId"": ""312""
// },
// {
//     ""Id"": ""443"",
//     ""Name"": ""ماست"",
//     ""ParentId"": ""312""
// },
// {
//     ""Id"": ""444"",
//     ""Name"": ""شیر"",
//     ""ParentId"": ""312""
// },
// {
//     ""Id"": ""445"",
//     ""Name"": ""سایر چاشنی ها"",
//     ""ParentId"": ""311""
// },
// {
//     ""Id"": ""446"",
//     ""Name"": ""سس"",
//     ""ParentId"": ""311""
// },
// {
//     ""Id"": ""447"",
//     ""Name"": ""آبلیموه و آبغوره وسرکه"",
//     ""ParentId"": ""311""
// },
// {
//     ""Id"": ""448"",
//     ""Name"": ""معرق، منبتو خاتم کاری"",
//     ""ParentId"": ""314""
// },
// {
//     ""Id"": ""449"",
//     ""Name"": ""سایرکالای خواب"",
//     ""ParentId"": ""301""
// },
// {
//     ""Id"": ""623"",
//     ""Name"": ""تجهیزات سلامت و سالمندی"",
//     ""ParentId"": ""0""
// },
// {
//     ""Id"": ""624"",
//     ""Name"": ""تجهیزات سنجش و کنترل سلامت"",
//     ""ParentId"": ""623""
// },
// {
//     ""Id"": ""625"",
//     ""Name"": ""تجهیزات کمک درمانی"",
//     ""ParentId"": ""623""
// },
// {
//     ""Id"": ""626"",
//     ""Name"": ""تجهیزات و ملزومات پزشکی مراقبت در منزل"",
//     ""ParentId"": ""623""
// },
// {
//     ""Id"": ""627"",
//     ""Name"": ""تجهیزات طبی ارتوپدی"",
//     ""ParentId"": ""623""
// },
// {
//     ""Id"": ""628"",
//     ""Name"": ""تجهیزات ورزشی و فیزیوتراپی"",
//     ""ParentId"": ""623""
// },
// {
//     ""Id"": ""632"",
//     ""Name"": ""خودرو و تجهیزات صنعتی"",
//     ""ParentId"": ""0""
// },
// {
//     ""Id"": ""633"",
//     ""Name"": ""کوه نوردی و کمپینگ"",
//     ""ParentId"": ""285""
// }

//         ]";
#endregion
