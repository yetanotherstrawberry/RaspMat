using Prism.Commands;
using RaspMat.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RaspMat.Services
{
    /// <summary>
    /// A service to execute 
    /// </summary>
    internal class AsyncCommandService : IAsyncCommandService
    {

        /// <summary>
        /// An <see cref="Action"/> to start when <see cref="ICommand.Execute(object)"/> <see cref="started"/> and <see cref="finished"/>.
        /// </summary>
        private readonly Action started, finished;

        /// <summary>
        /// An <see cref="Action{T}"/> where <c>T</c> is <see cref="Exception"/> to execute when an <see cref="Exception"/> has been thrown.
        /// </summary>
        private readonly Action<Exception> failed;

        /// <summary>
        /// Creates a new <see cref="AsyncCommandService"/> that performs <see cref="Action"/>s from parameters.
        /// </summary>
        /// <param name="commandStarted">An <see cref="Action"/> to start when <see cref="ICommand.Execute(object)"/> has been called.</param>
        /// <param name="commandFinished">An <see cref="Action"/> to start when <see cref="ICommand.Execute(object)"/> has finished.</param>
        /// <param name="commandError"><see cref="Action{Exception}"/> to execute when an <see cref="Exception"/> has been thrown.</param>
        public AsyncCommandService(Action commandStarted = null, Action commandFinished = null, Action<Exception> commandError = null)
        {
            started = commandStarted;
            finished = commandFinished;
            failed = commandError;
        }

        public async void TryExecAsync(Task task)
        {
            try
            {
                started?.Invoke();
                if (task is null) return;
                await task;
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

        public void TryExecAsync(Action action)
        {
            TryExecAsync(Task.Run(action));
        }

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
