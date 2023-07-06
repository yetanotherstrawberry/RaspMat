namespace RaspMat.Interfaces
{
    internal interface IMatrix<Number>
    {

        Number this[int row, int column] { get; }

    }
}
