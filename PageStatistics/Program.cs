using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PageStatistics.Commands;
using PageStatistics.Services;
using Serilog;

namespace PageStatistics
{
    internal class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(ConfigureHostServices)
                .UseSerilog(dispose: true)
                .Build();
            var logger = host.Services.GetService<ILogger<Program>>();
            var cliParser = BuildCliParser(host.Services);

            try
            {
                logger.Log(LogLevel.Information, "Starting console app");
                return await cliParser.InvokeAsync(args).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, "Critical error");
                return 0;
            }
        }

        private static void ConfigureHostServices(HostBuilderContext context, IServiceCollection services)
        {
            ConfigureLogging();
            ConfigureCliCommands(services);

            services.AddTransient<IPageLoader, PageLoader>();
            services.AddTransient<IConsole, SystemConsole>();
            services.AddTransient<ITextExtractor, TextExtractor>();
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

        private static void ConfigureCliCommands(IServiceCollection services)
        {
            var echoCommandType = typeof(EchoCommand);
            var commandType = typeof(Command);

            var commands = echoCommandType
                .Assembly
                .GetExportedTypes()
                .Where(x => x.Namespace == echoCommandType.Namespace && commandType.IsAssignableFrom(x));

            foreach (var command in commands) services.AddSingleton(commandType, command);
        }

        private static Parser BuildCliParser(IServiceProvider services)
        {
            var commandLineBuilder = new CommandLineBuilder();

            foreach (var command in services.GetServices<Command>()) commandLineBuilder.AddCommand(command);

            return commandLineBuilder.UseDefaults().Build();
        }
    }
}
