using System;
using System.Collections.Generic;
using System.Linq;
using PageStatistics.Infrastructure;
using PageStatistics.Models;

namespace PageStatistics.Services
{
    public class WordStatistics : IWordStatistics
    {
        private readonly IPageStatisticsDbContext _dbContext;
        private readonly Dictionary<string, int> _statistics = new Dictionary<string, int>();

        public WordStatistics(IPageStatisticsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Page Page { set; get; }

        public void AddWord(string word)
        {
            var currentCount = _statistics.ContainsKey(word) ? _statistics[word] : 0;
            _statistics[word] = currentCount + 1;
        }

        public List<WordFrequency> ToWordFrequencyList()
        {
            if (Page == null)
            {
                throw new Exception("Set Page before storing statistics.");
            }

            var words = CreateWordsByNames(_statistics.Select(pair => pair.Key).ToList());
            var wordFrequencies = words.Select(word =>
                    new WordFrequency
                    {
                        PageId = Page.Id, Page = Page, Word = word, WordId = word.Id,
                        Frequency = _statistics[word.Name]
                    }
                )
                .ToList();
            _dbContext
                .WordFrequencies
                .BulkInsert(
                    wordFrequencies,
                    options => { options.BatchSize = 250; }
                );

            return wordFrequencies;
        }

        private List<Word> CreateWordsByNames(List<string> wordNames)
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
    }
}
