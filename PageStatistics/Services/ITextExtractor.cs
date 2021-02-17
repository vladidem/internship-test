using System.Collections.Generic;

namespace PageStatistics.Services
{
    public interface ITextExtractor
    {
        public IEnumerable<string> Extract(string fileName);
    }
}
