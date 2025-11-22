using System.ComponentModel;
using System.Windows.Controls;

namespace RaspMat.Views
{
    /// <summary>
    /// The <see langword="base"/> <see langword="class"/> for all <see cref="UserControl"/>s.
    /// </summary>
    internal abstract class UserControlBase : UserControl
    {

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
