using MatrixInversion.Library.Mathematics;

namespace MatrixInversion.Library.Algorithms;

public interface IMatrixInverser
{
    Matrix Invert(Matrix matrix);
}
