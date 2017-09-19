using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Library.API.Entities;
using Microsoft.EntityFrameworkCore;
using Library.API.Services;
using Library.API.Models;
using Library.API.Helpers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Library.API.Profiles;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;

namespace Library
{
    public class Startup
    {
        public static IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(setupAction => {
                setupAction.ReturnHttpNotAcceptable = true;
                setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                setupAction.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());
            });

            // register the DbContext on the container, getting the connection string from
            // appSettings (note: use this during development; in a production environment,
            // it's better to store the connection string in an environment variable)
            var connectionString = Configuration["connectionStrings:libraryDBConnectionString"];
            services.AddDbContext<LibraryContext>(o => o.UseSqlServer(connectionString));

            // register the repository
            services.AddScoped<ILibraryService, LibraryService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, LibraryContext libraryContext, 
            ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();

            app.AddNLogWeb();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //global exception handling
                app.UseExceptionHandler(appBuilder => appBuilder.Run(async context => {
                    var exceptionHandlerFeature =  context.Features.Get<IExceptionHandlerFeature>();
                    if (exceptionHandlerFeature != null)
                    {
                        //TODO: inject logger and log to file
                        var logger = loggerFactory.CreateLogger("Global exception logger");
                        var ex = exceptionHandlerFeature.Error;
                        logger.LogError(500, ex, ex.Message);
                    }
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("An exception has occured. Please try again later.");
                }));
            }

            AutoMapper.Mapper.Initialize(config => config.AddProfile<LibraryProfile>());

            libraryContext.EnsureSeedDataForContext();

            app.UseMvc();
        }
    }
}
