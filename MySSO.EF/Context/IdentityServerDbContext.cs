using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using MySSO.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MySSO.EF.Context
{
    public class IdentityServerDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly string _connectionString;

        public IdentityServerDbContext(DbContextOptions<IdentityServerDbContext> options) : base(options)
        {
            var extension = options.FindExtension<SqlServerOptionsExtension>();
            _connectionString = extension.ConnectionString;
            //_connectionString = Configure.ConfigurationBuilder().Build().GetConnectionString("IdentityConnectionDb");
        }

        public IdentityServerDbContext(string connectionString) : base()
        {
            _connectionString = connectionString;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

    }
}
