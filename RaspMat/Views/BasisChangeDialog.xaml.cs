using RaspMat.Services.Interfaces;
using System;
using System.Windows;

namespace RaspMat.Views
{
    /// <summary>
    /// Interaction logic for <see cref="BasisChangeDialog"/>.
    /// </summary>
    internal partial class BasisChangeDialog : DialogWindowBase
    {

        /// <summary>
        /// Used to access the view.
        /// </summary>
        private readonly IViewService _viewService = App.GetService<IViewService>();

        /// <summary>
        /// Field for <see cref="CurrentPage"/>.
        /// </summary>
        private ushort _currentPage = ushort.MaxValue;

        /// <summary>
        /// Indicates the current page of the dialog.
        /// </summary>
        private ushort CurrentPage
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
            using (_viewService.PauseRender())
            {
                const ushort TOTAL_PAGES = 3;
                BackButton.IsEnabled = NextButton.IsEnabled = false;
                NextButton.Visibility = Visibility.Visible;
                FinishButton.Visibility = DestinationMatrixContainer.Visibility = SourceMatrixContainer.Visibility = DimensionsContainer.Visibility =
                    Visibility.Collapsed;
                switch (CurrentPage)
                {
                    case 0:
                        DimensionsContainer.Visibility = Visibility.Visible;
                        NextButton.IsEnabled = true;
                        break;
                    case 1:
                        SourceMatrixContainer.Visibility = Visibility.Visible;
                        BackButton.IsEnabled = true;
                        NextButton.IsEnabled = true;
                        break;
                    case 2:
                        DestinationMatrixContainer.Visibility = Visibility.Visible;
                        BackButton.IsEnabled = true;
                        NextButton.Visibility = Visibility.Collapsed;
                        FinishButton.Visibility = Visibility.Visible;
                        break;
                    case 3:
                        ProgressIndicator.Value = 1;
                        Close();
                        return;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(CurrentPage));
                }
                ProgressIndicator.Value = CurrentPage / (double)TOTAL_PAGES;
            }
        }

        /// <summary>
        /// Initializes XAML.
        /// </summary>
        public BasisChangeDialog()
        {
            InitializeComponent();
            CurrentPage = 0;
        }

        /// <summary>
        /// Handles the click event of the previous button.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that requested the operation.</param>
        /// <param name="eventArgs">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void PrevButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            CurrentPage--;
        }

        /// <summary>
        /// Handles the click event of the next button.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that requested the operation.</param>
        /// <param name="eventArgs">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void NextButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            CurrentPage++;
        }

    }
}
