using System.CommandLine;
using System.CommandLine.IO;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PageStatistics.Commands;
using PageStatistics.Infrastructure;
using PageStatistics.Services;
using Serilog;

namespace PageStatistics
{
    public class Startup
    {
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureHostServices)
                .UseSerilog(dispose: true);
        }

        private static void ConfigureHostServices(HostBuilderContext context, IServiceCollection services)
        {
            ConfigureLogging();
            ConfigureCliCommands(services);

            services.AddTransient<IPageLoader, PageLoader>();
            services.AddTransient<IConsole, SystemConsole>();
            services.AddTransient<ITextExtractor, TextExtractor>();
            services.AddTransient<ITextSplitter, TextSplitter>();
            services.AddTransient<IWordStatistics, WordStatistics>();
            services.AddDbContext<IPageStatisticsDbContext, PageStatisticsDbContext>(options =>
                {
                    var pathToDb = Path.Join(Directory.GetCurrentDirectory(), ".data", "database.sqlite");
                    options.UseSqlite($"Data Source ={pathToDb}");
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                }
            );
        }

        private static void ConfigureLogging()
        {
            var logFile = Path.Join(Directory.GetCurrentDirectory(), ".data", "log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.File(logFile)
                .CreateLogger();
        }

        private static void ConfigureCliCommands(IServiceCollection services)
        {
            var echoCommandType = typeof(EchoCommand);
            var commandType = typeof(Command);

            var commands = echoCommandType
                .Assembly
                .GetExportedTypes()
                .Where(x => x.Namespace == echoCommandType.Namespace && commandType.IsAssignableFrom(x));

            foreach (var command in commands)
            {
                services.AddSingleton(commandType, command);
            }
        }
    }
}
