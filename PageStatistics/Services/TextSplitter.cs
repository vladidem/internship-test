using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PageStatistics.Services
{
    public class TextSplitter : ITextSplitter
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

        public IEnumerable<string> SplitText(string text)
        {
            var words = RemoveSpecialChars(text)
                .Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                var normalizedWord = NormalizeWord(word);
                if (ShouldCount(normalizedWord))
                {
                    yield return normalizedWord;
                }
            }
        }

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

        private static bool ShouldCount(string word)
        {
            return word != null && word.Any(char.IsLetter);
        }
    }
}
