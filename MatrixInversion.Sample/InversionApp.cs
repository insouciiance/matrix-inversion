using System;
using System.Linq;
using CommandLine;
using MatrixInversion.Benchmarks;
using MatrixInversion.Library.Algorithms;
using MatrixInversion.Library.Extensions;
using MatrixInversion.Library.Logging;
using MatrixInversion.Library.Mathematics;

namespace MatrixInversion.Sample;

public class InversionApp
{
    private Options _options = null!;

    public InversionApp(string[] args)
    {
        Log.Default.Info($"Running app with args {string.Join(' ', args)}");

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(o => _options = o)
            .WithNotParsed(e => throw new InvalidOperationException($"Error parsing args: {string.Join(", ", e.Select(r => r.Tag.ToString()))}"));
    }

    public void Run()
    {
        if (_options.Size <= 0)
            throw new ArgumentException("Invalid matrix size set", nameof(Options.Size));

        if (_options.Min > _options.Max)
            throw new ArgumentException("Min value for matrix generation is more than the max value");

        if (_options.RunsCount <= 0)
            throw new ArgumentException("Benchmark should run less than 1 time, really?", nameof(Options.RunsCount));

        Matrix matrix = _options.GenerationStrategy switch
        {
            "random" => MatrixHelper.GenerateSquareMatrix(_options.Size, _options.Min, _options.Max),
            "identity" => MatrixHelper.GenerateIdentityMatrix(_options.Size),
            _ => throw new ArgumentOutOfRangeException(nameof(Options.GenerationStrategy))
        };

        if (_options.DumpMatrix)
            DumpMatrix(matrix);

        IMatrixInverser inverser = _options.Inverser switch
        {
            "gje" => new GJEMatrixInverser(),
            "gje-parallel" => new GJEParallelMatrixInverser(_options.ProcessorsCount),
            _ => throw new ArgumentOutOfRangeException(nameof(Options.Inverser))
        };

        Matrix? syncResult = new GJEMatrixInverser().Invert(Matrix.Clone(matrix));

        BenchmarkOptions options = new()
        {
            Runs = _options.RunsCount,
            Matrix = matrix,
            Inverser = inverser,
            AfterRun = m => MatrixHelper.AssertEqual(syncResult, m)
        };

        BenchmarkResult result = new Benchmark(options).Run();
        DumpResult(result);
    }

    private void DumpResult(in BenchmarkResult result)
    {
        Log.Default.Info("\n--- BENCHMARK RESULTS ---");
        Log.Default.Info($"Average: {result.Average}");
        Log.Default.Info($"StdDev: {result.StdDev}");

        if (_options.DumpResult)
            DumpMatrix(result.Matrix);
    }

    private static void DumpMatrix(Matrix matrix)
    {
        for (int i = 0; i < matrix.Height; i++)
        {
            for (int j = 0; j < matrix.Width; j++)
                Log.Default.Write($"{double.Round(matrix[j, i], 4),-8}", LogSeverity.Info);

            Log.Default.Info("");
        }
    }
}
