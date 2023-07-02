namespace RaspMat.Interfaces
{
    internal interface IAlgorithmResult<T>
    {

        string Step { get; }
        T Result { get; }

    }
}
