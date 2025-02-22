using CommunityToolkit.Mvvm.Input;
using RaspMat.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RaspMat.Services
{
    internal class AsyncRelayCommandingService : ICommandingService
    {

        private readonly Action<Action> _dispatcherInvoker;

        public AsyncRelayCommandingService(Action<Action> dispatcherInvoker = null)
        {
            _dispatcherInvoker = dispatcherInvoker ?? (action => action?.Invoke());
        }

        public ICommand CreateFromTask(Action before = null, Func<Task> task = null, Action finished = null, Func<bool> canExecute = null)
        {
            if (task is null) task = () => Task.CompletedTask;

            async Task Execute()
            {
                try
                {
                    _dispatcherInvoker(before);
                    await Task.Yield();
                    await task();
                }
                finally
                {
                    _dispatcherInvoker(finished);
                }
            }

            return canExecute is null ? new AsyncRelayCommand(Execute) : new AsyncRelayCommand(Execute, canExecute);
        }

        public ICommand CreateFromTask(Action before = null, Task task = null, Action finished = null, Func<bool> canExecute = null)
        {
            if (task is null) task = Task.CompletedTask;
            return CreateFromTask(before, async () => await task, finished, canExecute);
        }

        public ICommand CreateFromAction(Action before = null, Action action = null, Action finished = null, Func<bool> canExecute = null)
        {
            async Task Execute()
            {
                if (action is null) return;
                await Task.Run(action);
            }

            return CreateFromTask(before, Execute, finished, canExecute);
        }

        public ICommand CreateFromAction<TParameter>(Action before = null, Action<TParameter> action = null, Action finished = null, Predicate<TParameter> canExecute = null)
        {
            if (default(TParameter) != null)
            {
                throw new TypeInitializationException(typeof(TParameter).FullName, new NullReferenceException(nameof(ICommand.CanExecute)));
            }

            async Task Execute(TParameter parameter)
            {
                try
                {
                    _dispatcherInvoker(before);
                    if (action is null) return;
                    await Task.Run(() => action(parameter));
                }
                finally
                {
                    _dispatcherInvoker(finished);
                }
            }

            return canExecute is null ? new AsyncRelayCommand<TParameter>(Execute) : new AsyncRelayCommand<TParameter>(Execute, canExecute);
        }

    }
}
