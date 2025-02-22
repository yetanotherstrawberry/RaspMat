using System;
using System.Windows.Input;

namespace RaspMat.Services.Interfaces
{
    /// <summary>
    /// A service for creation of <see cref="ICommand"/>s.
    /// </summary>
    internal interface ICommandingService
    {

        ICommand CreateFromAction<TParameter>(Action before = null, Action<TParameter> action = null, Action finished = null, Predicate<TParameter> canExecute = null);

    }
}
