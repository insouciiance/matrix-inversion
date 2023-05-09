using System;
using MatrixInversion.Library.Algorithms;
using MatrixInversion.Library.Mathematics;

namespace MatrixInversion.Benchmarks;

public readonly record struct BenchmarkOptions(IMatrixInverser Inverser, Action<Matrix>? AfterRun, Matrix Matrix, int Runs);
