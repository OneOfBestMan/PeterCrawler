// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrawlMaster.cs" company="pzcast">
//   (C) 2015 pzcast. All rights reserved.
// </copyright>
// <summary>
//   The crawl master.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimpleCrawler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;

    /// <summary>
    /// The crawl master.
    /// </summary>
    public class CrawlMaster
    {
        #region Constants

        /// <summary>
        /// The web url regular expressions.
        /// </summary>
        private const string WebUrlRegularExpressions = @"^(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";

        #endregion

        #region Fields

        /// <summary>
        /// The cookie container.
        /// </summary>
        private readonly CookieContainer cookieContainer;

        /// <summary>
        /// The random.
        /// </summary>
        private readonly Random random;

        /// <summary>
        /// The thread status.
        /// </summary>
        private readonly bool[] threadStatus;

        /// <summary>
        /// The threads.
        /// </summary>
        private readonly Thread[] threads;

        private IHtmlProvider htmlProvider;


        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CrawlMaster"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public CrawlMaster(CrawlSettings settings)
        {
            this.cookieContainer = new CookieContainer();
            this.random = new Random();

            this.Settings = settings;
            this.threads = new Thread[settings.ThreadCount];
            this.threadStatus = new bool[settings.ThreadCount];
            this.htmlProvider = ContainerManager.Resolve<IHtmlProvider>();
        }

        #endregion


        #region Public Properties
        /// <summary>
        /// html处理接口，
        /// </summary>
        public IHtmlProvider HtmlProvider
        {
            get { return htmlProvider; }
            set { htmlProvider = value; }
        }
        /// <summary>
        /// Gets the settings.
        /// </summary>
        public CrawlSettings Settings { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The crawl.
        /// </summary>
        public void Crawl()
        {
            this.Initialize();

            for (int i = 0; i < this.threads.Length; i++)
            {
                this.threads[i].Start(i);
                this.threadStatus[i] = false;
            }
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            foreach (Thread thread in this.threads)
            {
                thread.Abort();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The crawl process.
        /// </summary>
        /// <param name="threadIndex">
        /// The thread index.
        /// </param>
        private void CrawlProcess(object threadIndex)
        {
            var currentThreadIndex = (int)threadIndex;
            while (true)
            {
                // 根据队列中的 Url 数量和空闲线程的数量，判断线程是睡眠还是退出
                if (UrlQueue.Instance.Count == 0)
                {
                    this.threadStatus[currentThreadIndex] = true;
                    if (!this.threadStatus.Any(t => t == false))
                    {
                        break;
                    }

                    Thread.Sleep(2000);
                    continue;
                }

                this.threadStatus[currentThreadIndex] = false;

                if (UrlQueue.Instance.Count == 0)
                {
                    continue;
                }

                UrlInfo urlInfo = UrlQueue.Instance.DeQueue();
                HtmlProvider.DealHtml(urlInfo,Settings,cookieContainer);
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        private void Initialize()
        {
            if (this.Settings.SeedsAddress != null && this.Settings.SeedsAddress.Count > 0)
            {
                foreach (string seed in this.Settings.SeedsAddress)
                {
                    if (Regex.IsMatch(seed, WebUrlRegularExpressions, RegexOptions.IgnoreCase))
                    {
                        UrlQueue.Instance.EnQueue(new UrlInfo(seed) { Depth = 1 });
                    }
                }
            }

            for (int i = 0; i < this.Settings.ThreadCount; i++)
            {
                var threadStart = new ParameterizedThreadStart(this.CrawlProcess);

                this.threads[i] = new Thread(threadStart);
            }

            ServicePointManager.DefaultConnectionLimit = 256;
        }
        #endregion
    }
}