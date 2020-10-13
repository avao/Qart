using System;
using BenchmarkDotNet.Running;
using Qart.Testing.Framework;

namespace BenchmarkingQart
{
    class Program
    {
        static void Main(string[] args)
        {
//            Utf8ReaderFromFile.Run(@"C:\Projects\content.json");
            BenchmarkRunner.Run<UrlBasedParameterExtraction_Benchmark>();
        }
    }
}
