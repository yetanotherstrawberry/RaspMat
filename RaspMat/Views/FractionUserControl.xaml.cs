using System.Numerics;
using System.Windows.Input;

namespace RaspMat.Views
{
    /// <summary>
    /// Interaction logic for <see cref="FractionUserControl"/>.
    /// </summary>
    internal partial class FractionUserControl : UserControlBase
    {

        /// <summary>
        /// Initializes XAML.
        /// </summary>
        public FractionUserControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Forces integer input.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that triggered this handler.</param>
        /// <param name="textCompositionArgs">An instance of <see cref="TextCompositionEventArgs"/> to check <see cref="TextCompositionEventArgs.Text"/> and set <see cref="RoutedEventArgs.Handled"/>.</param>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs textCompositionArgs)
        {
            if (!BigInteger.TryParse(textCompositionArgs.Text, out var _)) textCompositionArgs.Handled = true;
        }

    }
}
