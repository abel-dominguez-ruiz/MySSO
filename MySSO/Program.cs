using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MySSO
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

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
