using Newtonsoft.Json;
using RaspMat.Services.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace RaspMat.Services
{
    /// <summary>
    /// JSON serialization service.
    /// </summary>
    internal class JsonSerializationService : ISerializationService
    {

        private readonly IFileService _fileService;
        private readonly JsonSerializer serializer = new JsonSerializer();

        public async Task<TDeserialized> Deserialize<TDeserialized>() where TDeserialized : class // Constraint forces value to be nullable.
        {
            var stream = await _fileService.OpenFileAsync();
            if (stream is null) return null; // No file selected by the user.

            using (stream)
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return serializer.Deserialize<TDeserialized>(jsonReader);
            }
        }

        public async Task Serialize<TDeserialized>(TDeserialized serialized) where TDeserialized : class
        {
            var stream = await _fileService.NewFileAsync();
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
