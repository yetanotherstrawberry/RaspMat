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
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Application.Current.MainWindow.GetType().Equals(typeof(TWindow))) throw new InvalidOperationException();

                if (!Windows.TryGetValue(typeof(TWindow), out var _tempWindow) || !_tempWindow.IsLoaded)
                {
                    Windows.Remove(typeof(TWindow));
                    Windows.Add(typeof(TWindow), _tempWindow = _serviceProvider.GetRequiredService<TWindow>());
                }

                if (_tempWindow.IsVisible)
                {
                    _tempWindow.Close();
                }
                else
                {
                    if (ModalViews.Contains(typeof(TWindow)))
                    {
                        _tempWindow.Owner = Application.Current.MainWindow;
                        _tempWindow.ShowDialog();
                    }
                    else _tempWindow.Show();
                }
            });
        }

        public void ToggleStepsView() => ToggleView<StepListWindow>();

        public void ToggleNewMatDialog() => ToggleView<NewMatDialog>();

        public void Execute(Action action) => Application.Current.Dispatcher.Invoke(action);

        public WPFWindowService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

    }
}
