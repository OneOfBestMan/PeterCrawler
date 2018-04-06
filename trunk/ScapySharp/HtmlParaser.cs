using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScapySharp
{
    public class HtmlParaser
    {

        public HtmlDocument GetDocument(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }

        public IList<HtmlNode> GetNodes(HtmlDocument doc,string selector)
        {
            var docNode = doc.DocumentNode;

            var nodes = docNode.CssSelect(selector);
            return nodes.ToList();
        }
    }
}
