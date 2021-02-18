using System.Collections.Generic;

namespace PageStatistics.Services
{
    public interface ITextSplitter
    {
        /// <summary>
        ///     Split text into words and normalize them.
        /// </summary>
        /// <param name="text">Text to split into words.</param>
        /// <returns>IEnumerable with words.</returns>
        public IEnumerable<string> SplitText(string text);
    }
}
