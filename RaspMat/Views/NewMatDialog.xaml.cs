using System.Windows;
using System.Windows.Input;

namespace RaspMat.Views
{
    /// <summary>
    /// Interaction logic for NewMatDialog.xaml
    /// </summary>
    internal partial class NewMatDialog : WindowBase
    {

        /// <summary>
        /// Initializes XAML.
        /// </summary>
        public NewMatDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Forces positive integer input.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that triggered the handler.</param>
        /// <param name="textCompositionArgs">An instance of <see cref="TextCompositionEventArgs"/> to check <see cref="TextCompositionEventArgs.Text"/> and set <see cref="RoutedEventArgs.Handled"/>.</param>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs textCompositionArgs)
        {
            if (!int.TryParse(textCompositionArgs.Text, out var number) || number <= 0) textCompositionArgs.Handled = true;
        }

    }
}
