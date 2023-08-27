using Prism.Commands;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RaspMat.Helpers
{

    internal class AsyncDelegateCommand<TParameter> : DelegateCommandBase
    {
        private readonly Action<TParameter> _action;
        private readonly Action _start, _finished;
        private readonly Func<TParameter, bool> _canExecute;

        public AsyncDelegateCommand(
            Action<TParameter> action = default,
            Action start = default,
            Action finished = default,
            Func<TParameter, bool> canExecute = default,
            Expression<Func<bool>> canExecuteChanged = default) : base()
        {
            _action = action;
            _canExecute = canExecute;
            _start = start;
            _finished = finished;

            if (!(canExecuteChanged is null)) ObservesPropertyInternal(canExecuteChanged);
        }

        protected AsyncDelegateCommand(Action action, Action start, Action finished, Func<bool> canExecute, Expression<Func<bool>> canExecuteChanged)
            : this(start: start, finished: finished, canExecuteChanged: canExecuteChanged)
        {
            if (!(action is null))
                _action = _ => action();

            if (!(canExecute is null))
                _canExecute = _ => canExecute();
        } // This constructor is supposed to be called from a class that hides the type parameter.

        protected override bool CanExecute(object parameter) => _canExecute?.Invoke((TParameter)parameter) ?? true;

        protected override async void Execute(object parameter)
        {
            // This code assumes there is a global exception handler. In that case, catch is not required.
            try
            {
                _start?.Invoke();
                await Task.Run(() => _action?.Invoke((TParameter)parameter));
            }
            finally
            {
                _finished?.Invoke();
            }
        }
    }

    internal sealed class AsyncDelegateCommand : AsyncDelegateCommand<object>
    {
        public AsyncDelegateCommand(
            Action action = default,
            Action start = default,
            Action finished = default,
            Func<bool> canExecute = default,
            Expression<Func<bool>> canExecuteChanged = default)
            : base(action, start, finished, canExecute, canExecuteChanged)
        {
            // This class allows to create parameterless commands. This constructor hides the parameter by setting its type to object and ignoring any value.
        }
    }

}
