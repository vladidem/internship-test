using System.IO;
using HtmlAgilityPack;

namespace PageStatistics.Services
{
    public class TextExtractor : ITextExtractor
    {
        public string Extract(string fileName)
        {
            var html = File.ReadAllText(fileName);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            return htmlDoc.DocumentNode.InnerText;
        }
    }
}
