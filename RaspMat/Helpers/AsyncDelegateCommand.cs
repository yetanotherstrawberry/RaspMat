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

        public AsyncDelegateCommand(Action<TParameter> action = null, Action start = null, Action finished = null, Func<TParameter, bool> canExecute = null, Expression<Func<bool>> canExecuteChanged = null) : base()
        {
            _action = action;
            _canExecute = canExecute;
            _start = start;
            _finished = finished;
            if (!(canExecuteChanged is null)) ObservesPropertyInternal(canExecuteChanged);
        }

        protected override bool CanExecute(object parameter) => _canExecute?.Invoke((TParameter)parameter) ?? true;

        protected override async void Execute(object parameter)
        {
            // This implementation assumes there is a global WPF exception handler. In that case, catch is not required.
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
        public AsyncDelegateCommand(Action action = null, Action start = null, Action finished = null, Func<bool> canExecute = null, Expression<Func<bool>> canExecuteChanged = null)
            : base(_ => action?.Invoke(), start, finished, _ => canExecute?.Invoke() ?? true, canExecuteChanged)
        {
            // This class allows to create parameterless commands. This constructor hides the parameter by setting its type to object and ignoring any value.
        }
    }

}
