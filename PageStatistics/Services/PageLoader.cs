using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PageStatistics.Services
{
    public class PageLoader : IPageLoader
    {
        public async Task<string> Download(string address)
        {
            var fileName = Path.Join(Directory.GetCurrentDirectory(), MakeValidFileName(address) + ".html");
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
