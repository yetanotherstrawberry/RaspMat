using Prism.Ioc;
using Prism.Unity;
using RaspMat.Services;
using RaspMat.Services.Interfaces;
using RaspMat.Views;
using System;
using System.Windows;
using System.Windows.Threading;

namespace RaspMat
{
    /// <summary>
    /// Main class of this <see cref="Application"/>.
    /// </summary>
    public partial class App : PrismApplication
    {

        /// <summary>
        /// Displays the <see cref="Exception"/> and sets <see cref="DispatcherUnhandledExceptionEventArgs.Handled"/> to <see langword="true"/>.
        /// </summary>
        /// <param name="sender">An <see cref="object"/> that threw the <see cref="Exception"/>.</param>
        /// <param name="disUnhExcArgs">An instance which will have its <see cref="DispatcherUnhandledExceptionEventArgs.Handled"/> set by this method.</param>
        private void WinExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs disUnhExcArgs)
        {
            disUnhExcArgs.Handled = true; // Do not crash the application if possible.
            Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(disUnhExcArgs.Exception.Message, RaspMat.Properties.Resources.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        protected override Window CreateShell() => Container.Resolve<MainWindow>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IFileService, Win32WPFFileService>();
            containerRegistry.RegisterSingleton<ISerializationService, JsonSerializationService>();
            containerRegistry.RegisterSingleton<IStepViewService, StepWPFWindowService>();

            containerRegistry.RegisterDialog<NewMatDialog>(RaspMat.Properties.Resources._NEW_MAT_DIALOG);
        }

        protected override void OnExit(ExitEventArgs exitArgs)
        {
            base.OnExit(exitArgs);
            DispatcherUnhandledException -= WinExceptionHandler;
        }

        protected override void OnStartup(StartupEventArgs startupArgs)
        {
            DispatcherUnhandledException += WinExceptionHandler;
            base.OnStartup(startupArgs);
        }

    }
}
