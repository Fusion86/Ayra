using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Running;

namespace Ayra.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Assembly thisAsm = Assembly.GetExecutingAssembly();

            List<AssemblyName> assemblies = new List<AssemblyName>(new[] { thisAsm.GetName() });
            var referencedAssemblies = thisAsm.GetReferencedAssemblies().Where(x => x.Name.StartsWith("System", StringComparison.InvariantCulture) == false);
            assemblies.AddRange(referencedAssemblies);

            foreach (AssemblyName asm in assemblies)
                Console.WriteLine($"{asm.Name} {asm.Version}");

            // Gather all benchmarks in a list
            List<Benchmark> benchmarks = new List<Benchmark>();
            foreach (Type t in thisAsm.GetTypes().Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(Benchmark))))
                benchmarks.Add((Benchmark)Activator.CreateInstance(t));

            foreach (Benchmark b in benchmarks)
                RunBenchmark(b);
        }

        static void RunBenchmark(Benchmark b)
        {
            if (b.HasRun) return;

            foreach (Benchmark sub in b.DependsOn)
                if (!sub.HasRun) RunBenchmark(sub);
            
           BenchmarkRunner.Run(b.GetType());
        }
    }
}
