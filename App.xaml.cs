using Prism.Ioc;
using Prism.Unity;
using RaspMat.Interfaces;
using RaspMat.Services;
using RaspMat.Views;
using System.Windows;

namespace RaspMat
{
    public partial class App : PrismApplication
    {

        private readonly IErrorService errorService = new ErrorService();

        protected override Window CreateShell()
        {
            return Container.Resolve<GaussianWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<NewMatDialog>(name: RaspMat.Properties.Resources._NEW_MAT_DIALOG);
            containerRegistry.RegisterInstance<IFileService>(new FileService(RaspMat.Properties.Resources._FILE_FILTER));
            containerRegistry.RegisterSingleton<ISerializationService, SerializationService>();
        }

        public App()
        {
            DispatcherUnhandledException += errorService.ExceptionHandler;
        }

        ~App()
        {
            DispatcherUnhandledException -= errorService.ExceptionHandler;
        }

    }
}
