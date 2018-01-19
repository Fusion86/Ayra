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
        public void WritePassByReference()
        {
            WriteReference(ref largeArray);
        }

        [Benchmark]
        public void WritePassByValue()
        {
            WriteValue(largeArray);
        }

        [Benchmark]
        public void ReadPassByReference()
        {
            ReadReference(ref largeArray);
        }

        [Benchmark]
        public void ReadPassByReadonlyReference()
        {
            ReadReference(ref largeArray);
        }

        [Benchmark]
        public void ReadPassByValue()
        {
            ReadValue(largeArray);
        }

        public void WriteReference(ref byte[] data)
        {
            for (int i = 0; i < data.Length - 1; i++)
            {
                data[i] = data[i + 1]; // Do some stuff that takes time
            }
        }

        public void WriteValue(byte[] data)
        {
            for (int i = 0; i < data.Length - 1; i++)
            {
                data[i] = data[i + 1]; // Do some stuff that takes time
            }
        }

        public void ReadReference(ref byte[] data)
        {
            byte[] x = new byte[data.Length];

            for (int i = 0; i < data.Length - 1; i++)
            {
                x[i] = data[i + 1]; // Do some stuff that takes time
            }
        }

        public void ReadReference(in byte[] data)
        {
            byte[] x = new byte[data.Length];

            for (int i = 0; i < data.Length - 1; i++)
            {
                x[i] = data[i + 1]; // Do some stuff that takes time
            }
        }

        public void ReadValue(byte[] data)
        {
            byte[] x = new byte[data.Length];

            for (int i = 0; i < data.Length - 1; i++)
            {
                x[i] = data[i + 1]; // Do some stuff that takes time
            }
        }
    }
}
