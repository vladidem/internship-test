using System.Threading.Tasks;
using PageStatistics.Models;

namespace PageStatistics.Services
{
    public interface IPageLoader
    {
        /// <summary>
        ///     Download page by its address, create and store page model.
        /// </summary>
        /// <param name="address">Web address of page to download.</param>
        /// <returns>Made page model.</returns>
        public Task<Page> Create(string address);
    }
}
