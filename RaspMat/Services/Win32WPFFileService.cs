using Microsoft.Win32;
using RaspMat.Properties;
using RaspMat.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace RaspMat.Services
{
    /// <summary>
    /// Service implementing I/O using <see cref="FileDialog"/>s.
    /// </summary>
    internal class Win32WPFFileService : IFileService
    {

        private readonly IDictionary<Type, FileDialog> _dialogs = new Dictionary<Type, FileDialog>();

        private async Task<TDialog> CreateDialog<TDialog>() where TDialog : FileDialog, new()
        {
            // Dialogs must be created and shown from the main (UI) thread.
            return await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                TDialog dialog;
                if (!_dialogs.ContainsKey(typeof(TDialog)))
                {
                    dialog = new TDialog()
                    {
                        Filter = Resources._FILE_FILTER,
                    };
                    _dialogs.Add(typeof(TDialog), dialog);
                }
                else
                {
                    dialog = (TDialog)_dialogs[typeof(TDialog)];
                }

                if (dialog.ShowDialog(Application.Current.MainWindow) ?? false)
                {
                    return dialog;
                }

                return null;
            });
        }

        public async Task<Stream> OpenFileAsync()
        {
            var dialog = await CreateDialog<OpenFileDialog>().ConfigureAwait(false);
            return dialog?.OpenFile();
        }

        public async Task<Stream> NewFileAsync()
        {
            var dialog = await CreateDialog<SaveFileDialog>().ConfigureAwait(false);
            return dialog?.OpenFile();
        }

    }
}
