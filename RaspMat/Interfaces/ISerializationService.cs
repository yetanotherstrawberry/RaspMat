namespace RaspMat.Interfaces
{
    internal interface ISerializationService
    {

        void Serialize<T>(T obj) where T : class;
        T Deserialize<T>() where T : class;

    }
}
