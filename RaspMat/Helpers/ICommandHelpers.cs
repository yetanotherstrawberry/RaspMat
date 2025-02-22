using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RaspMat.Helpers
{

    internal static class ICommandHelpers
    {

        public delegate bool Predicate();

        public static ICommand CreateAsyncICommand(
            Action start = null,
            Func<Task> action = null,
            Action finished = null,
            Func<bool> canExecute = null)
        {
            return new AsyncRelayCommand(() => new Task(async () =>
            {
                try
                {
                    start?.Invoke();
                    await action();
                    // This code assumes there is a global exception handler. In that case, catch is not required.
                }
                finally
                {
                    finished?.Invoke();
                }
            }), canExecute ?? (() => true));
        }

        public static ICommand CreateAsyncICommand<TParameter>(
        Action<TParameter> action = null,
        Action finished = null,
        Predicate<TParameter> canExecute = null)
        {
            return new AsyncRelayCommand<TParameter>(parameter => Task.Run(() =>
            {
                try
                {
                    action?.Invoke(parameter);
                    // This code assumes there is a global exception handler. In that case, catch is not required.
                }
                finally
                {
                    finished?.Invoke();
                }
            }), canExecute ?? (_ => true));
        }

        public static ICommand CreateAsyncICommand(Action action = null, Action finished = null, Predicate canExecute = null)
            => CreateAsyncICommand(
                action is null ? null as Action<object> : _ => action.Invoke(),
                finished is null ? null : finished,
                canExecute is null ? null as Predicate<object> : _ => canExecute.Invoke());

    }

}
