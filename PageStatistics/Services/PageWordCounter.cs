using System;
using System.Collections.Generic;
using System.CommandLine;

namespace PageStatistics.Services
{
    public class PageWordCounter : IPageWordCounter
    {
        private static readonly char[] Delimiters =
        {
            ' ',
            '\n',
            '\r',
            '\t',
            ',',
            '.',
            '!',
            '?',
            '"',
            '\'',
            ';',
            ':',
            '[',
            ']',
            '(',
            ')',
            '^',
            '&',
            '\\',
            '/',
            '-'
        };

        private readonly IConsole _console;

        public PageWordCounter(IConsole console)
        {
            _console = console;
            Statistics = new Dictionary<string, int>();
        }

        public void AddText(string text)
        {
            var words = text.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                var normalizedWord = word.ToLower();
                var currentCount = Statistics.ContainsKey(normalizedWord) ? Statistics[normalizedWord] : 0;
                Statistics[normalizedWord] = currentCount + 1;
            }
        }

        public Dictionary<string, int> Statistics { get; }
    }
}
