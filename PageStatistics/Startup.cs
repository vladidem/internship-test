using System.CommandLine;
using System.CommandLine.IO;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PageStatistics.Commands;
using PageStatistics.Services;
using Serilog;

namespace PageStatistics
{
    public class Startup
    {
        public static IHost BuildHost()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices(ConfigureHostServices)
                .UseSerilog(dispose: true)
                .Build();
        }

        private static void ConfigureHostServices(HostBuilderContext context, IServiceCollection services)
        {
            ConfigureLogging();
            ConfigureCliCommands(services);

            services.AddTransient<IPageLoader, PageLoader>();
            services.AddTransient<IConsole, SystemConsole>();
            services.AddTransient<ITextExtractor, TextExtractor>();
            services.AddTransient<IPageWordCounter, PageWordCounter>();
            services.AddDbContext<IPageStatisticsDbContext, PageStatisticsDbContext>(options =>
            {
                var pathToDb = Path.Join(Directory.GetCurrentDirectory(), "database.sqlite");
                options.UseSqlite($"Data Source ={pathToDb}");
            });
        }

        private static void ConfigureLogging()
        {
            var logFile = Path.Join(Directory.GetCurrentDirectory(), "log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.File(logFile)
                .WriteTo.Console()
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
