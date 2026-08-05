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
    public partial class App : Application, IDisposable
    {

        /// <summary>
        /// Returns <see cref="App"/> that has been set to be the <see cref="Application.Current"/> one.
        /// </summary>
        private static App Self => Current as App;

        /// <summary>
        /// <see cref="Dispatcher.Invoke(Action)"/> used to run <see cref="Action"/>s that access the UI.
        /// </summary>
        private Action<Action> _invoker;

        /// <summary>
        /// Service provider for dependency injection.
        /// </summary>
        private IServiceProvider Services => _serviceProvider;

        /// <summary>
        /// Field for <see cref="Services"/>.
        /// </summary>
        private ServiceProvider _serviceProvider;

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
            var exception = disUnhExcArgs.Exception;
            while (exception.InnerException != null) exception = exception.InnerException;

            void ShowMessage()
            {
                MessageBox.Show(Self?.MainWindow, exception.Message, RaspMat.Properties.Resources.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (_invoker is null) ShowMessage();
            else _invoker(ShowMessage);

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
            return Self?.ViewModelLocator.TryGetValue(viewType, out var viewModelType) ?? false ? Self.Services.GetRequiredService(viewModelType) : null;
        }

        /// <summary>
        /// Creates the <typeparamref name="TService"/> or <see langword="return"/>s an existing one.
        /// Will <see langword="return"/> <see langword="null"/> if the <typeparamref name="TService"/> has not been registered or <see cref="App"/> has not been initialized.
        /// </summary>
        /// <typeparam name="TService">The requested service.</typeparam>
        /// <returns>An instance of <typeparamref name="TService"/> or <see langword="null"/>.</returns>
        internal static TService GetService<TService>() where TService : class
        {
            return Self?.Services.GetService<TService>();
        }

        /// <summary>
        /// Registers <typeparamref name="TView"/> and <typeparamref name="TViewModel"/> as transient services.
        /// Adds mapping to <see cref="ViewModelLocator"/>.
        /// </summary>
        /// <typeparam name="TView">View to register and assign the ViewModel to.</typeparam>
        /// <typeparam name="TViewModel">ViewModel to register and assign to the view.</typeparam>
        /// <param name="collection">Service builder to register transient <typeparamref name="TView"/> and <typeparamref name="TViewModel"/> to.</param>
        /// <param name="singleton">Indicates whether the view should be registered as a singleton.</param>
        private void RegisterViewModel<TView, TViewModel>(IServiceCollection collection, bool singleton = false) where TViewModel : class where TView : class
        {
            if (singleton)
            {
                collection.AddSingleton<TView>();
                collection.AddSingleton<TViewModel>();
            }
            else
            {
                collection.AddTransient<TView>();
                collection.AddTransient<TViewModel>();
            }
            ViewModelLocator.Add(typeof(TView), typeof(TViewModel));
        }

        /// <summary>
        /// Registers services.
        /// </summary>
        /// <param name="builder"><see cref="IServiceCollection"/> to register the services to.</param>
        private void AddServices(IServiceCollection builder)
        {
            builder.AddSingleton<IViewService, WpfViewService>(serviceProvider =>
            {
                return new WpfViewService(serviceProvider, Dispatcher.CurrentDispatcher);
            });

            builder.AddTransient<ICommandingService, AsyncRelayCommandingService>();
            builder.AddSingleton<IFileService, Win32WPFFileService>();
            builder.AddSingleton<ISerializationService, JsonSerializationService>();
            builder.AddSingleton<IEventService, WeakReferenceMessengerEventService>();
            builder.AddSingleton<IMathService, DataTableMathService>();
        }

        /// <summary>
        /// Registers ViewModels.
        /// </summary>
        /// <param name="builder"><see cref="IServiceCollection"/> to register the services to.</param>
        private void RegisterViewModels(IServiceCollection builder)
        {
            RegisterViewModel<MainWindow, MainWindowViewModel>(builder, true);
            RegisterViewModel<StepListWindow, StepListWindowViewModel>(builder);

            RegisterViewModel<NewMatrixDialog, NewMatrixDialogViewModel>(builder);
            RegisterViewModel<InputMatrixDialog, InputMatrixDialogViewModel>(builder);
            RegisterViewModel<NewVectorsDialog, NewVectorsDialogViewModel>(builder);

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

            _serviceProvider = builder.BuildServiceProvider();

            _invoker = Services.GetRequiredService<IViewService>().Execute;
            Services.GetRequiredService<IViewService>().ToggleMainWindow();
        }

        /// <inheritdoc/>
        protected override void OnExit(ExitEventArgs exitArgs)
        {
            base.OnExit(exitArgs);
            Dispose();
        }

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            _serviceProvider.Dispose();
            DispatcherUnhandledException -= MsgBoxExceptionHandler;
        }

    }
}
