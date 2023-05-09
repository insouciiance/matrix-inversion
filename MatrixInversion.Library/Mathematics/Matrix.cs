using System;

namespace MatrixInversion.Library.Mathematics;

public partial class Matrix
{
    private readonly float[][] _matrix;

    public int Width => _matrix[0].Length;

    public int Height => _matrix.Length;

    public Matrix(float[][] matrix)
    {
        _matrix = matrix;
    }

    public float[] this[int i]
    {
        get => _matrix[i];
    }

    public float[][] this[Range range]
    {
        get => _matrix[range.Start..range.End];
    }

    public float this[int i, int j]
    {
        get => _matrix[i][j];
        set => _matrix[i][j] = value;
    }
}
