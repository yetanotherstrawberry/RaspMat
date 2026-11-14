using System.Windows;

namespace RaspMat.Views
{
    /// <summary>
    /// Interaction logic for <see cref="BasisChangeDialog"/>.
    /// </summary>
    internal partial class BasisChangeDialog : DialogWindowBase
    {

        /// <summary>
        /// Initializes XAML.
        /// </summary>
        public BasisChangeDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the click event of the previous button.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that requested the operation.</param>
        /// <param name="eventArgs">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void PrevButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (!eventArgs.Handled)
            {
                ProgressIndicator.Value--;
                eventArgs.Handled = true;
            }
        }

        /// <summary>
        /// Handles the click event of the next button.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that requested the operation.</param>
        /// <param name="eventArgs">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void NextButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (!eventArgs.Handled)
            {
                ProgressIndicator.Value++;
                if (ProgressIndicator.Value == ProgressIndicator.Maximum)
                {
                    Close();
                }
                eventArgs.Handled = true;
            }
        }

    }
}
