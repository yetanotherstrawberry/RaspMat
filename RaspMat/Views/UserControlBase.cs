using System.ComponentModel;
using System.Windows.Controls;

namespace RaspMat.Views
{
    /// <summary>
    /// The <see langword="base"/> <see langword="class"/> for <see cref="UserControl"/>s.
    /// </summary>
    internal abstract class UserControlBase : UserControl
    {

        /// <summary>
        /// Sets the ViewModel.
        /// </summary>
        public UserControlBase()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                var viewModel = App.GetViewModel(GetType());
                if (viewModel != null) DataContext = viewModel;
            }
        }

    }
}
