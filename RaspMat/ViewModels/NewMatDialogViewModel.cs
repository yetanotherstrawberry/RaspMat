using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RaspMat.Properties;
using System;
using System.Windows.Input;

namespace RaspMat.ViewModels
{
    internal class NewMatDialogViewModel : BindableBase, IDialogAware
    {

        public ICommand CloseDialogCommand { get; }

        public event Action<IDialogResult> RequestClose;

        public string Rows { get; set; }

        public string Columns { get; set; }

        public bool AddZeros { get; set; }

        public string Title => Resources.ENTER_MATRIX;

        protected void CloseDialog()
        {
            var ret = new DialogResult(ButtonResult.OK);

            ret.Parameters.Add(Resources._ROWS, int.Parse(Rows));
            ret.Parameters.Add(Resources._COLS, int.Parse(Columns));
            ret.Parameters.Add(Resources._ADD_ZEROS, AddZeros);

            RequestClose?.Invoke(ret);
        }

        public NewMatDialogViewModel()
        {
            CloseDialogCommand = new DelegateCommand(CloseDialog);
        }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        public void OnDialogOpened(IDialogParameters parameters) { }

    }
}
