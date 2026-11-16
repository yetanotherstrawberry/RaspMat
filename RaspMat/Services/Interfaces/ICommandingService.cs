using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RaspMat.Services.Interfaces
{
    /// <summary>
    /// A service for creation of <see cref="ICommand"/>s.
    /// </summary>
    internal interface ICommandingService
    {

        /// <summary>
        /// Creates a <see langword="new"/> <see cref="ICommand"/>. All parameters can be <see langword="null"/> which means nothing will be done and execution is always allowed.
        /// </summary>
        /// <param name="before"><see cref="Action"/> to synchronously <see cref="Action.Invoke"/> before <paramref name="task"/>.</param>
        /// <param name="task">The main work to be done. Will <see cref="Func{T}.Invoke"/> synchronously and then <see langword="await"/> the result.</param>
        /// <param name="finished"><see cref="Action"/> to synchronously <see cref="Action.Invoke"/> after <paramref name="task"/> <see cref="Task.IsCompleted"/> even if <see cref="Task.IsFaulted"/>.</param>
        /// <param name="canExecute">Determines whether the <see cref="ICommand.CanExecute(object)"/> is <see langword="true"/>.</param>
        /// <returns>A <see langword="new"/> <see cref="ICommand"/>.</returns>
        ICommand CreateFromTask(Action before = null, Func<Task> task = null, Action finished = null, Func<bool> canExecute = null);

        /// <summary>
        /// Creates a <see langword="new"/> <see cref="ICommand"/>. All parameters can be <see langword="null"/> which means nothing will be done and execution is always allowed.
        /// </summary>
        /// <typeparam name="TParameter">The parameter of <paramref name="action"/> and <paramref name="canExecute"/>.</typeparam>
        /// <param name="before"><see cref="Action"/> to synchronously <see cref="Action.Invoke"/> before <paramref name="task"/>.</param>
        /// <param name="action">The main work to be done.</param>
        /// <param name="finished"><see cref="Action"/> to synchronously <see cref="Action.Invoke"/> after <paramref name="task"/> <see cref="Task.IsCompleted"/> even if <see cref="Task.IsFaulted"/>.</param>
        /// <param name="canExecute">Determines whether the <see cref="ICommand.CanExecute(object)"/> is <see langword="true"/>.</param>
        /// <returns>A <see langword="new"/> <see cref="ICommand"/>.</returns>
        ICommand CreateFromAction<TParameter>(Action before = null, Action<TParameter> action = null, Action finished = null, Predicate<TParameter> canExecute = null);

        /// <summary>
        /// Creates a <see langword="new"/> <see cref="ICommand"/>. All parameters can be <see langword="null"/> which means nothing will be done and execution is always allowed.
        /// </summary>
        /// <param name="before"><see cref="Action"/> to synchronously <see cref="Action.Invoke"/> before <paramref name="task"/>.</param>
        /// <param name="action">The main work to be done.</param>
        /// <param name="finished"><see cref="Action"/> to synchronously <see cref="Action.Invoke"/> after <paramref name="task"/> <see cref="Task.IsCompleted"/> even if <see cref="Task.IsFaulted"/>.</param>
        /// <param name="canExecute">Determines whether the <see cref="ICommand.CanExecute(object)"/> is <see langword="true"/>.</param>
        /// <returns>A <see langword="new"/> <see cref="ICommand"/>.</returns>
        ICommand CreateFromAction(Action before = null, Action action = null, Action finished = null, Func<bool> canExecute = null);
        
        /// <summary>
        /// Notifies all <see cref="ICommand"/>s created by <see langword="this"/> <see cref="ICommandingService"/> that the <see cref="ICommand.CanExecute(object)"/> could change.
        /// </summary>
        void NotifyCanExecuteChanged();

    }
}
