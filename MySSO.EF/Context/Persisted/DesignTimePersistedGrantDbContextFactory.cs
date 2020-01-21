using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MySSO.EF.Context.Persisted
{
    public class DesignTimeConfigurationDbContextFactory : IDesignTimeDbContextFactory<IdentityPersistedGrantDbContext>
    {
        public IdentityPersistedGrantDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json")
             .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var builder = new DbContextOptionsBuilder<PersistedGrantDbContext>();
            builder.UseSqlServer(connectionString);

            return new IdentityPersistedGrantDbContext(builder.Options, new OperationalStoreOptions());

        }
    }
}
