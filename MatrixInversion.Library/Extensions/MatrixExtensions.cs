using System.Text;
using MatrixInversion.Library.Logging;
using MatrixInversion.Library.Mathematics;

namespace MatrixInversion.Library.Extensions;

public static class MatrixExtensions
{
    public static void Dump(this Matrix matrix)
    {
        StringBuilder builder = new();

        for (int i = 0; i < matrix.Height; i++)
        {
            for (int j = 0; j < matrix.Width; j++)
                builder.Append($"{double.Round(matrix[i, j], 3), -7} ");

            builder.AppendLine();
        }

        Log.Default.Info(builder.ToString());
    }
}
