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

namespace PageStatistics.Commands
{
    public class CollectStatisticsCommand : Command
    {
        private const string CommandName = "collect";
        private const string CommandDescription = "Download page by address and collect word statistics";

        private readonly IConsole _console;
        private readonly IPageWordCounter _counter;
        private readonly IPageStatisticsDbContext _dbContext;
        private readonly ITextExtractor _extractor;
        private readonly IPageLoader _loader;
        private readonly ILogger<EchoCommand> _logger;

        private Page _page;

        public CollectStatisticsCommand(
            ILogger<EchoCommand> logger,
            IPageLoader loader,
            ITextExtractor extractor,
            IPageWordCounter counter,
            IConsole console,
            IPageStatisticsDbContext dbContext
        ) : base(CommandName, CommandDescription)
        {
            _logger = logger;
            _loader = loader;
            _extractor = extractor;
            _counter = counter;
            _console = console;
            _dbContext = dbContext;

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
            _page = await _loader.Create(address);
            _dbContext.Pages.SingleInsert(_page);

            foreach (var text in _extractor.Extract(_page))
            {
                _counter.AddText(text);
            }

            var wordFrequencies = StatisticsToWordFrequencyList(_counter.Statistics);
            _dbContext.WordFrequencies.BulkInsert(wordFrequencies, options => { options.BatchSize = 250; });

            PrintWordFrequencies(wordFrequencies);

            return 0;
        }

        private List<WordFrequency> StatisticsToWordFrequencyList(Dictionary<string, int> statistics)
        {
            var words = CreateWordsByName(statistics.Select(pair => pair.Key).ToList());
            return words.Select(word =>
                    new WordFrequency
                    {
                        PageId = _page.Id, Page = _page, Word = word, WordId = word.Id,
                        Frequency = statistics[word.Name]
                    }
                )
                .ToList();
        }

        private List<Word> CreateWordsByName(List<string> wordNames)
        {
            var existingWords = _dbContext.Words.Where(word => wordNames.Contains(word.Name)).ToList();
            var existingWordNames = existingWords.Select(word => word.Name).ToList();
            var createdWords = new List<Word>();

            foreach (var wordName in wordNames.Except(existingWordNames))
            {
                var word = new Word {Name = wordName};
                createdWords.Add(word);
            }

            _dbContext.Words.BulkInsert(createdWords, options => { options.BatchSize = 250; });
            existingWords.AddRange(createdWords);
            return existingWords;
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
