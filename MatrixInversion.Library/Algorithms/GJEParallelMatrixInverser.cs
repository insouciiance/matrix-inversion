using System;
using System.Collections.Generic;
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

        List<Action> tasks;

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
                            SwapRows(result[i], result[ii]);
                            return;
                        }
                    }

                    throw new ArgumentException("Matrix must be invertible");
                }
            }

            float coef = matrix[i, i];
            NormalizeRow(matrix[i], coef);
            NormalizeRow(result[i], coef);

            int currentRow = i;
            int offset = i + 1;

            int rawRows = (matrix.Height - offset) / _maxDegreeOfParallelism;
            int extra = (matrix.Height - offset) % _maxDegreeOfParallelism;

            tasks = new();

            for (int ii = 0; ii < _maxDegreeOfParallelism; ii++)
            {
                int rows = ii < extra ? rawRows + 1 : rawRows;

                float[][] slice = matrix[offset..(offset + rows)];
                float[][] resultSlice = result[offset..(offset + rows)];

                float[] matrixFrom = matrix[i];
                float[] resultFrom = result[i];

                tasks.Add(() =>
                {
                    for (int j = 0; j < rows; j++)
                    {
                        float factor = slice[j][currentRow];

                        float[] matrixTo = slice[j];
                        float[] resultTo = resultSlice[j];

                        SubtractRow(matrixFrom, matrixTo, factor);
                        SubtractRow(resultFrom, resultTo, factor);
                    }
                });

                offset += rows;
            }

            Parallel.Invoke(_parallelOptions, tasks.ToArray());
        }

        for (int i = matrix.Height - 1; i >= 0; i--)
        {
            tasks = new();

            int currentRow = i;
            int offset = 0;

            int rawRows = i / _maxDegreeOfParallelism;
            int extra = i % _maxDegreeOfParallelism;

            for (int ii = 0; ii < _maxDegreeOfParallelism; ii++)
            {
                int rows = ii < extra ? rawRows + 1 : rawRows;

                float[][] slice = matrix[offset..(offset + rows)];
                float[][] resultSlice = result[offset..(offset + rows)];

                float[] matrixFrom = matrix[i];
                float[] resultFrom = result[i];

                tasks.Add(() =>
                {
                    for (int j = 0; j < rows; j++)
                    {
                        float factor = slice[j][currentRow];

                        float[] matrixTo = slice[j];
                        float[] resultTo = resultSlice[j];

                        SubtractRow(matrixFrom, matrixTo, factor);
                        SubtractRow(resultFrom, resultTo, factor);
                    }
                });

                offset += rows;
            }

            Parallel.Invoke(_parallelOptions, tasks.ToArray());
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
