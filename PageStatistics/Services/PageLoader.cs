using System;
using System.CommandLine;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PageStatistics.Models;
using PageStatistics.Infrastructure;

namespace PageStatistics.Services
{
    public class PageLoader : IPageLoader
    {
        private readonly IConsole _console;
        private readonly IPageStatisticsDbContext _dbContext;
        public PageLoader(IConsole console, IPageStatisticsDbContext dbContext)
        {
            _console = console;
            _dbContext = dbContext;
        }

        public async Task<Page> Create(string address)
        {
            var page = new Page()
            {
                FileName = await Download(address),
                Address = address,
                LoadedAt = DateTime.Now,
            };
            _dbContext.Pages.SingleInsert(page);

            return page;
        }

        private async Task<string> Download(string address)
        {
            var fileName = Path.Join(Directory.GetCurrentDirectory(), MakeValidFileName(address) + ".html");
            var webClient = new WebClient();

            webClient.DownloadProgressChanged += DownloadProgressCallback;
            webClient.DownloadFileCompleted += DownloadCompletedCallback;

            await webClient.DownloadFileTaskAsync(new Uri(address), fileName);

            return fileName;
        }

        private static string MakeValidFileName(string name)
        {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(name, invalidRegStr, "_");
        }

        private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            var progress = $"\r{e.ProgressPercentage} % complete";
            _console.Out.Write(progress);
        }

        private void DownloadCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            _console.Out.Write("\n");
        }
    }
}
