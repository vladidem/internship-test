using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Linq;
using System.Threading.Tasks;
using ConsoleTables;
using Microsoft.Extensions.Logging;
using PageStatistics.Services;

namespace PageStatistics.Commands
{
    public class CollectStatisticsCommand : Command
    {
        private const string CommandName = "collect";
        private const string CommandDescription = "Download page by address and collect word statistics";

        private readonly IConsole _console;
        private readonly IPageWordCounter _counter;
        private readonly ITextExtractor _extractor;
        private readonly IPageLoader _loader;
        private readonly ILogger<EchoCommand> _logger;

        public CollectStatisticsCommand(
            ILogger<EchoCommand> logger,
            IPageLoader loader,
            ITextExtractor extractor,
            IPageWordCounter counter,
            IConsole console
        ) : base(CommandName, CommandDescription)
        {
            _logger = logger;
            _loader = loader;
            _extractor = extractor;
            _counter = counter;
            _console = console;

            ConfigureCommand();
        }

        private void ConfigureCommand()
        {
            var addressArgument = new Argument<string>
            {
                Name = "address"
            };

            AddArgument(addressArgument);

            Handler = CommandHandler.Create(
                (Func<string, Task<int>>) HandleCommand);
        }

        private async Task<int> HandleCommand(string address)
        {
            _logger.Log(LogLevel.Information, $"Started downloading page {address}");
            var fileName = await _loader.Download(address);

            foreach (var text in _extractor.Extract(fileName))
            {
                _counter.AddText(text);
            }

            PrintStatistics(_counter.Statistics);

            return 0;
        }

        private void PrintStatistics(Dictionary<string, int> statistics)
        {
            var table = new ConsoleTable("Word", "Frequency");
            foreach (var (word, count) in statistics.ToList().OrderByDescending(keyValue => keyValue.Value))
            {
                table.AddRow(word, count);
            }

            _console.Out.WriteLine("Words statistics:");
            _console.Out.WriteLine(table.ToString());
        }
    }
}
