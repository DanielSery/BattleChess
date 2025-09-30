using BenchmarkDotNet.Running;

namespace CrownsGuard.Benchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<PositionsHelperBenchmarks>();
    }
}