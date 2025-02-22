using System.Windows.Input;

namespace RaspMat.ViewModels
{
    internal class NewMatDialogViewModel : ViewModelBase
    {

        public ICommand CloseDialogCommand { get; }

        public string Rows { get; set; }

        public string Columns { get; set; }

        public bool AddZeros { get; set; }

    }
}
