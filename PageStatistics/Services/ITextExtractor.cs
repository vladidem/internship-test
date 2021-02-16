using System.Collections.Generic;

namespace PageStatistics.Services
{
    public interface ITextExtractor
    {
        public string Extract(string fileName);
    }
}
