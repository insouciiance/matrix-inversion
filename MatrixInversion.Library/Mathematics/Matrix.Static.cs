namespace MatrixInversion.Library.Mathematics;

public partial class Matrix
{
    public static Matrix Identity(int size)
    {
        float[][] matrix = new float[size][];

        for (int i = 0; i < size; i++)
        {
            matrix[i] = new float[size];
            matrix[i][i] = 1;
        }

        return new Matrix(matrix);
    }

    public static Matrix Clone(Matrix m)
    {
        float[][] copy = new float[m.Height][];

        for (int i = 0; i < m.Height; i++)
        {
            copy[i] = new float[m.Width];

            for (int j = 0; j < m.Width; j++)
                copy[i][j] = m[i][j];
        }
        
        return new(copy);
    }
}
