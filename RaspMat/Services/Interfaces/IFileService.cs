using System.IO;
using System.Threading.Tasks;

namespace RaspMat.Services.Interfaces
{
    /// <summary>
    /// A service for handling file I/O.
    /// </summary>
    internal interface IFileService
    {

        /// <summary>
        /// Returns an unopened read-only <see cref="Stream"/> to the selected file.
        /// </summary>
        /// <returns>An awaitable <see cref="Task"/> containing a read-only <see cref="Stream"/> to a file or <see langword="null"/> if no file selected.</returns>
        Task<Stream> OpenFileAsync();

        /// <summary>
        /// Return an unopened write-only <see cref="Stream"/> to the selected file.
        /// </summary>
        /// <returns>An awaitable <see cref="Task"/> containing a write-only <see cref="Stream"/> to a file or <see langword="null"/> if no file selected.</returns>
        Task<Stream> NewFileAsync();

    }
}
