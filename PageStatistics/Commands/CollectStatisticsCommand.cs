using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Linq;
using System.Threading.Tasks;
using ConsoleTables;
using Microsoft.Extensions.Logging;
using PageStatistics.Models;
using PageStatistics.Services;
using PageStatistics.Infrastructure;

namespace PageStatistics.Commands
{
    public class CollectStatisticsCommand : Command
    {
        private const string CommandName = "collect";
        private const string CommandDescription = "Download page by address and collect word statistics";

        private readonly IConsole _console;
        private readonly ITextSplitter _splitter;
        private readonly IPageStatisticsDbContext _dbContext;
        private readonly ITextExtractor _extractor;
        private readonly IPageLoader _loader;
        private readonly ILogger<EchoCommand> _logger;
        private readonly IWordStatistics _wordStatistics;

        public CollectStatisticsCommand(
            ILogger<EchoCommand> logger,
            IPageLoader loader,
            ITextExtractor extractor,
            ITextSplitter splitter,
            IConsole console,
            IWordStatistics wordStatistics,
            IPageStatisticsDbContext dbContext
        ) : base(CommandName, CommandDescription)
        {
            _logger = logger;
            _loader = loader;
            _extractor = extractor;
            _splitter = splitter;
            _console = console;
            _dbContext = dbContext;
            _wordStatistics = wordStatistics;

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
            var page = await _loader.Create(address);
            _wordStatistics.Page = page;

            foreach (var text in _extractor.Extract(page))
            foreach (var word in _splitter.SplitText(text))
            {
                _wordStatistics.AddWord(word);
            }

            var wordFrequencies = _wordStatistics.ToWordFrequencyList();

            PrintWordFrequencies(wordFrequencies);

            return 0;
        }


        private void PrintWordFrequencies(List<WordFrequency> wordFrequencies)
        {
            var table = new ConsoleTable("Word", "Frequency");
            foreach (var wordFrequency in wordFrequencies.OrderByDescending(wordFrequency => wordFrequency.Frequency))
            {
                table.AddRow(wordFrequency.Word.Name, wordFrequency.Frequency);
            }

            _console.Out.WriteLine("Words statistics:");
            _console.Out.WriteLine(table.ToString());
        }
    }
}
