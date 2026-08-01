using CommunityToolkit.Mvvm.Input;
using RaspMat.Extensions;
using RaspMat.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RaspMat.Services
{
    /// <summary>
    /// Implements <see cref="ICommandingService"/> so that <see cref="ICommand"/>s will run asynchronously and track them weakly.
    /// </summary>
    internal class AsyncRelayCommandingService : ICommandingService
    {

        /// <summary>
        /// Used to access the UI.
        /// </summary>
        private readonly IViewService _viewService;

        /// <summary>
        /// An <see cref="ISet{T}"/> that stores <see cref="ICommand"/>s created by this <see cref="ICommandingService"/>.
        /// </summary>
        private ISet<WeakReference<IRelayCommand>> Commands { get; } = new HashSet<WeakReference<IRelayCommand>>();

        /// <summary>
        /// Adds a <see cref="WeakReference{T}"/> to <see cref="Commands"/>.
        /// </summary>
        /// <param name="command"></param>
        private void AddCommand(IRelayCommand command) => Commands.Add(new WeakReference<IRelayCommand>(command));

        /// <summary>
        /// Creates a <see langword="new"/> instance of <see cref="AsyncRelayCommandingService"/>.
        /// </summary>
        /// <param name="viewService">Used to access the UI.</param>
        public AsyncRelayCommandingService(IViewService viewService)
        {
            _viewService = viewService.ThrowIfNull();
        }

        /// <inheritdoc/>
        public ICommand CreateFromTask(Action before = null, Func<Task> task = null, Action finished = null, Func<bool> canExecute = null)
        {
            if (task is null) task = () => Task.CompletedTask;

            async Task Execute()
            {
                try
                {
                    _viewService.Execute(before);
                    await task();
                }
                finally
                {
                    _viewService.Execute(finished);
                }
            }

            var ret = canExecute is null ? new AsyncRelayCommand(Execute) : new AsyncRelayCommand(Execute, canExecute);
            AddCommand(ret);
            return ret;
        }

        /// <inheritdoc/>
        public ICommand CreateFromAction(Action before = null, Action action = null, Action finished = null, Func<bool> canExecute = null)
        {
            async Task Execute()
            {
                if (action is null) return;
                await Task.Run(action);
            }

            return CreateFromTask(before, Execute, finished, canExecute);
        }

        /// <inheritdoc/>
        public ICommand CreateFromAction<TParameter>(Action before = null, Action<TParameter> action = null, Action finished = null, Predicate<TParameter> canExecute = null)
        {
            if (default(TParameter) != null) // AsyncRelayCommand requires parameter to be null-able.
            {
                throw new TypeInitializationException(typeof(TParameter).FullName, new NullReferenceException(nameof(ICommand.CanExecute)));
            }

            async Task Execute(TParameter parameter)
            {
                try
                {
                    _viewService.Execute(before);
                    if (action is null) return;
                    await Task.Run(() => action(parameter));
                }
                finally
                {
                    _viewService.Execute(finished);
                }
            }

            var ret = canExecute is null ? new AsyncRelayCommand<TParameter>(Execute) : new AsyncRelayCommand<TParameter>(Execute, canExecute);
            AddCommand(ret);
            return ret;
        }

        /// <inheritdoc/>
        public void NotifyCanExecuteChanged()
        {
            var toRemove = new HashSet<WeakReference<IRelayCommand>>();
            foreach (var commandRef in Commands)
            {
                if (commandRef.TryGetTarget(out var relayCommand)) relayCommand.NotifyCanExecuteChanged();
                else Commands.Remove(commandRef);
            }
            Commands.ExceptWith(toRemove);
        }

    }
}
