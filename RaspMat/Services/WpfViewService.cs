using Microsoft.Extensions.DependencyInjection;
using RaspMat.Extensions;
using RaspMat.Services.Interfaces;
using RaspMat.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace RaspMat.Services
{
    /// <summary>
    /// Uses <see cref="Window"/>s as views.
    /// </summary>
    internal class WpfViewService : IViewService
    {

        /// <summary>
        /// The <see cref="IServiceProvider"/> used to create <see cref="Window"/>s.
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// <see cref="Window"/>s created by <see langword="this"/> instance.
        /// </summary>
        private IDictionary<Type, Window> Windows { get; } = new Dictionary<Type, Window>();

        /// <summary>
        /// Closes or opens a <see cref="Window"/>.
        /// </summary>
        /// <typeparam name="TWindow">The <see cref="Window"/> to toggle.</typeparam>
        private void ToggleView<TWindow>() where TWindow : Window
        {
            Execute(() =>
            {
                if (!Windows.TryGetValue(typeof(TWindow), out var tempWindow) || !tempWindow.IsLoaded)
                {
                    Windows.Remove(typeof(TWindow));
                    Windows.Add(typeof(TWindow), tempWindow = _serviceProvider.GetRequiredService<TWindow>());
                }

                if (tempWindow.IsVisible) tempWindow.Close();
                else
                {
                    if (tempWindow is DialogWindowBase)
                    {
                        tempWindow.Owner = Application.Current.MainWindow;
                        tempWindow.ShowDialog();
                    }
                    else tempWindow.Show();
                }
            });
        }

        /// <inheritdoc/>
        public void ToggleMainWindow() => ToggleView<MainWindow>();

        /// <inheritdoc/>
        public void ToggleStepsView() => ToggleView<StepListWindow>();

        /// <inheritdoc/>
        public void ToggleNewMatrixDialog() => ToggleView<NewMatrixDialog>();

        /// <inheritdoc/>
        public void ToggleMatrixInputDialog() => ToggleView<InputMatrixDialog>();

        /// <inheritdoc/>
        public void Execute(Action action) => Application.Current.Dispatcher.Invoke(action);

        /// <inheritdoc/>
        public Task<TResult> ExecuteAsync<TResult>(Func<TResult> callback) => Application.Current.Dispatcher.InvokeAsync(callback).Task;

        /// <summary>
        /// Creates a new instance of <see cref="IViewService"/> that uses the provided <see cref="IServiceProvider"/> to create <see cref="Window"/>s.
        /// </summary>
        /// <param name="serviceProvider"><see cref="IServiceProvider"/> used to create <see cref="Window"/>s.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceProvider"/> <see langword="is"/> <see langword="null"/>.</exception>
        public WpfViewService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider.ThrowIfNull();
        }

    }
}
