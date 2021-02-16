using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;

namespace PageStatistics.Services
{
    public class PageWordCounter: IPageWordCounter
    {
        private static readonly Char[] Delimiters = {' ', ',', '.', '!', '?', '"', ';', ':', '[', ']', '(', ')', '\n', '\r', '\t', '\''};

        private readonly Dictionary<string, int> _statistics;
        private readonly IConsole _console;

        public PageWordCounter(IConsole console)
        {
            _console = console;
            _statistics = new Dictionary<string, int>();
        }

        public void AddText(string text)
        {
            var words = text.Split(Delimiters);

            foreach (var word in words)
            {
                var currentCount = _statistics.ContainsKey(word.ToLower()) ? _statistics[word.ToLower()] : 0;
                _statistics[word.ToLower()] = currentCount + 1;
            }
        }

        public Dictionary<string, int> Statistics => _statistics;
    }
}
