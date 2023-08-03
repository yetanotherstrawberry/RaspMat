using Prism.Mvvm;
using RaspMat.Interfaces;
using RaspMat.ViewModels;
using RaspMat.Views;
using System.Windows;

namespace RaspMat.Services
{
    internal class StepWindowService : IStepViewService
    {

        private Window _stepViewWindow;

        public void Toggle()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!_stepViewWindow?.IsLoaded ?? true)
                {
                    _stepViewWindow = new StepListWindow();
                }

                ViewModelLocator.SetAutoWireViewModel(_stepViewWindow, true);

                if (_stepViewWindow.IsVisible)
                    _stepViewWindow.Close();
                else
                    _stepViewWindow.Show();
            });
        }

        public StepWindowService()
        {
            ViewModelLocationProvider.Register<StepListWindow, StepListWindowViewModel>();
        }

    }
}
