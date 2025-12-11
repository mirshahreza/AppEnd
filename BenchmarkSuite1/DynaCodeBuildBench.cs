using System;
using BenchmarkDotNet.Attributes;
using AppEndDynaCode;

namespace AppEndBenchmarks
{
    [MemoryDiagnoser]
    [ThreadingDiagnoser]
    public class DynaCodeBuildBench
    {
        [GlobalSetup]
        public void Setup()
        {
            // Initialize with defaults; avoid AppEndApi dependency
            AppEndDynaCode.DynaCode.Init();
        }

        [Benchmark]
        public void Rebuild()
        {
            AppEndDynaCode.DynaCode.ReBuild();
        }
    }
}