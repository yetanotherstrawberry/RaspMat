using Microsoft.Extensions.DependencyInjection;
using RaspMat.Services.Interfaces;
using RaspMat.Views;
using System;
using System.Collections.Generic;
using System.Windows;

namespace RaspMat.Services
{
    internal class WPFWindowService : IViewService
    {

        private IDictionary<Type, Window> Windows { get; } = new Dictionary<Type, Window>();

        private readonly IServiceProvider _serviceProvider;
        private readonly Action<Action> _dispatcherInvoke;

        private void SpawnWindow<TWindow>() where TWindow : Window
        {
            _dispatcherInvoke(() =>
            {
                if (!Windows.TryGetValue(typeof(TWindow), out var _tempWindow) || !_tempWindow.IsLoaded)
                {
                    Windows.Remove(typeof(TWindow));
                    Windows.Add(typeof(TWindow), _tempWindow = _serviceProvider.GetRequiredService<TWindow>());
                    _tempWindow.Owner = Application.Current.MainWindow;
                }

                if (_tempWindow.IsVisible)
                    _tempWindow.Close();
                else
                    _tempWindow.Show();
            });
        }

        public void ToggleStepWindow() => SpawnWindow<StepListWindow>();

        public void ToggleNewMatDialog() => SpawnWindow<NewMatDialog>();

        public void Execute(Action action) => _dispatcherInvoke(action);

        public WPFWindowService(IServiceProvider serviceProvider, Action<Action> dispatcherInvoke)
        {
            _serviceProvider = serviceProvider;
            _dispatcherInvoke = dispatcherInvoke;
        }

    }
}
