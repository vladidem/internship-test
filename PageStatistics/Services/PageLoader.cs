using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PageStatistics.Infrastructure;
using PageStatistics.Models;

namespace PageStatistics.Services
{
    public class PageLoader : IPageLoader
    {
        private readonly IPageStatisticsDbContext _dbContext;

        public PageLoader(IPageStatisticsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Page> Create(string address)
        {
            var page = new Page
            {
                FileName = await Download(address),
                Address = address,
                LoadedAt = DateTime.Now
            };
            _dbContext.Pages.SingleInsert(page);

            return page;
        }

        private async Task<string> Download(string address)
        {
            var fileName = Path.Join(Directory.GetCurrentDirectory(), ".data", MakeValidFileName(address) + ".html");
            var webClient = new WebClient();

            await webClient.DownloadFileTaskAsync(new Uri(address), fileName);

            return fileName;
        }

        private static string MakeValidFileName(string name)
        {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(name, invalidRegStr, "_");
        }
    }
}
