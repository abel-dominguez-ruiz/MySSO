using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySSO.EF.Context;
using MySSO.Entity.Entities;
using MySSO.Services.Services;
using System;
using IdentityServer4;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Interfaces;
using MySSO.EF.Context.Persisted;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework.DbContexts;
using MySSO.EF.Context.Configuration;

namespace MySSO.Configuration
{
    public static class ConfigureDbContext
    {
        public static void ConfigureOgunIdsServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Configure Providers
            var dataProtectionProviderType = typeof(DataProtectorTokenProvider<ApplicationUser>);
            var phoneNumberProviderType = typeof(PhoneNumberTokenProvider<ApplicationUser>);
            var emailTokenProviderType = typeof(EmailTokenProvider<ApplicationUser>);

            
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddSingleton(new ConfigurationStoreOptions());
            services.AddScoped<IConfigurationDbContext, IdentityConfigurationDbContext>();
            services.AddDbContext<IdentityConfigurationDbContext>(
                options => options.UseSqlServer(connectionString,
                                                op => op.EnableRetryOnFailure(maxRetryCount: 5,
                                                                              maxRetryDelay: TimeSpan.FromSeconds(30),
                                                                              errorNumbersToAdd: null)));
            services.AddDbContext<ConfigurationDbContext>(options => options.UseSqlServer(connectionString));

            services.AddSingleton(new OperationalStoreOptions { EnableTokenCleanup = false });
            services.AddScoped<IPersistedGrantDbContext, IdentityPersistedGrantDbContext>();
            services.AddDbContext<IdentityPersistedGrantDbContext>(
                options => options.UseSqlServer(connectionString,
                                                op => op.EnableRetryOnFailure(maxRetryCount: 5,
                                                                              maxRetryDelay: TimeSpan.FromSeconds(30),
                                                                              errorNumbersToAdd: null))
            );
            services.AddDbContext<PersistedGrantDbContext>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<IdentityServerDbContext>(options => options.UseSqlServer(connectionString));
            services.AddIdentity<ApplicationUser, IdentityRole>(x =>
            {
                x.Lockout.MaxFailedAccessAttempts = 5;
                x.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                x.Lockout.AllowedForNewUsers = true;
                x.User.RequireUniqueEmail = true;
                x.Password.RequiredLength = 6;
                x.Password.RequireDigit = false;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<IdentityServerDbContext>()
                .AddTokenProvider(TokenOptions.DefaultProvider, dataProtectionProviderType)
                .AddTokenProvider(TokenOptions.DefaultEmailProvider, emailTokenProviderType);

            ////Configure Identity server SQL connections and persist
            //services.AddIdentityServer(opts =>
            //{
            //    if (configuration["IssuerUri"] != null)
            //    {
            //        opts.IssuerUri = configuration["IssuerUri"];
            //    }
            //})
            //    //.AddSigningCredential("CN=sts")
            //    .AddTemporarySigningCredential()
            //    .AddOperationalStore(builder => builder.UseSqlServer(connectionString,
            //        options => options.MigrationsAssembly(migrationsAssembly)))
            //    .AddConfigurationStore(builder => builder.UseSqlServer(connectionString,
            //        options => options.MigrationsAssembly(migrationsAssembly)))
            //    .AddAspNetIdentity<ApplicationUser>()
            //    .AddProfileService<SSOUserProfileService>();

        }
    }
}
