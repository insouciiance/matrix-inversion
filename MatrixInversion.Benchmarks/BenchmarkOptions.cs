using MatrixInversion.Library.Algorithms;
using MatrixInversion.Library.Mathematics;

namespace MatrixInversion.Benchmarks;

public readonly record struct BenchmarkOptions(IMatrixInverser Inverser, Matrix Matrix, int Runs);
