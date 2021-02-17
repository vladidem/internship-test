using System.Collections.Generic;

namespace PageStatistics.Services
{
    public interface ITextExtractor
    {
        /// <summary>
        ///     Extract text from html file.
        /// </summary>
        /// <param name="fileName">Path to html file.</param>
        /// <returns>Text parts of html file.</returns>
        public IEnumerable<string> Extract(string fileName);
    }
}
