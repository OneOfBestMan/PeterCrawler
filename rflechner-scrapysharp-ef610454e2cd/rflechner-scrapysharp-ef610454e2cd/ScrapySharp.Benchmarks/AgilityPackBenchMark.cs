using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace ScrapySharp.Benchmarks
{
    public class AgilityPackBenchMark : IBenchMark
    {
        private readonly Stopwatch stopwatch = new Stopwatch();
        
        public void Run(string source)
        {
            stopwatch.Reset();
            //var source = File.ReadAllText("Html/Page1.htm");
            stopwatch.Start();

            var htmlDocument = new HtmlDocument();
            
            for (int i = 0; i < BenchMarksParameters.Iterations; i++)
            {
                
                htmlDocument.LoadHtml(source);

                var html = htmlDocument.DocumentNode;

                var nodes = html.CssSelect("span.login-box").ToArray();
                Console.WriteLine("Matched: {0}", nodes.Length);

                nodes = html.CssSelect("span#pass-box").ToArray();
                Console.WriteLine("Matched: {0}", nodes.Length);

                nodes = html.CssSelect("script[type=text/javascript]").ToArray();
                Console.WriteLine("Matched: {0}", nodes.Length);
            }

            stopwatch.Stop();
        }

        public TimeSpan TimeElapsed
        {
            get { return stopwatch.Elapsed; }
        }
    }
}