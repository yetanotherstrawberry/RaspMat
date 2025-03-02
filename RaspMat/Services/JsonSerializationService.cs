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
        private readonly JsonSerializer _serializer = JsonSerializer.CreateDefault();

        public async Task<TDeserialized> Deserialize<TDeserialized>() where TDeserialized : class
        {
            var stream = await _fileService.OpenFileAsync().ConfigureAwait(false);
            if (stream is null) return null; // No file selected by the user.

            using (stream)
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return await Task.Run(() => _serializer.Deserialize<TDeserialized>(jsonReader)).ConfigureAwait(false);
            }
        }

        public async Task Serialize<TDeserialized>(TDeserialized serialized) where TDeserialized : class
        {
            var stream = await _fileService.NewFileAsync().ConfigureAwait(false);
            if (stream is null) return; // No file selected - cancel silently.

            using (stream)
            using (var writer = new StreamWriter(stream))
            {
                await Task.Run(() => _serializer.Serialize(writer, serialized)).ConfigureAwait(false);
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
