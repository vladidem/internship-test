using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading.Tasks;
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

            _logger.Log(LogLevel.Information, $"Extracting text from html file {fileName}");
            var text = _extractor.Extract(fileName);

            _logger.Log(LogLevel.Information, $"Counting word on page {address}");
            _counter.AddText(text);

            PrintStatistics(_counter.Statistics);

            return 1;
        }

        private void PrintStatistics(Dictionary<string, int> statistics)
        {
            foreach (var (word, count) in statistics.ToList().OrderByDescending(keyValue => keyValue.Value))
            {
                _console.Out.Write($"{word} :\t {count}\n");
            }
        }
    }
}
