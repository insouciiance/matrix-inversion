using System;
using MatrixInversion.Library.Mathematics;

namespace MatrixInversion.Library.Algorithms;

public class GJEMatrixInverser : IMatrixInverser
{
    public Matrix Invert(Matrix matrix)
    {
        if (matrix.Width != matrix.Height)
            throw new ArgumentException("Matrix must be square", nameof(matrix));

        Matrix result = Matrix.Identity(matrix.Width);

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

            for (int ii = i + 1; ii < matrix.Height; ii++)
            {
                float factor = matrix[ii, i];

                float[] matrixFrom = matrix[i];
                float[] matrixTo = matrix[ii];
                float[] resultFrom = result[i];
                float[] resultTo = result[ii];

                SubtractRow(matrixFrom, matrixTo, factor);
                SubtractRow(resultFrom, resultTo, factor);
            }
        }

        for (int i = matrix.Height - 1; i >= 0; i--)
        {
            for (int ii = i - 1; ii >= 0; ii--)
            {
                float factor = matrix[ii, i];

                float[] matrixFrom = matrix[i];
                float[] matrixTo = matrix[ii];
                float[] resultFrom = result[i];
                float[] resultTo = result[ii];

                SubtractRow(matrixFrom, matrixTo, factor);
                SubtractRow(resultFrom, resultTo, factor);
            }
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
