using System;

namespace ScrapySharp.Benchmarks
{
    public interface IBenchMark
    {
        void Run(string source);

        TimeSpan TimeElapsed { get; }
    }
}