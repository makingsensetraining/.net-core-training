using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Library.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace Library.API.IntegrationTests
{
    class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void RegisterDbContext(IServiceCollection services)
        {
            services.AddDbContext<LibraryContext>(
                optionsBuilder => optionsBuilder.UseInMemoryDatabase("InMemoryDb"));

        }

        public override void EnsureDatabaseCreated(LibraryContext dbContext)
        {
        }
    }
}
