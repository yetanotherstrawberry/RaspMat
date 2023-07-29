using Microsoft.Win32;
using RaspMat.Interfaces;
using System.IO;

namespace RaspMat.Services
{
    /// <summary>
    /// Service implementing I/O.
    /// </summary>
    internal class FileService : IFileService
    {

        /// <summary>
        /// Filter for the file selection. Used for <see cref="FileDialog.Filter"/>.
        /// </summary>
        private readonly string filter;

        public Stream OpenFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = filter,
            };

            if (dialog.ShowDialog() ?? false)
            {
                return dialog.OpenFile();
            }

            return null;
        }

        public Stream NewFile()
        {
            var dialog = new SaveFileDialog
            {
                Filter = filter,
            };

            if (dialog.ShowDialog() ?? false)
            {
                return dialog.OpenFile();
            }

            return null;
        }

        /// <summary>
        /// Creates a service for handling file I/O. Prompts the user with <see cref="FileDialog"/>s for file selection.
        /// </summary>
        /// <param name="fileFilter">Filter for file selection. Use <see cref="string.Empty"/> to allow all extensions.</param>
        public FileService(string fileFilter = "")
        {
            filter = fileFilter;
        }

    }
}
