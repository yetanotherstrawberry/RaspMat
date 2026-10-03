using System;
using System.Windows;

namespace RaspMat.Views
{
    /// <summary>
    /// Interaction logic for <see cref="BasisChangeDialog"/>.
    /// </summary>
    internal partial class BasisChangeDialog : WindowBase
    {

        /// <summary>
        /// Indicates whether the dialog has been shown.
        /// </summary>
        private bool Shown { get; set; } = false;

        /// <summary>
        /// Field for <see cref="CurrentPage"/>.
        /// </summary>
        private int _currentPage = -1;

        /// <summary>
        /// Indicates the current page of the dialog.
        /// </summary>
        private int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                UpdateContent();
            }
        }

        /// <summary>
        /// Updates the content of the dialog based on the current page.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The <see cref="CurrentPage"/> is out of range.</exception>
        private void UpdateContent()
        {
            const int TOTAL_PAGES = 3;
            DimensionsContainer.Visibility = Visibility.Hidden;
            BackButton.Visibility = NextButton.Visibility = Visibility.Visible;
            switch (CurrentPage)
            {
                case 0:
                    DimensionsContainer.Visibility = Visibility.Visible;
                    BackButton.Visibility = Visibility.Hidden;
                    break;
                case 1:
                    
                    break;
                case 2:

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(CurrentPage));
            }
            ProgressIndicator.Value = CurrentPage / 3.0;
        }

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

        }

        /// <summary>
        /// Handles the click event of the next button.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that requested the operation.</param>
        /// <param name="eventArgs">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void NextButton_Click(object sender, RoutedEventArgs eventArgs)
        {

        }

        /// <inheritdoc/>
        protected override void OnContentRendered(EventArgs eventArgs)
        {
            base.OnContentRendered(eventArgs);
            if (!Shown)
            {
                Shown = true;
                CurrentPage = 0;
            }
        }

    }
}
