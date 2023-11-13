using System.Diagnostics;

namespace Tata;

public static class Program
{
    public static void Main()
    {
        const int iterations = 100_000;

        for (int p = 1; p <= Environment.ProcessorCount; p++)
        {
            Console.WriteLine($"Paralellism: {p}");
            long totalTimeNs = 0;
            ExecuteConcurrently(() =>
            {
                Stopwatch sw = Stopwatch.StartNew();
                for (int i = 0; i < iterations; i++)
                {
                    try
                    {
                        throw new OverflowException();
                    }
                    catch(OverflowException) {}
                }
                sw.Stop();
                Interlocked.Add(ref totalTimeNs, (long)sw.Elapsed.TotalNanoseconds);
            }, p);
            Console.WriteLine($"Time per exception thrown: {1d * totalTimeNs / iterations / p} ns");
        }
    }

    private static void ExecuteConcurrently(Action action, int parallelism)
    {
        var threads = new Thread[parallelism];
        for (int i = 0; i < parallelism; i++)
        {
            threads[i] = new Thread(_ => action());
            threads[i].Start();
        }

        for (int i = 0; i < parallelism; i++)
        {
            threads[i].Join();
        }
    }
}