using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RaspMat.Interfaces
{
    internal interface IAsyncCommandService
    {

        /// <summary>
        /// Generates an <see cref="ICommand"/> from an <see cref="Action"/>.
        /// </summary>
        /// <param name="action"><see cref="Action"/> to be executed on <see cref="ICommand.Execute(object)"/>.</param>
        /// <param name="canExecute"><see cref="Func{bool}"/> to return when <see cref="ICommand.CanExecute(object)"/> is executed.</param>
        /// <returns>An <see cref="ICommand"/> that uses provided functions for its implementation.</returns>
        ICommand GenerateAsyncActionCommand(Action action, Func<bool> canExecute = null);

        /// <summary>
        /// Generates an <see cref="ICommand"/> from an <see cref="Action{T}"/>.
        /// </summary>
        /// <typeparam name="TParameter"><see cref="Type"/> of the parameter accepted by <paramref name="action"/>.</typeparam>
        /// <param name="action"><see cref="Action{T}"/> to be executed on <see cref="ICommand.Execute(object)"/>.</param>
        /// <param name="canExecute"><see cref="Func{TParameter, bool}"/> to return when <see cref="ICommand.CanExecute(object)"/> is executed.</param>
        /// <returns>An <see cref="ICommand"/> that uses provided functions for its implementation.</returns>
        ICommand GenerateAsyncActionCommand<TParameter>(Action<TParameter> action, Func<TParameter, bool> canExecute = null);

        /// <summary>
        /// Tries to execute an <paramref name="action"/>.
        /// </summary>
        /// <param name="action"><see cref="Action"/> to be executed.</param>
        void TryExecAsync(Action action);

        /// <summary>
        /// Tries to execute a <paramref name="task"/>.
        /// </summary>
        /// <param name="task"><see cref="Task"/> to be executed.</param>
        void TryExecAsync(Task task);

    }
}
