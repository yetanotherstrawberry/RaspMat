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
            Execute(() =>
            {
                if (Application.Current.MainWindow.GetType().Equals(typeof(TWindow))) throw new InvalidOperationException(nameof(Application.Current.MainWindow));

                if (!Windows.TryGetValue(typeof(TWindow), out var tempWindow) || !tempWindow.IsLoaded)
                {
                    Windows.Remove(typeof(TWindow));
                    Windows.Add(typeof(TWindow), tempWindow = _serviceProvider.GetRequiredService<TWindow>());
                }

                if (tempWindow.IsVisible) tempWindow.Close();
                else
                {

                    if (ModalViews.Contains(typeof(TWindow)))
                    {
                        tempWindow.Owner = Application.Current.MainWindow;
                        tempWindow.ShowDialog();
                    }
                    else tempWindow.Show();
                }
            });
        }

        /// <inheritdoc/>
        public void ToggleStepsView() => ToggleView<StepListWindow>();

        /// <inheritdoc/>
        public void ToggleNewMatDialog() => ToggleView<NewMatDialog>();

        /// <inheritdoc/>
        public void Execute(Action action) => Application.Current.Dispatcher.Invoke(action);

        /// <inheritdoc/>
        public Task ExecuteAsync<TResult>(Func<TResult> callback) => Application.Current.Dispatcher.InvokeAsync(callback).Task;

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
