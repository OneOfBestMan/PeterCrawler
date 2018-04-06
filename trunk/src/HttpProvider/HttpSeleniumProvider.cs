using CrawlerSelenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleCrawler
{
    public class HttpSeleniumProvider : IHtmlProvider
    {
        public event AddUrlEventHandler AddUrlEvent;
        public event CrawlErrorEventHandler CrawlErrorEvent;
        public event DataReceivedEventHandler DataReceivedEvent;

        public void DealHtml(UrlInfo urlInfo, CrawlSettings settings, CookieContainer cookieContainer)
        {
           var html= RequestManager.GetHtmlContent(urlInfo.UrlString);
            ParseLinks(urlInfo, html, settings);
            if (this.DataReceivedEvent != null)
            {
                this.DataReceivedEvent(
                    new DataReceivedEventArgs
                    {
                        Url = urlInfo.UrlString,
                        Depth = urlInfo.Depth,
                        Html = html
                    });
            }
        }

        private void ParseLinks(UrlInfo urlInfo, string html, CrawlSettings settings)
        {
            if (settings.Depth > 0 && urlInfo.Depth >= settings.Depth)
            {
                return;
            }

            var urlDictionary = new Dictionary<string, string>();

            Match match = Regex.Match(html, "(?i)<a .*?href=\"([^\"]+)\"[^>]*>(.*?)</a>");
            while (match.Success)
            {
                // 以 href 作为 key
                string urlKey = match.Groups[1].Value;

                // 以 text 作为 value
                string urlValue = Regex.Replace(match.Groups[2].Value, "(?i)<.*?>", string.Empty);

                urlDictionary[urlKey] = urlValue;
                match = match.NextMatch();
            }

            foreach (var item in urlDictionary)
            {
                string href = item.Key;
                string text = item.Value;

                if (!string.IsNullOrEmpty(href))
                {
                    bool canBeAdd = true;

                    if (settings.EscapeLinks != null && settings.EscapeLinks.Count > 0)
                    {
                        if (settings.EscapeLinks.Any(suffix => href.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
                        {
                            canBeAdd = false;
                        }
                    }

                    if (settings.HrefKeywords != null && settings.HrefKeywords.Count > 0)
                    {
                        if (!settings.HrefKeywords.Any(href.Contains))
                        {
                            canBeAdd = false;
                        }
                    }

                    if (canBeAdd)
                    {
                        string url = href.Replace("%3f", "?")
                            .Replace("%3d", "=")
                            .Replace("%2f", "/")
                            .Replace("&amp;", "&");

                        if (string.IsNullOrEmpty(url) || url.StartsWith("#")
                            || url.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase)
                            || url.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        var baseUri = new Uri(urlInfo.UrlString);
                        Uri currentUri = url.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                                             ? new Uri(url)
                                             : new Uri(baseUri, url);

                        url = currentUri.AbsoluteUri;

                        if (settings.LockHost)
                        {
                            // 去除二级域名后，判断域名是否相等，相等则认为是同一个站点
                            // 例如：mail.pzcast.com 和 www.pzcast.com
                            if (currentUri!=null && !string.IsNullOrEmpty(currentUri.Host) && currentUri.Host.Split('.').Length>1)
                            {
                                if (baseUri.Host.Split('.').Skip(1).Aggregate((a, b) => a + "." + b)
                                != currentUri.Host.Split('.').Skip(1).Aggregate((a, b) => a + "." + b))
                                {
                                    continue;
                                }
                            }
                        }
                        //非基础url的过滤
                        if (settings.LockBaseUrl)
                        {
                            if (!currentUri.AbsoluteUri.Contains(baseUri.AbsolutePath))
                            {
                                continue;
                            }
                        }

                        if (!IsMatchRegular(url, settings))
                        {
                            continue;
                        }

                        var addUrlEventArgs = new AddUrlEventArgs { Title = text, Depth = urlInfo.Depth + 1, Url = url };
                        if (this.AddUrlEvent != null && !this.AddUrlEvent(addUrlEventArgs))
                        {
                            continue;
                        }

                        UrlQueue.Instance.EnQueue(new UrlInfo(url) { Depth = urlInfo.Depth + 1 });
                    }
                }
            }
        }

        private bool IsMatchRegular(string url, CrawlSettings settings)
        {
            bool result = false;

            if (settings.RegularFilterExpressions != null && settings.RegularFilterExpressions.Count > 0)
            {
                if (
                    settings.RegularFilterExpressions.Any(
                        pattern => Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase)))
                {
                    result = true;
                }
            }
            else
            {
                result = true;
            }

            return result;
        }
    }
}
