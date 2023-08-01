using Prism.Ioc;
using Prism.Unity;
using RaspMat.Interfaces;
using RaspMat.Services;
using RaspMat.Views;
using System.Windows;
using System.Windows.Threading;

namespace RaspMat
{
    /// <summary>
    /// Main class of the <see cref="Application"/>.
    /// </summary>
    public partial class App : PrismApplication
    {

        private void ExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            MessageBox.Show(args.Exception.Message, RaspMat.Properties.Resources.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        }

        protected override Window CreateShell() => Container.Resolve<MainWindow>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<NewMatDialog>(RaspMat.Properties.Resources._NEW_MAT_DIALOG);
            containerRegistry.RegisterSingleton<IFileService, Win32FileService>();
            containerRegistry.RegisterSingleton<ISerializationService, JsonSerializationService>();
        }

        /// <summary>
        /// Creates the main class of this <see cref="Application"/>. Binds <see cref="Application.DispatcherUnhandledException"/>.
        /// </summary>
        public App()
        {
            DispatcherUnhandledException += ExceptionHandler;
        }

        /// <summary>
        /// Unbinds <see cref="Application.DispatcherUnhandledException"/>.
        /// </summary>
        ~App()
        {
            DispatcherUnhandledException -= ExceptionHandler;
        }

    }
}
