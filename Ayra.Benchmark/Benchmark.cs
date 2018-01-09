using BenchmarkDotNet.Attributes;

namespace Ayra.Benchmark
{
    public abstract class Benchmark
    {
        public abstract string Name { get; }
        public bool HasRun;
        public virtual Benchmark[] DependsOn => new Benchmark[0];
    }
}
