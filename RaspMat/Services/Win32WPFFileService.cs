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

        /// <summary>
        /// Stores created <see cref="CommonDialog"/>s based on their <see cref="Type"/>.
        /// </summary>
        private readonly IDictionary<Type, CommonDialog> _dialogs = new Dictionary<Type, CommonDialog>();

        /// <summary>
        /// Used for UI access.
        /// </summary>
        private readonly IViewService _viewService;

        /// <summary>
        /// Uses a <typeparamref name="TDialog"/> from <see cref="_dialogs"/> or creates a <see langword="new"/> one and stores it.
        /// Shows the <typeparamref name="TDialog"/> to the user.
        /// If the user selected a file a <typeparamref name="TDialog"/> is returned; <see langword="null"/> otherwise.
        /// </summary>
        /// <typeparam name="TDialog"><see cref="Type"/> of the dialog. Must be instantiable and inherit <see cref="FileDialog"/>.</typeparam>
        /// <returns>A <typeparamref name="TDialog"/> or <see langword="null"/>.</returns>
        private async Task<TDialog> CreateDialog<TDialog>() where TDialog : FileDialog, new()
        {
            // Dialogs must be created and shown from the main (UI) thread.
            return await _viewService.ExecuteAsync(() =>
            {
                if (!_dialogs.TryGetValue(typeof(TDialog), out var dialog))
                {
                    _dialogs.Add(typeof(TDialog), dialog = new TDialog()
                    {
                        Filter = Resources._FILE_FILTER,
                    });
                }

                if (dialog.ShowDialog(Application.Current.MainWindow) ?? false)
                {
                    return (TDialog)dialog;
                }

                return null;
            });
        }

        /// <inheritdoc/>
        public async Task<Stream> OpenFileAsync()
        {
            var dialog = await CreateDialog<OpenFileDialog>().ConfigureAwait(continueOnCapturedContext: false);
            return dialog?.OpenFile();
        }

        /// <inheritdoc/>
        public async Task<Stream> NewFileAsync()
        {
            var dialog = await CreateDialog<SaveFileDialog>().ConfigureAwait(continueOnCapturedContext: false);
            return dialog?.OpenFile();
        }

        public Win32WPFFileService(IViewService viewService)
        {
            _viewService = viewService;
        }

    }
}
