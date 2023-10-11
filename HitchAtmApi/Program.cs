using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace HitchAtmApi
{
    public class Program
    {
        public static string AUTH_PATH = $"{AppDomain.CurrentDomain.BaseDirectory}Files\\Auth";

        public static void Main(string[] args)
        {
            if (File.Exists(AUTH_PATH) == false)
            {
                Directory.CreateDirectory(AUTH_PATH);
            }

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>();
    }
}
