using BenchmarkDotNet.Running;

namespace EFCore.IncludeBuilder.Benchmarks;

class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run(typeof(Program).Assembly);
    }
}
