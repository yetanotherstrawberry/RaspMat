using Microsoft.Extensions.DependencyInjection;
using RaspMat.Services.Interfaces;
using RaspMat.Views;
using System;
using System.Windows;

namespace RaspMat.Services
{
    internal class StepWPFWindowService : IStepViewService
    {

        private Window _stepViewWindow;
        private readonly IServiceProvider _serviceProvider;

        public void Toggle()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!_stepViewWindow?.IsLoaded ?? true)
                {
                    _stepViewWindow = _serviceProvider.GetRequiredService<StepListWindow>();
                    _stepViewWindow.Owner = Application.Current.MainWindow;
                }

                if (_stepViewWindow.IsVisible)
                    _stepViewWindow.Close();
                else
                    _stepViewWindow.Show();
            });
        }

        public StepWPFWindowService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

    }
}
