using Newtonsoft.Json;
using RaspMat.Interfaces;
using RaspMat.Models;
using System.IO;

namespace RaspMat.Services
{
    /// <summary>
    /// JSON serialization service.
    /// </summary>
    internal class JsonSerializationService : ISerializationService
    {

        /// <summary>
        /// Instance that will return <see cref="Stream"/>s to (de)serialize from/to.
        /// </summary>
        private readonly IFileService fileService;

        /// <summary>
        /// Instance of a serializer.
        /// </summary>
        private readonly JsonSerializer serializer;

        public IDeserializationResult<TDeserialized> Deserialize<TDeserialized>()
        {
            var stream = fileService.OpenFile();
            if (stream is null) return new DeserializationResult<TDeserialized>(); // No file selected by the user.

            using (stream)
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return new DeserializationResult<TDeserialized>(successful: true, serializer.Deserialize<TDeserialized>(jsonReader));
            }
        }

        public void Serialize<TDeserialized>(TDeserialized serialized)
        {
            var stream = fileService.NewFile();
            if (stream is null) return; // No file selected - cancel silently.

            using (stream)
            using (var writer = new StreamWriter(stream))
            {
                serializer.Serialize(writer, serialized);
            }
        }

        /// <summary>
        /// Creates a JSON serialization service.
        /// </summary>
        /// <param name="fileSrv"><see cref="IFileService"/> that will return <see cref="Stream"/>s to (de)serialize from/to.</param>
        public JsonSerializationService(IFileService fileSrv)
        {
            fileService = fileSrv;
            serializer = new JsonSerializer();
        }

    }
}
