using RaspMat.Interfaces;
using System.Text.Json;

namespace RaspMat.Services
{
    internal class SerializationService : ISerializationService
    {

        private readonly IFileService fileService;

        public T Deserialize<T>()
        {
            using (var stream = fileService.OpenFile())
            {
                if (stream == null) return default;

                return JsonSerializer.Deserialize<T>(stream);
            }
        }

        public void Serialize<T>(T obj)
        {
            using (var stream = fileService.NewFile())
            {
                if (stream == null) return;

                JsonSerializer.Serialize(stream, obj);
            }
        }

        public SerializationService(IFileService fileSrv)
        {
            fileService = fileSrv;
        }

    }
}
