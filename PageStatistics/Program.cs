using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace PageStatistics
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(ConfigureHostServices)
                .UseSerilog(dispose: true)
                .Build();
            var app = host.Services.GetService<App>();
            app.Run();
        }

        private static void ConfigureHostServices(HostBuilderContext context, IServiceCollection services)
        {
            ConfigureLogging();
            services.AddTransient<App>();
        }

        private static void ConfigureLogging()
        {
            var logFile = Path.Join(Directory.GetCurrentDirectory(), "log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.File(logFile)
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}
