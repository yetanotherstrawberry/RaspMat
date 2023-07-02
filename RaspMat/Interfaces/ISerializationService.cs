namespace RaspMat.Interfaces
{
    internal interface ISerializationService
    {

        void Serialize<T>(T obj);
        T Deserialize<T>();

    }
}
