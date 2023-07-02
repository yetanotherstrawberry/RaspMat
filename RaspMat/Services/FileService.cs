using Microsoft.Win32;
using RaspMat.Interfaces;
using System.IO;

namespace RaspMat.Services
{
    internal class FileService : IFileService
    {

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

        public FileService(string fileFilter)
        {
            filter = fileFilter;
        }

    }
}
