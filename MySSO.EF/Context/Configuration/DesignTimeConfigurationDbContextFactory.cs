using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MySSO.EF.Context.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MySSO.EF.Context.Configuration
{
    public class DesignTimeConfigurationDbContextFactory : IDesignTimeDbContextFactory<IdentityConfigurationDbContext>
    {
        public IdentityConfigurationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json")
             .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var builder = new DbContextOptionsBuilder<ConfigurationDbContext>();
            builder.UseSqlServer(connectionString);

            return new IdentityConfigurationDbContext(builder.Options, new ConfigurationStoreOptions());

        }
    }
}
