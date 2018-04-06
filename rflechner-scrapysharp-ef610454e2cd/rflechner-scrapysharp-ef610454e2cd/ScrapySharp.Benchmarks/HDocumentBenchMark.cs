using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ScrapySharp.Extensions;
using ScrapySharp.Html.Dom;

namespace ScrapySharp.Benchmarks
{
    public class HDocumentBenchMark : IBenchMark
    {
        private readonly Stopwatch stopwatch = new Stopwatch();

        public void Run(string source)
        {
            stopwatch.Reset();
            
            stopwatch.Start();

            for (int i = 0; i < BenchMarksParameters.Iterations; i++)
            {
                var html = HDocument.Parse(source);

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