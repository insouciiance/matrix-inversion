using System;
using System.Threading;
using System.Threading.Tasks;
using MatrixInversion.Library.Mathematics;

namespace MatrixInversion.Library.Algorithms;

public class GJEParallelMatrixInverser : IMatrixInverser
{
    private readonly int _maxDegreeOfParallelism;

    private readonly ParallelOptions _parallelOptions;

    public GJEParallelMatrixInverser(int maxDegreeOfParallelism)
    {
        _maxDegreeOfParallelism = maxDegreeOfParallelism;
        _parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallelism };

        ThreadPool.SetMaxThreads(_maxDegreeOfParallelism, _maxDegreeOfParallelism);
        ThreadPool.SetMinThreads(_maxDegreeOfParallelism, _maxDegreeOfParallelism);
    }

    public Matrix Invert(Matrix matrix)
    {
        if (matrix.Width != matrix.Height)
            throw new ArgumentException("Matrix must be square", nameof(matrix));

        Matrix result = Matrix.Identity(matrix.Width);

        Action[] tasks;

        for (int i = 0; i < matrix.Height; i++)
        {
            if (matrix[i, i] == 0)
            {
                TrySwapRows();

                void TrySwapRows()
                {
                    for (int ii = i + 1; ii < matrix.Height; ii++)
                    {
                        if (matrix[i, ii] != 0)
                        {
                            SwapRows(matrix[i], matrix[ii]);
                            SwapRows(matrix[i], matrix[ii]);
                            return;
                        }
                    }

                    throw new ArgumentException("Matrix must be invertible");
                }
            }

            float coef = matrix[i, i];
            NormalizeRow(matrix[i], coef);
            NormalizeRow(result[i], coef);

            tasks = new Action[matrix.Height - i - 1];

            for (int ii = i + 1; ii < matrix.Height; ii++)
            {
                float factor = matrix[ii, i];

                float[] matrixFrom = matrix[i];
                float[] matrixTo = matrix[ii];
                float[] resultFrom = result[i];
                float[] resultTo = result[ii];

                tasks[ii - i - 1] = () =>
                {
                    SubtractRow(matrixFrom, matrixTo, factor);
                    SubtractRow(resultFrom, resultTo, factor);
                };
            }

            Parallel.Invoke(_parallelOptions, tasks);
        }

        for (int i = matrix.Height - 1; i >= 0; i--)
        {
            tasks = new Action[i];

            for (int ii = i - 1; ii >= 0; ii--)
            {
                float factor = matrix[ii, i];

                float[] matrixFrom = matrix[i];
                float[] matrixTo = matrix[ii];
                float[] resultFrom = result[i];
                float[] resultTo = result[ii];

                tasks[i - 1 - ii] = () =>
                {
                    SubtractRow(matrixFrom, matrixTo, factor);
                    SubtractRow(resultFrom, resultTo, factor);
                };
            }

            Parallel.Invoke(_parallelOptions, tasks);
        }

        return result;

        void NormalizeRow(float[] row, float coef)
        {
            for (int j = 0; j < row.Length; j++)
                row[j] /= coef;
        }

        void SwapRows(float[] from, float[] to)
        {
            for (int j = 0; j < from.Length; j++)
                (from[j], to[j]) = (to[j], from[j]);
        }

        void SubtractRow(float[] from, float[] to, float factor = 1)
        {
            for (int j = 0; j < from.Length; j++)
                to[j] -= from[j] * factor;
        }
    }
}
