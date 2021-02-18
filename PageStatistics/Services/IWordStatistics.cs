using System.Collections.Generic;
using PageStatistics.Models;

namespace PageStatistics.Services
{
    public interface IWordStatistics
    {
        /// <summary>
        ///     Add word to statistics.
        /// </summary>
        public void AddWord(string word);

        /// <summary>
        ///     Page, which statistics are collected.
        /// </summary>
        public Page Page { get; set; }

        /// <summary>
        ///     Transform collected statistic into list of WordFrequency models, store them.
        /// </summary>
        public List<WordFrequency> ToWordFrequencyList();
    }
}
