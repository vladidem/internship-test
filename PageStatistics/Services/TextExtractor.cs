using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using HtmlAgilityPack;
using PageStatistics.Models;

namespace PageStatistics.Services
{
    public class TextExtractor : ITextExtractor
    {
        private const string TagSeparator = " ";
        private static readonly List<string> IgnoredNodes = new List<string> {"head", "script", "style"};

        public string Extract(Page page)
        {
            var html = File.ReadAllText(page.FileName);
            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(html);
            var sb = new StringBuilder();
            BuildNodeText(htmlDoc.DocumentNode, sb);

            return sb.ToString();
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
        private void BuildNodeText(HtmlNode node, StringBuilder sb)
        {
            if (ShouldSkip(node))
            {
                return;
            }

            if (node.NodeType == HtmlNodeType.Text)
            {
                sb.Append(HttpUtility.HtmlDecode(((HtmlTextNode) node).Text));
                sb.Append(TagSeparator);
                return;
            }

            foreach (var childNode in node.ChildNodes)
            {
                BuildNodeText(childNode, sb);
            }
        }

        private bool ShouldSkip(HtmlNode node)
        {
            var name = (node.Name ?? "").ToLower();
            return node.NodeType == HtmlNodeType.Comment || IgnoredNodes.Contains(name);
        }
    }
}
