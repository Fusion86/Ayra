using BenchmarkDotNet.Attributes;
using System;
using System.Threading;

namespace Ayra.Benchmark.Benchmarks
{
    public class DownloadTMD : Benchmark
    {
        public override string Name => "Download TMD";

        [Benchmark]
        public void Run()
        {
            Thread.Sleep(2000);
        }
    }
}
