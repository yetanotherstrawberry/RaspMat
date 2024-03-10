using Prism.Mvvm;
using RaspMat.Services.Interfaces;
using RaspMat.ViewModels;
using RaspMat.Views;
using System.Windows;

namespace RaspMat.Services
{
    internal class StepWPFWindowService : IStepViewService
    {

        private Window _stepViewWindow;

        public void Toggle()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!_stepViewWindow?.IsLoaded ?? true)
                {
                    _stepViewWindow = new StepListWindow()
                    {
                        Owner = Application.Current.MainWindow,
                    };
                    ViewModelLocator.SetAutoWireViewModel(_stepViewWindow, true);
                }

                if (_stepViewWindow.IsVisible)
                    _stepViewWindow.Close();
                else
                    _stepViewWindow.Show();
            });
        }

        public StepWPFWindowService()
        {
            ViewModelLocationProvider.Register<StepListWindow, StepListWindowViewModel>();
        }

    }
}
