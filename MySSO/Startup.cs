using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySSO.Configuration;
using MySSO.EF.Context;
using MySSO.EF.Context.Configuration;
using MySSO.EF.Context.Persisted;
using MySSO.Entity.Entities;

namespace MySSO
{
    public class Startup
    {
        private bool IsDev = false;

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                                    .SetBasePath(env.ContentRootPath)
                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                                     .AddEnvironmentVariables();

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
#if DEBUG
            IsDev = true;
#endif
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureOgunIdsServices(Configuration);
            services.AddIdentityServer()
                 .AddInMemoryCaching()
                .AddClientStore<InMemoryClientStore>()
                .AddResourceStore<InMemoryResourcesStore>();
            services.AddControllers();
        }

        private async Task InitializeDatabaseAsync(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                foreach (var contextType in new[]
                {
                    typeof(IdentityServerDbContext),
                    typeof(IdentityPersistedGrantDbContext),
                    typeof(IdentityConfigurationDbContext)
                })
                {
                    await ((DbContext)serviceScope.ServiceProvider.GetRequiredService(contextType)).Database
                        .MigrateAsync();
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Task.Run(async () => await InitializeDatabaseAsync(app)).Wait();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("default");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
            app.UseIdentityServer();
        }
    }
}
