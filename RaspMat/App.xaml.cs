using Prism.Ioc;
using Prism.Unity;
using RaspMat.Interfaces;
using RaspMat.Services;
using RaspMat.Views;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace RaspMat
{
    /// <summary>
    /// Main class of the <see cref="Application"/>.
    /// </summary>
    public partial class App : PrismApplication
    {

        /// <summary>
        /// Displays the <see cref="Exception"/> and sets <see cref="DispatcherUnhandledExceptionEventArgs.Handled"/> to <see langword="true"/>.
        /// </summary>
        /// <param name="sender">An <see cref="object"/> that threw the <see cref="Exception"/>.</param>
        /// <param name="args">An instance which will have its <see cref="DispatcherUnhandledExceptionEventArgs.Handled"/> set by this method.</param>
        private void ExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            MessageBox.Show(args.Exception.Message, RaspMat.Properties.Resources.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true; // Do not crash the application if possible.
        }

        protected override Window CreateShell() => Container.Resolve<MainWindow>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IFileService, Win32FileService>();
            containerRegistry.RegisterSingleton<ISerializationService, JsonSerializationService>();
            containerRegistry.RegisterSingleton<IStepViewService, StepWindowService>();

            containerRegistry.RegisterDialog<NewMatDialog>(RaspMat.Properties.Resources._NEW_MAT_DIALOG);
        }

        /// <summary>
        /// Creates the main class of this <see cref="Application"/>, binds <see cref="Application.DispatcherUnhandledException"/> and sets <see cref="Thread.CurrentThread.CurrentUICulture"/>.
        /// </summary>
        public App() : base()
        {
            DispatcherUnhandledException += ExceptionHandler;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        ~App()
        {
            DispatcherUnhandledException -= ExceptionHandler;
        }

    }
}
