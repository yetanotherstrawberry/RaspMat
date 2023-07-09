using System;
using System.Windows.Threading;

namespace RaspMat.Interfaces
{
    internal interface IErrorService
    {

        /// <summary>
        /// Retrieves the <see cref="Exception.Message"/> of the <paramref name="exception"/> and shows it to the user.
        /// </summary>
        /// <param name="exception">An <see cref="Exception"/> which <see cref="Exception.Message"/> will be shown to the user.</param>
        void Error(Exception exception);

        /// <summary>
        /// Shows an error message to the user.
        /// </summary>
        /// <param name="message">Message to be shown to the user.</param>
        void Error(string message);

        /// <summary>
        /// Handles the <see cref="DispatcherUnhandledExceptionEventArgs.Exception"/> of <paramref name="args"/>.
        /// </summary>
        /// <param name="sender">An <see cref="object"/> that generated the <see cref="DispatcherUnhandledExceptionEventArgs.Exception"/> of <paramref name="args"/>.</param>
        /// <param name="args">Argrument to set <see cref="DispatcherUnhandledExceptionEventArgs.Handled"/> to <see langword="true"/> when <see cref="Exception"/> is handled.</param>
        void ExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs args);

    }
}
