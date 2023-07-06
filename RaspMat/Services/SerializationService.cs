using Newtonsoft.Json;
using RaspMat.Interfaces;
using System.IO;

namespace RaspMat.Services
{
    internal class SerializationService : ISerializationService
    {

        private readonly IFileService fileService;
        private readonly JsonSerializer serializer = new JsonSerializer();

        public T Deserialize<T>() where T : class
        {
            var stream = fileService.OpenFile();
            if (stream is null) return default;

            using (stream)
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return serializer.Deserialize<T>(jsonReader);
            }
        }

        public void Serialize<T>(T obj) where T : class
        {
            var stream = fileService.NewFile();
            if (stream is null) return; // No file selected - cancel silently.

            using (stream)
            using (var writer = new StreamWriter(stream))
            {
                serializer.Serialize(writer, obj);
            }
        }

        public SerializationService(IFileService fileSrv)
        {
            fileService = fileSrv;
        }

    }
}
