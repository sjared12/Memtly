using Memtly.Core;
using Memtly.Core.Enums;
using Microsoft.AspNetCore;

namespace Memtly.Community
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MemtlyCore.Version = MemtlyVersion.Community;
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseIISIntegration()
                .UseIIS()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>();

            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
            {
                var portValue = Environment.GetEnvironmentVariable("PORT");
                var port = int.TryParse(portValue, out var parsedPort) ? parsedPort : 5000;

                webHostBuilder.UseUrls($"http://*:{port}");
            }

            return webHostBuilder;
        }
    }
}