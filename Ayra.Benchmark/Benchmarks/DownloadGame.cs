using BenchmarkDotNet.Attributes;
using System;

namespace Ayra.Benchmark.Benchmarks
{
    public class DownloadGame : Benchmark
    {
        public override string Name => "Download game";

        [Benchmark]
        public void Run()
        {
            
        }
    }
}
