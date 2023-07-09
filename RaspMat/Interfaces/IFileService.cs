using System.IO;

namespace RaspMat.Interfaces
{
    /// <summary>
    /// A service for handling file I/O.
    /// </summary>
    internal interface IFileService
    {

        /// <summary>
        /// Returns an unopened read-only <see cref="Stream"/> to the selected file.
        /// </summary>
        /// <returns>A read-only <see cref="Stream"/> to a file or <see langword="null"/> if no file selected.</returns>
        Stream OpenFile();

        /// <summary>
        /// Return an unopened write-only <see cref="Stream"/> to the selected file.
        /// </summary>
        /// <returns>A write-only <see cref="Stream"/> to a file or <see langword="null"/> if no file selected.</returns>
        Stream NewFile();

    }
}
