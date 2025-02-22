using System.ComponentModel;
using System.Windows.Controls;

namespace RaspMat.Views
{
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
