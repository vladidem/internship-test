using System.Threading.Tasks;

namespace PageStatistics.Services
{
    public interface IPageLoader
    {
        /// <summary>
        ///     Download page by its address.
        /// </summary>
        /// <param name="address">Web address of page to download.</param>
        /// <returns>Path, where downloaded page file is stored.</returns>
        public Task<string> Download(string address);
    }
}
