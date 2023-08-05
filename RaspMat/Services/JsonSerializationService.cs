using Newtonsoft.Json;
using RaspMat.DTOs;
using RaspMat.Interfaces;
using System.IO;

namespace RaspMat.Services
{
    /// <summary>
    /// JSON serialization service.
    /// </summary>
    internal class JsonSerializationService : ISerializationService
    {

        private readonly IFileService _fileService;
        private readonly JsonSerializer serializer = new JsonSerializer();

        public DeserializationResultDTO<TDeserialized> Deserialize<TDeserialized>()
        {
            var stream = _fileService.OpenFile();
            if (stream is null) return new DeserializationResultDTO<TDeserialized>(); // No file selected by the user.

            using (stream)
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return new DeserializationResultDTO<TDeserialized>(successful: true, serializer.Deserialize<TDeserialized>(jsonReader));
            }
        }

        public void Serialize<TDeserialized>(TDeserialized serialized)
        {
            var stream = _fileService.NewFile();
            if (stream is null) return; // No file selected - cancel silently.

            using (stream)
            using (var writer = new StreamWriter(stream))
            {
                serializer.Serialize(writer, serialized);
            }
        }

        /// <summary>
        /// Creates a new <see cref="JsonSerializationService"/>.
        /// </summary>
        /// <param name="fileService">A service used for streaming the (de)serialized data.</param>
        public JsonSerializationService(IFileService fileService)
        {
            _fileService = fileService;
        }

    }
}
