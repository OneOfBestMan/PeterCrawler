using System;
using System.IO;

namespace ScrapySharp.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var agilityPackBenchMark = new AgilityPackBenchMark();
            var hDocumentBenchMark = new HDocumentBenchMark();

            var source = File.ReadAllText("Html/Page1.htm");

            for (int i = 0; i < 50; i++)
            {
                agilityPackBenchMark.Run(source);
                Console.WriteLine("AgilityPackBenchMark => Elapsed time: {0} ms", agilityPackBenchMark.TimeElapsed.TotalMilliseconds);
                GC.Collect();

                hDocumentBenchMark.Run(source);
                Console.WriteLine("HDocumentBenchMark => Elapsed time: {0} ms", hDocumentBenchMark.TimeElapsed.TotalMilliseconds);
                GC.Collect();
            }
            
            Console.WriteLine("Press any key ...");
            Console.ReadKey(true);
        }
    }
}
