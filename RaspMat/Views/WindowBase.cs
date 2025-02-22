using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace RaspMat.Views
{
    internal abstract class WindowBase : Window
    {

        public WindowBase()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                var viewModel = App.GetViewModel(GetType());
                if (viewModel != null) DataContext = viewModel;
            }
        }

        protected override void OnKeyDown(KeyEventArgs keyArgs)
        {
            base.OnKeyDown(keyArgs);

            switch (keyArgs.Key)
            {
                case Key.Escape:
                    Close();
                    break;
            }
        }

    }
}
