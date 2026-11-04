using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RaspMat.Views
{
    /// <summary>
    /// Interaction logic for NewMatDialog.xaml
    /// </summary>
    internal partial class NewMatrixDialog : DialogWindowBase
    {

        /// <summary>
        /// Initializes XAML.
        /// </summary>
        public NewMatrixDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Filters the input.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that triggered the handler.</param>
        /// <param name="textCompositionArgs">An instance of <see cref="TextCompositionEventArgs"/> to check <see cref="TextCompositionEventArgs.Text"/> and set <see cref="RoutedEventArgs.Handled"/>.</param>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs textCompositionArgs)
        {
            if (!textCompositionArgs.Text.All(stringChar => char.IsDigit(stringChar) || stringChar == '-' || stringChar == '/')) textCompositionArgs.Handled = true;
        }

    }
}
