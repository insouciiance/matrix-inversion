using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using MatrixInversion.Library.Extensions;
using MatrixInversion.Library.Logging;
using MatrixInversion.Library.Mathematics;

namespace MatrixInversion.Benchmarks;

public class Benchmark
{
    private readonly BenchmarkOptions _options;

    public Benchmark(BenchmarkOptions options)
    {
        _options = options;
    }

    public BenchmarkResult Run()
    {
        Log.Default.Info("Starting benchmark");

        Log.Default.Info("Starting warmup");

        Matrix result = _options.Inverser.Invert(Matrix.Clone(_options.Matrix));

        Log.Default.Info("Warmup complete");

        List<float> results = new();

        Stopwatch sw = new();

        for (int i = -5; i < _options.Runs; i++)
        {
            Log.Default.Info($"Starting run {i + 1}");

            Matrix copy = Matrix.Clone(_options.Matrix);

            sw.Start();
            Matrix next = _options.Inverser.Invert(copy);
            sw.Stop();

            MatrixHelper.AssertEqual(result, next);

            Log.Default.Info($"Run {i + 1} done. Elapsed: {sw.ElapsedMilliseconds}ms");

            if (i >= 0)
                results.Add(sw.ElapsedMilliseconds);
        
            sw.Reset();
        }

        Log.Default.Info("Benchmark done");

        return BenchmarkResult.FromRawResults(result, results);
    }
}
