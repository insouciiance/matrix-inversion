using System;

namespace MatrixInversion.Library.Mathematics;

public static class MatrixHelper
{
    private static readonly Random Random = new(42);

    public static Matrix GenerateSquareMatrix(int size, int min, int max)
    {
        float[][] matrix = new float[size][];

        for (int i = 0; i < size; i++)
        {
            matrix[i] = new float[size];

            for (int j = 0; j < size; j++)
                matrix[i][j] = Random.Next(min, max);
        }

        return new(matrix);
    }

    public static Matrix GenerateIdentityMatrix(int size)
    {
        float[][] matrix = new float[size][];

        for (int i = 0; i < size; i++)
        {
            matrix[i] = new float[size];
            matrix[i][i] = 1;
        }

        return new(matrix);
    }

    public static void AssertEqual(Matrix lhs, Matrix rhs)
    {
        if (lhs.Width != rhs.Width)
            throw new ArgumentException("Matrices had not equal width");

        if (lhs.Height != rhs.Height)
            throw new ArgumentException("Matrices had not equal height");

        for (int i = 0; i < lhs.Height; i++)
        {
            for (int j = 0; j < rhs.Width; j++)
            {
                if (Math.Abs(lhs[i, j] - rhs[i, j]) > 0.001)
                    throw new ArgumentException($"Matrix elements not equal on index [{i}, {j}]: {lhs[i, j]} vs {rhs[i, j]}");
            }
        }
    }
}
