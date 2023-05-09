using System;
using System.Collections.Generic;
using System.Linq;
using MatrixInversion.Library.Mathematics;

namespace MatrixInversion.Benchmarks;

public readonly record struct BenchmarkResult
{
    public required Matrix Matrix { get; init; }

    public required IReadOnlyList<float> Results { get; init; }

    public float Average => Results.Average();

    public float StdDev => GetStdDev();

    public static BenchmarkResult FromRawResults(Matrix matrix, IReadOnlyList<float> results) => new()
    {
        Matrix = matrix,
        Results = results
    };

    private float GetStdDev()
    {
        float average = Average;
        return (float)Math.Sqrt(Results.Select(r => (float)Math.Pow(r - average, 2)).Sum());
    }
}
