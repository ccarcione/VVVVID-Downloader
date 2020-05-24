using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace VVVVID_Downloader.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://localhost:6000");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
