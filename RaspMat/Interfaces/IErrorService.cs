using System;
using System.Windows.Threading;

namespace RaspMat.Interfaces
{
    internal interface IErrorService
    {

        void Error(Exception exception);
        void Error(string message);
        void ExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs args);

    }
}
