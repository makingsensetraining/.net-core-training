using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using CityInfo.API.Services;
using Microsoft.Extensions.Configuration;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;
using CityInfo.API.Models;

namespace CityInfo.API
{
    public class Startup
    {
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddMvcOptions(o=>o.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter()));
            //Uncomment this code to avoid property name transformation to camel case
            //.AddJsonOptions(o=> {
            //    if(o.SerializerSettings.ContractResolver != null)
            //    {
            //        var contractResolver = o.SerializerSettings.ContractResolver as DefaultContractResolver;
            //        contractResolver.NamingStrategy = null;
            //    }
            //});

#if DEBUG

            services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif
            var connString = Configuration["connectionStrings:cityInfoDBConnectionString"];
            services.AddDbContext<CityInfoContext>(c=> c.UseSqlServer(connString));
            services.AddScoped<ICityInfoRepository, CityInfoRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, 
            CityInfoContext cityInfoContext)
        {
            loggerFactory.AddNLog(); //add NLogLoggerProvider

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            //return status code and description
            app.UseStatusCodePages();

            AutoMapper.Mapper.Initialize(config => {
                config.CreateMap<City, CityWithoutPointsOfInterestDto>();
                config.CreateMap<City, CityDto>();
                config.CreateMap<PointOfInterest, PointOfInterestDto>();
                config.CreateMap<PointOfInterestForCreationDto, PointOfInterest>();
                config.CreateMap<PointOfInterestForUpdateDto, PointOfInterest>();
                config.CreateMap<PointOfInterest, PointOfInterestForUpdateDto>();
            });

            cityInfoContext.EnsureSeedDataForContext();

            app.UseMvc();

        }
    }
}
