using System.Collections.Generic;

namespace PageStatistics.Services
{
    public interface IPageWordCounter
    {
        /// <summary>
        ///     Dictionary with word as key and word frequency as value.
        /// </summary>
        public Dictionary<string, int> Statistics { get; }

        /// <summary>
        ///     Split text into words and add them to statistics.
        /// </summary>
        /// <param name="text">Text to split into words.</param>
        public void AddText(string text);
    }
}
