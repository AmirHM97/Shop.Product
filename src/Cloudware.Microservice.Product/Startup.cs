
using Cloudware.Microservice.Product.Infrastructure;
using Cloudware.Microservice.Product.Infrastructure.Repositories;
using Cloudware.Microservice.Product.Infrastructure.Services;
using Cloudware.Utilities.Common.Extension;
using Cloudware.Utilities.Common.Setting;
using Cloudware.Utilities.Configure.Microservice.Middleware;
using Cloudware.Utilities.Configure.Microservice.Services;
using Cloudware.Utilities.Contract.Idp;
using Cloudware.Utilities.Formbuilder.Extension;
using Cloudware.Utilities.Table;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

using System.Linq;
using System.Reflection;


namespace Cloudware.Microservice.Product
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var currentDomain = AppDomain.CurrentDomain;
           
            var executingAssembly = Assembly.GetExecutingAssembly();
            Assembly contract = typeof(UserCreatedEvent).Assembly;
            var assembly = AppDomain.CurrentDomain.GetAssemblies();
            _ = assembly.Append(contract);
            services.AddScoped<IUnitOfWork, ProductContext>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IPropertyService, PropertyService>();
            var setting = services.AddClwServices<AppSettings>(Configuration, currentDomain, executingAssembly, System.AppContext.BaseDirectory,3);
            services.AddDbContext<ProductContext>(options => options
            .UseSqlServer(setting.ConnectionStrings.MsSql));

            services.AddSingleton<IProductReadDbContext, ProductReadDbContext>();
            services.AddClwTable();
            // services.Configure<ProductSettings>(Configuration.GetSection("ConnectionStrings"));
            // services.AddScoped<IProductItemsDataCollectionRepository, ProductItemsDataCollectionRepository>();

            services.AddScoped<Infrastructure.Services.IPaginationService, Infrastructure.Services.PaginationService>();


            services.AddClwFormbuilder(setting.ConnectionStrings.MongoDb);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseClw("Cloudware.Microservice.Product v1");

        }
    }

}