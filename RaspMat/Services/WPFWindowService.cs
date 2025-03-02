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

        private readonly IServiceProvider _serviceProvider;
        private readonly Action<Action> _dispatcherInvoke;

        /// <summary>
        /// <see cref="Window"/>s created by <see langword="this"/> instance.
        /// </summary>
        private IDictionary<Type, Window> Windows { get; } = new Dictionary<Type, Window>();

        /// <summary>
        /// <see cref="Window"/>s that are to be created as blocking dialogs.
        /// </summary>
        private ISet<Type> ModalViews { get; } = new HashSet<Type>()
        {
            typeof(NewMatDialog),
        };

        /// <summary>
        /// Closes or opens a <see cref="Window"/>.
        /// </summary>
        /// <typeparam name="TWindow">The <see cref="Window"/> to toggle.</typeparam>
        private void ToggleView<TWindow>() where TWindow : Window
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
                {
                    _tempWindow.Close();
                }
                else
                {
                    if (ModalViews.Contains(typeof(TWindow))) _tempWindow.ShowDialog();
                    else _tempWindow.Show();
                }
            });
        }

        public void ToggleStepsView() => ToggleView<StepListWindow>();

        public void ToggleNewMatDialog() => ToggleView<NewMatDialog>();

        public void Execute(Action action) => _dispatcherInvoke(action);

        public WPFWindowService(IServiceProvider serviceProvider, Action<Action> dispatcherInvoke)
        {
            _serviceProvider = serviceProvider;
            _dispatcherInvoke = dispatcherInvoke;
        }

    }
}
