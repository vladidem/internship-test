using System.Collections.Generic;
using PageStatistics.Models;

namespace PageStatistics.Services
{
    public interface ITextExtractor
    {
        /// <summary>
        ///     Extract text from html Page.
        /// </summary>
        /// <param name="page">Page model.</param>
        /// <returns>Text parts of html file.</returns>
        public IEnumerable<string> Extract(Page page);
    }
}
