using System;
using System.Windows.Input;

namespace RaspMat.Interfaces
{
    internal interface IAsyncCommandService
    {

        ICommand CreateAsyncCommand(Action action);

    }
}
