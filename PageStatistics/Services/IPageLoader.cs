using System.Threading.Tasks;

namespace PageStatistics.Services
{
    public interface IPageLoader
    {
        public Task<string> Download(string address);
    }
}
