using Prism.Ioc;
using Prism.Unity;
using RaspMat.Interfaces;
using RaspMat.Services;
using RaspMat.Views;
using System;
using System.Windows;

namespace RaspMat
{
    /// <summary>
    /// Main class of the <see cref="Application"/>.
    /// </summary>
    public partial class App : PrismApplication
    {

        /// <summary>
        /// Service used to handle error messages and <see cref="Exception"/>s.
        /// </summary>
        private readonly IErrorService errorService = new ErrorService();

        protected override Window CreateShell()
        {
            return Container.Resolve<GaussianWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<NewMatDialog>(name: RaspMat.Properties.Resources._NEW_MAT_DIALOG);
            containerRegistry.RegisterInstance<IFileService>(new FileService(RaspMat.Properties.Resources._FILE_FILTER));
            containerRegistry.RegisterSingleton<ISerializationService, JsonSerializationService>();
            containerRegistry.RegisterInstance(errorService);
        }

        /// <summary>
        /// Creates the main class of this <see cref="Application"/>. Binds <see cref="Application.DispatcherUnhandledException"/>.
        /// </summary>
        public App()
        {
            DispatcherUnhandledException += errorService.ExceptionHandler;
        }

        /// <summary>
        /// Unbinds <see cref="Application.DispatcherUnhandledException"/>.
        /// </summary>
        ~App()
        {
            DispatcherUnhandledException -= errorService.ExceptionHandler;
        }

    }
}
