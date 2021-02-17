using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            var words = RemoveSpecialChars(text)
                .Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                var normalizedWord = NormalizeWord(word);
                if (ShouldNotCount(normalizedWord))
                {
                    continue;
                }

                var currentCount = Statistics.ContainsKey(normalizedWord) ? Statistics[normalizedWord] : 0;
                Statistics[normalizedWord] = currentCount + 1;
            }
        }

        public Dictionary<string, int> Statistics { get; }

        private static string RemoveSpecialChars(string text)
        {
            var stringBuilder = new StringBuilder();
            foreach (var c in text)
            {
                if (!char.IsControl(c) &&
                    !char.IsSeparator(c) &&
                    !char.IsSurrogate(c))
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append(" ");
                }
            }

            return stringBuilder.ToString();
        }

        private static string NormalizeWord(string word)
        {
            return word.ToLower();
        }

        private static bool ShouldNotCount(string word)
        {
            return word == null || !word.Any(char.IsLetterOrDigit);
        }
    }
}
