using Microsoft.Win32;
using RaspMat.Interfaces;
using RaspMat.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace RaspMat.Services
{
    /// <summary>
    /// Service implementing I/O using <see cref="FileDialog"/>s.
    /// </summary>
    internal class Win32FileService : IFileService
    {

        private readonly IDictionary<Type, FileDialog> _dialogs = new Dictionary<Type, FileDialog>();

        private TDialog CreateDialog<TDialog>() where TDialog : FileDialog, new()
        {
            // Dialogs must be created and shown from the main (UI) thread.
            return Application.Current.Dispatcher.Invoke(() =>
            {
                TDialog dialog;
                if (!_dialogs.ContainsKey(typeof(TDialog)))
                {
                    dialog = new TDialog
                    {
                        Filter = Resources._FILE_FILTER,
                    };
                    _dialogs.Add(typeof(TDialog), dialog);
                }
                else
                {
                    dialog = (TDialog)_dialogs[typeof(TDialog)];
                }

                if (dialog.ShowDialog() ?? false)
                {
                    return dialog;
                }
                return null;
            });
        }

        public Stream OpenFile()
        {
            return CreateDialog<OpenFileDialog>()?.OpenFile();
        }

        public Stream NewFile()
        {
            return CreateDialog<SaveFileDialog>()?.OpenFile();
        }

    }
}
