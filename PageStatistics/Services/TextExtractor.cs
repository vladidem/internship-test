using System.Collections.Generic;
using System.IO;
using System.Web;
using HtmlAgilityPack;

namespace PageStatistics.Services
{
    public class TextExtractor : ITextExtractor
    {
        private static readonly List<string> IgnoredNodes = new List<string> {"head", "script", "style"};

        public IEnumerable<string> Extract(string fileName)
        {
            var html = File.ReadAllText(fileName);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            return NodeText(htmlDoc.DocumentNode);
        }

        /// <summary>
        ///     Extract text from HtmlNode.
        /// </summary>
        /// <remarks>
        ///     DocumentNode.InnerText would be shorter, but it works incorrectly in certain cases.
        ///     It concatenates words from different tags. For example:
        ///     <code>
        ///         htmlDoc.LoadHtml("&lt;ul&gt;&lt;li&gt;first&lt;/li&gt;&lt;li&gt;second>&lt;/li&gt;&lt;/ul&gt;");
        ///         htmlDoc.DocumentNode.InnerText
        ///     </code>
        ///     returns "firstsecond" instead of separate words.
        /// </remarks>
        private IEnumerable<string> NodeText(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Text)
            {
                yield return HttpUtility.HtmlDecode(((HtmlTextNode) node).Text);
                yield break;
            }

            if (ShouldSkip(node))
            {
                yield break;
            }

            foreach (var childNode in node.ChildNodes)
            foreach (var text in NodeText(childNode))
            {
                yield return text;
            }
        }

        private bool ShouldSkip(HtmlNode node)
        {
            var name = (node.Name ?? "").ToLower();
            return node.NodeType == HtmlNodeType.Comment || IgnoredNodes.Contains(name);
        }
    }
}
