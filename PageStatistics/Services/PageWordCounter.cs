using System;
using System.Collections.Generic;

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
            ';',
            ':',
            '[',
            ']',
            '(',
            ')',
            '}',
            '{',
            '=',
            '+',
            '>',
            '<',
            '^',
            '&',
            '\\',
            '/',
            '-'
        };

        public PageWordCounter()
        {
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
