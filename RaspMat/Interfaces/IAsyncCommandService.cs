using System;
using System.Windows.Input;

namespace RaspMat.Interfaces
{
    internal interface IAsyncCommandService
    {

        ICommand GenerateAsyncActionCommand(Action action, Func<bool> canExecute = null);
        ICommand GenerateAsyncActionCommand<TParameter>(Action<TParameter> action, Func<TParameter, bool> canExecute = null);

    }
}
