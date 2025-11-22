using Microsoft.Extensions.DependencyInjection;
using RaspMat.Services;
using RaspMat.Services.Interfaces;
using RaspMat.ViewModels;
using RaspMat.Views;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace RaspMat
{
    /// <summary>
    /// Main class of this <see cref="Application"/>.
    /// </summary>
    public partial class App : Application
    {

        /// <summary>
        /// Returns <see cref="App"/> that has been set to be the <see cref="Application.Current"/> one.
        /// </summary>
        private static App Self => Current as App;

        /// <summary>
        /// <see cref="Dispatcher.Invoke(Action)"/> used to run <see cref="Action"/>s on the UI thread.
        /// </summary>
        private Action<Action> _invoker;

        /// <summary>
        /// Service provider for dependency injection.
        /// </summary>
        private IServiceProvider Services => _services;

        /// <summary>
        /// Field for <see cref="Services"/>.
        /// </summary>
        private ServiceProvider _services;

        /// <summary>
        /// Returns <see cref="Type"/> of the ViewModel based on the <see cref="Type"/> of the <see cref="KeyValuePair{TKey, TValue}.Key"/> of the view.
        /// </summary>
        private IDictionary<Type, Type> ViewModelLocator { get; } = new Dictionary<Type, Type>();

        /// <summary>
        /// Displays the <see cref="Exception"/> and sets <see cref="DispatcherUnhandledExceptionEventArgs.Handled"/> to <see langword="true"/>.
        /// </summary>
        /// <param name="sender">An <see cref="object"/> that threw the <see cref="Exception"/>.</param>
        /// <param name="disUnhExcArgs">An instance which will have its <see cref="DispatcherUnhandledExceptionEventArgs.Handled"/> set by this method.</param>
        private void MsgBoxExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs disUnhExcArgs)
        {
            _invoker(() =>
            {
                MessageBox.Show(Self?.MainWindow, disUnhExcArgs.Exception.Message, RaspMat.Properties.Resources.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            });
            disUnhExcArgs.Handled = true; // Do not crash if possible.
        }

        /// <summary>
        /// Gets the ViewModel for the specified <paramref name="viewType"/>.
        /// The ViewModel must be registered in the <see cref="OnStartup(StartupEventArgs)"/> method.
        /// </summary>
        /// <param name="viewType"><see cref="Type"/> of the view.</param>
        /// <returns>Instance of ViewModel casted to <see cref="object"/> or <see langword="null"/>.</returns>
        internal static object GetViewModel(Type viewType)
        {
            return Self.ViewModelLocator.TryGetValue(viewType, out var viewModelType) ? Self.Services.GetRequiredService(viewModelType) : null;
        }

        /// <summary>
        /// Registers <typeparamref name="TView"/> and <typeparamref name="TViewModel"/> as transient services.
        /// Adds mapping to <see cref="ViewModelLocator"/>.
        /// </summary>
        /// <typeparam name="TView">View to register and assign the ViewModel to.</typeparam>
        /// <typeparam name="TViewModel">ViewModel to register and assign to the view.</typeparam>
        /// <param name="collection">Service builder to register transient <typeparamref name="TView"/> and <typeparamref name="TViewModel"/> to.</param>
        private void RegisterViewModel<TView, TViewModel>(IServiceCollection collection) where TViewModel : class where TView : class
        {
            collection.AddTransient<TView>();
            collection.AddTransient<TViewModel>();
            ViewModelLocator.Add(typeof(TView), typeof(TViewModel));
        }

        /// <summary>
        /// Registers services.
        /// </summary>
        /// <param name="builder"><see cref="IServiceCollection"/> to register the services to.</param>
        private void AddServices(IServiceCollection builder)
        {
            builder.AddTransient<ICommandingService, AsyncRelayCommandingService>(serviceProvider =>
            {
                return new AsyncRelayCommandingService(_invoker);
            });

            builder.AddSingleton<IFileService, Win32WPFFileService>();
            builder.AddSingleton<ISerializationService, JsonSerializationService>();
            builder.AddSingleton<IEventService, WeakReferenceMessengerEventService>();
            builder.AddSingleton<IMathService, DataTableMathService>();
            builder.AddSingleton<IViewService, WpfViewService>();
        }

        /// <summary>
        /// Registers ViewModels.
        /// </summary>
        /// <param name="builder"><see cref="IServiceCollection"/> to register the services to.</param>
        private void RegisterViewModels(IServiceCollection builder)
        {
            RegisterViewModel<MainWindow, MainWindowViewModel>(builder);
            RegisterViewModel<StepListWindow, StepListWindowViewModel>(builder);
            RegisterViewModel<NewMatDialog, NewMatDialogViewModel>(builder);

            RegisterViewModel<FractionUserControl, FractionUserControlViewModel>(builder);
            RegisterViewModel<GaussianUserControl, GaussianUserControlViewModel>(builder);
            RegisterViewModel<GraphUserControl, GraphUserControlViewModel>(builder);
        }

        /// <inheritdoc/>
        protected override void OnStartup(StartupEventArgs startupEventArgs)
        {
            DispatcherUnhandledException += MsgBoxExceptionHandler;

            base.OnStartup(startupEventArgs);

            var builder = new ServiceCollection();
            AddServices(builder);
            RegisterViewModels(builder);

            _invoker = action =>
            {
                if (action is null) return;
                Self.Dispatcher.Invoke(action);
            };

            _services = builder.BuildServiceProvider();

            MainWindow = Services.GetRequiredService<MainWindow>();
            MainWindow.Show();
        }

        /// <inheritdoc/>
        protected override void OnExit(ExitEventArgs exitArgs)
        {
            base.OnExit(exitArgs);

            _services.Dispose();

            DispatcherUnhandledException -= MsgBoxExceptionHandler;
        }

    }
}
