using BenchmarkDotNet.Attributes;
using Qart.Testing.Framework;

namespace BenchmarkingQart
{
    [MemoryDiagnoser]
    public class UrlBasedParameterExtraction_Benchmark
    {
        private string testingValue = "weather?q=London,uk&appid=2b1fd2d7f77ccf1b7de9b441571b39b8&test=aaa";

        [Benchmark(Baseline = true)]
        public void RunLegacy()
        {
            UrlBasedParameterExtraction.Parse(testingValue);
        }

        [Benchmark]
        public void RunNew()
        {
            UrlBasedParameterExtraction.Parse2(testingValue);
        }
    }
}
