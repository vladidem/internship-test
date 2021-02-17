using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PageStatistics
{
    internal class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var services = host.Services;
            var logger = services.GetService<ILogger<Program>>();
            var cliParser = BuildCliParser(services);

            try
            {
                logger.Log(LogLevel.Information, "Ensuring database is up to date");
                EnsureDatabaseUpToDate(services);

                logger.Log(LogLevel.Information, "Starting console app");
                return await cliParser.InvokeAsync(args).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, "Critical error");
                return 0;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Startup.CreateHostBuilder(args);

        private static Parser BuildCliParser(IServiceProvider services)
        {
            var commandLineBuilder = new CommandLineBuilder();

            foreach (var command in services.GetServices<Command>())
            {
                commandLineBuilder.AddCommand(command);
            }

            return commandLineBuilder.UseDefaults().Build();
        }

        private static void EnsureDatabaseUpToDate(IServiceProvider services)
        {
            var db = services.GetService<IPageStatisticsDbContext>().Database;
            db.EnsureCreated();
        }
    }
}
