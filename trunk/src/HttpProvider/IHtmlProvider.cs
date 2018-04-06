using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace SimpleCrawler
{
    public interface IHtmlProvider
    {
        /// <summary>
        /// The add url event.
        /// </summary>
        event AddUrlEventHandler AddUrlEvent;

        /// <summary>
        /// The crawl error event.
        /// </summary>
        event CrawlErrorEventHandler CrawlErrorEvent;

        /// <summary>
        /// The data received event.
        /// </summary>
        event DataReceivedEventHandler DataReceivedEvent;
        void DealHtml(UrlInfo urlInfo, CrawlSettings settings, CookieContainer cookieContainer);


    }
}
