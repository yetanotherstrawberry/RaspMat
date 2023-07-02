using RaspMat.Interfaces;
using System;
using System.Windows;
using System.Windows.Threading;

namespace RaspMat.Services
{
    internal class ErrorService : IErrorService
    {

        public void Error(string message)
        {
            MessageBox.Show(message, Properties.Resources.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void Error(Exception exception) => Error(exception.Message);

        public void ExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            Error(args.Exception);
            args.Handled = true;
        }

    }
}
