using Prism.Commands;
using RaspMat.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RaspMat.Services
{
    internal class AsyncCommandService : IAsyncCommandService
    {

        private readonly Action started, finished;
        private readonly Action<Exception> failed;

        public AsyncCommandService(Action commandStarted = null, Action commandFinished = null, Action<Exception> commandError = null)
        {
            started = commandStarted;
            finished = commandFinished;
            failed = commandError;
        }

        private async void TryExecAsync(Action action)
        {
            try
            {
                started?.Invoke();
                if (action is null) return;
                await Task.Run(action);
            }
            catch (Exception exception)
            {
                failed?.Invoke(exception);
            }
            finally
            {
                finished?.Invoke();
            }
        }

        /// <summary>
        /// Generates an implementation of <see cref="ICommand"/> for an <see cref="Action"/> without parameters.
        /// Ignores <see cref="ICommand.Execute(object)"/>'s and <see cref="ICommand.CanExecute(object)"/>'s parameter.
        /// </summary>
        /// <param name="action">A parameterless <see cref="Action"/> to execute on call to <see cref="ICommand.Execute(object)"/>.</param>
        /// <returns>An <see cref="ICommand"/> which executes <paramref name="action"/> on call.</returns>
        public ICommand GenerateAsyncActionCommand(Action action, Func<bool> canExecute = null)
        {
            return new DelegateCommand(() => TryExecAsync(action), canExecute ?? (() => true));
        }

        public ICommand GenerateAsyncActionCommand<TParameter>(Action<TParameter> action, Func<TParameter, bool> canExecute = null)
        {
            return new DelegateCommand<TParameter>(input => TryExecAsync(() => action(input)), canExecute ?? (parameter => true));
        }

    }
}
