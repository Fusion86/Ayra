using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using System;

namespace Ayra.Benchmark.Benchmark
{
    [ClrJob, CoreJob, MonoJob]
    public class StructByRef : IBenchmark
    {
        private byte[] largeArray = new byte[4096];

        [GlobalSetup]
        public void Setup()
        {
            Random rand = new Random();
            for (int i = 0; i < largeArray.Length; i++)
            {
                largeArray[i] = (byte)rand.Next(0, 0xFF);
            }
        }

        [Benchmark]
        public void PassByReference()
        {
            GetByReference(ref largeArray);
            //Console.WriteLine(); // Make sure the optimizer doesn't do weird stuff
        }

        [Benchmark]
        public void PassByValue()
        {
            GetByValue(largeArray);
            //Console.WriteLine(); // Make sure the optimizer doesn't do weird stuff
        }

        public void GetByReference(ref byte[] data)
        {
            for (int i = 0; i < data.Length - 1; i++)
            {
                data[i] = data[i + 1]; // Do some stuff that takes time
            }
        }

        public void GetByValue(byte[] data)
        {
            for (int i = 0; i < data.Length - 1; i++)
            {
                data[i] = data[i + 1]; // Do some stuff that takes time
            }
        }
    }
}
