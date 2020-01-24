using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySSO.Application.Configuration;
using MySSO.EF.Data;
using System;
using System.Linq;

namespace MySSO
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetService<IConfiguration>();
                var hostingEnvironment = services.GetService<IWebHostEnvironment>();
                try
                {
                    var clients = DefaultClientsConfig.Get(configuration).Select(x => x.ToEntity());
                    var apiResources = DefaultApiResourcesConfig.Get().Select(x=> x.ToEntity());
                    var identityResources = DefaultIdentityResources.Get().Select(x => x.ToEntity());
                    var defaultUsers = DefaultUsersConfig.Get();


                    DbInitializer.InitializeDatabase(services, identityResources, apiResources, clients, defaultUsers).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(serverOptions =>
                    {
                        // Set properties and call methods on options
                    }).UseIISIntegration();
                    webBuilder.UseStartup<Startup>();
                });
    }
}
