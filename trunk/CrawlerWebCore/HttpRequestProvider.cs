using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleCrawler
{
    public class HttpRequestProvider : IHtmlProvider
    {
        public event AddUrlEventHandler AddUrlEvent;
        public event CrawlErrorEventHandler CrawlErrorEvent;
        public event DataReceivedEventHandler DataReceivedEvent;

        public void DealHtml(UrlInfo urlInfo, CrawlSettings settings, CookieContainer cookieContainer)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try
            {
                if (urlInfo==null)
                {
                    return ;
                }

                // 创建并配置Web请求
                request = WebRequest.Create(urlInfo.UrlString) as HttpWebRequest;
                this.ConfigRequest(request, settings, cookieContainer);

                if (request != null)
                {
                    response = request.GetResponse() as HttpWebResponse;
                }

                if (response != null)
                {
                    PersistenceCookie(response, settings, cookieContainer);

                    Stream stream = null;

                    // 如果页面压缩，则解压数据流
                    if (response.ContentEncoding == "gzip")
                    {
                        Stream responseStream = response.GetResponseStream();
                        if (responseStream != null)
                        {
                            stream = new GZipStream(responseStream, CompressionMode.Decompress);
                        }
                    }
                    else
                    {
                        stream = response.GetResponseStream();
                    }

                    using (stream)
                    {
                        string html = this.ParseContent(stream, response.CharacterSet);

                        ParseLinks(urlInfo, html,settings);

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

                        if (stream != null)
                        {
                            stream.Close();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                if (this.CrawlErrorEvent != null)
                {
                    if (urlInfo != null)
                    {
                        this.CrawlErrorEvent(
                            new CrawlErrorEventArgs { Url = urlInfo.UrlString, Exception = exception });
                    }
                }
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }

                if (response != null)
                {
                    response.Close();
                }
            }
        }

        private void ConfigRequest(HttpWebRequest request, CrawlSettings settings, CookieContainer cookieContainer)
        {
            request.UserAgent = settings.UserAgent;
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = true;
            request.MediaType = "text/html";
            request.Headers["Accept-Language"] = "zh-CN,zh;q=0.8";
            if (settings.Timeout > 0)
            {
                request.Timeout = settings.Timeout;
            }
        }

        /// <summary>
        /// The persistence cookie.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        private void PersistenceCookie(HttpWebResponse response, CrawlSettings settings, CookieContainer cookieContainer)
        {
            if (settings.KeepCookie)
            {
                return;
            }

            string cookies = response.Headers["Set-Cookie"];
            if (!string.IsNullOrEmpty(cookies))
            {
                var cookieUri =
                    new Uri(
                        string.Format(
                            "{0}://{1}:{2}/",
                            response.ResponseUri.Scheme,
                            response.ResponseUri.Host,
                            response.ResponseUri.Port));

                cookieContainer.SetCookies(cookieUri, cookies); 
            }
        }

        private string ParseContent(Stream stream, string characterSet)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            byte[] buffer = memoryStream.ToArray();

            Encoding encode = Encoding.ASCII;
            string html = encode.GetString(buffer);

            string localCharacterSet = characterSet;

            Match match = Regex.Match(html, "<meta([^<]*)charset=([^<]*)\"", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                localCharacterSet = match.Groups[2].Value;

                var stringBuilder = new StringBuilder();
                foreach (char item in localCharacterSet)
                {
                    if (item == ' ')
                    {
                        break;
                    }

                    if (item != '\"')
                    {
                        stringBuilder.Append(item);
                    }
                }

                localCharacterSet = stringBuilder.ToString();
            }

            if (string.IsNullOrEmpty(localCharacterSet))
            {
                localCharacterSet = characterSet;
            }

            if (!string.IsNullOrEmpty(localCharacterSet))
            {
                encode = Encoding.GetEncoding(localCharacterSet);
            }

            memoryStream.Close();

            return encode.GetString(buffer);
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
                            if (baseUri.Host.Split('.').Skip(1).Aggregate((a, b) => a + "." + b)
                                != currentUri.Host.Split('.').Skip(1).Aggregate((a, b) => a + "." + b))
                            {
                                continue;
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

                        if (!IsMatchRegular(url,settings))
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
