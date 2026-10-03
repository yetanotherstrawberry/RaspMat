using RaspMat.Extensions;
using System.Windows;
using System.Windows.Input;

namespace RaspMat.Views
{
    /// <summary>
    /// The <see langword="base"/> <see langword="class"/> for <see cref="Window"/>s.
    /// </summary>
    internal abstract class WindowBase : Window
    {

        /// <summary>
        /// Sets the ViewModel.
        /// </summary>
        public WindowBase()
        {
            if (this.IsNotInDesign())
            {
                var viewModel = App.GetViewModel(GetType());
                if (viewModel != null) DataContext = viewModel;
            }
        }

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs keyArgs)
        {
            base.OnKeyDown(keyArgs);

            if (!keyArgs.Handled)
            {
                switch (keyArgs.Key)
                {
                    case Key.Escape:
                        Close();
                        break;
                    default:
                        return;
                }
                keyArgs.Handled = true;
            }
        }

    }
}
