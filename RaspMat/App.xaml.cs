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
            disUnhExcArgs.Handled = true; // Do not crash the application if possible.
            Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(((App)Current).MainWindow, disUnhExcArgs.Exception.Message, RaspMat.Properties.Resources.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        /// <summary>
        /// Gets the ViewModel for the specified <paramref name="viewType"/>.
        /// The ViewModel must be registered in the <see cref="OnStartup(StartupEventArgs)"/> method.
        /// </summary>
        /// <param name="viewType"><see cref="Type"/> of the view.</param>
        /// <returns>Instance of ViewModel casted to <see cref="object"/> or <see langword="null"/>.</returns>
        internal static object GetViewModel(Type viewType)
        {
            var app = (App)Current;
            return app.ViewModelLocator.TryGetValue(viewType, out var viewModelType) ? app.Services.GetRequiredService(viewModelType) : null;
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
            builder.AddSingleton<IFileService, Win32WPFFileService>();
            builder.AddSingleton<ISerializationService, JsonSerializationService>();
            builder.AddSingleton<IEventService, WeakReferenceMessengerEventService>();
            builder.AddSingleton<IMathService, DataTableMathService>();
            builder.AddSingleton<ICommandingService, AsyncRelayCommandingService>(serviceProvider =>
            {
                return new AsyncRelayCommandingService(Current.Dispatcher.Invoke);
            });

            builder.AddSingleton<IStepViewService, StepWPFWindowService>();
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

        protected override void OnStartup(StartupEventArgs startupEventArgs)
        {
            DispatcherUnhandledException += MsgBoxExceptionHandler;

            base.OnStartup(startupEventArgs);

            var builder = new ServiceCollection();
            AddServices(builder);
            RegisterViewModels(builder);

            _services = builder.BuildServiceProvider();
            (MainWindow = Services.GetRequiredService<MainWindow>()).Show();
        }

        protected override void OnExit(ExitEventArgs exitArgs)
        {
            base.OnExit(exitArgs);

            _services.Dispose();

            DispatcherUnhandledException -= MsgBoxExceptionHandler;
        }

    }
}
