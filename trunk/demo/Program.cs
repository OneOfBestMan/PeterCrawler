// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="pzcast">
//   (C) 2015 pzcast. All rights reserved.
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimpleCrawler.Demo
{
    using HtmlAgilityPack;
    using ScrapySharp.Extensions;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        #region Static Fields

        /// <summary>
        /// The settings.
        /// </summary>
        private static readonly CrawlSettings Settings = new CrawlSettings();

        /// <summary>
        /// The filter.
        /// 关于使用 Bloom 算法去除重复 URL：http://www.cnblogs.com/heaad/archive/2011/01/02/1924195.html
        /// </summary>
        private static BloomFilter<string> filter;

        private static int TaskId { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Main(string[] args)
        {
            Bootstrapper.Start();
            filter = new BloomFilter<string>(200000);
            const string CityName = "beijing";

            // 设置种子地址
            string url = "http://www.cnblogs.com/vipyoumay/";
            Settings.SeedsAddress.Add(url);

            // 设置 URL 关键字
            //Settings.HrefKeywords.Add(string.Format("/{0}/bj", CityName));
            //Settings.HrefKeywords.Add(string.Format("/{0}/sj", CityName));

            // 设置爬取线程个数
            Settings.ThreadCount = 1;

            // 设置爬取深度
            Settings.Depth = 6;

            // 设置爬取时忽略的 Link，通过后缀名的方式，可以添加多个
            Settings.EscapeLinks.Add(".jpg");

            // 设置自动限速，1~5 秒随机间隔的自动限速
            Settings.AutoSpeedLimit = false;

            // 设置都是锁定域名,去除二级域名后，判断域名是否相等，相等则认为是同一个站点
            // 例如：mail.pzcast.com 和 www.pzcast.com
            Settings.LockHost = true;
            Settings.LockBaseUrl = true;
            // 设置请求的 User-Agent HTTP 标头的值
            // settings.UserAgent 已提供默认值，如有特殊需求则自行设置

            // 设置请求页面的超时时间，默认值 15000 毫秒
            // settings.Timeout 按照自己的要求确定超时时间

            // 设置用于过滤的正则表达式
            // settings.RegularFilterExpressions.Add("");
            var master = new CrawlMaster(Settings);
            master.HtmlProvider.AddUrlEvent += MasterAddUrlEvent;
            //master.AddUrlEvent += MasterAddUrlEvent;
            master.HtmlProvider.DataReceivedEvent += MasterDataReceivedEvent;
            master.Crawl();
            CreateTask(url, url);
            Console.ReadKey();
        }

        /// <summary>
        /// The master add url event.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool MasterAddUrlEvent(AddUrlEventArgs args)
        {
            if (!filter.Contains(args.Url))
            {
                filter.Add(args.Url);
                Console.WriteLine(args.Url);
                return true;
            }

            return false; // 返回 false 代表：不添加到队列中
        }

        /// <summary>
        /// The master data received event.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void MasterDataReceivedEvent(DataReceivedEventArgs args)
        {
            // 在此处解析页面，可以用类似于 HtmlAgilityPack（页面解析组件）的东东、也可以用正则表达式、还可以自己进行字符串分析

            //Console.WriteLine(args.Html);
            var selector = "div#topics";
            if (!string.IsNullOrEmpty(args.Html))
            {
                var doc = GetDocument(args.Html);
                var list = GetNodes(doc, selector);

                var title = doc.DocumentNode.CssSelect("title");
                var titleString = title.FirstOrDefault().InnerHtml;
                //foreach (var htmlNode in title)
                //{
                //    titleString = htmlNode.InnerHtml;
                //    Console.WriteLine(htmlNode.InnerHtml);
                //}


                // Console.WriteLine(list.FirstOrDefault().InnerHtml);
                CreateHtml(args.Url, titleString, args.Depth, args.Html, list);
                Console.WriteLine(args.Url + " 下载完毕");
                //Console.Read();
            }

        }

        private static void CreateHtml(string url, int depth, string html, IList<HtmlNode> list)
        {
            throw new NotImplementedException();
        }

        private static HtmlDocument GetDocument(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }

        private static string GetNodes(HtmlDocument doc, string selector)
        {
            var docNode = doc.DocumentNode;

            var node = docNode.CssSelect(selector).FirstOrDefault();
            if (node!=null)
            {
                return node.InnerHtml;
            }
            return "";
        }

        private static void CreateHtml(string url, string title, int depth, string html, string nodes)
        {
            c_TaskDetails detail = new c_TaskDetails();
            c_TaskDetailsInfo info = new c_TaskDetailsInfo()
            {
                TaskId = TaskId,
                Title = title,
                CrawUrl = url,
                RawHtml = html,
                SelectHtml = nodes,
                Depth = depth,
                CreationTime = DateTime.Now
            };
            detail.Insert(info);
        }

        private static void CreateTask(string name, string baseUrl)
        {
            c_Tasks task = new c_Tasks();
            c_TasksInfo info = new c_TasksInfo();
            info.TaskName = name;
            info.BaseUrl = baseUrl;
            info.CreationTime = DateTime.Now;
            TaskId = task.Insert(info);
        }



        #endregion
    }
}