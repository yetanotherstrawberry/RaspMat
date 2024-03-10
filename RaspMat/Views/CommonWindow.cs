using System.Windows;
using System.Windows.Input;

namespace RaspMat.Views
{
    public class CommonWindow : Window
    {

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
