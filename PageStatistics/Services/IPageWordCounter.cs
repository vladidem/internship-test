using System.Collections.Generic;

namespace PageStatistics.Services
{
    public interface IPageWordCounter
    {
        public void AddText(string text);

        public Dictionary<string, int> Statistics { get; }
    }
}
