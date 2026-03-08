using RaspMat.Models;
using System;
using System.Threading.Tasks;

namespace RaspMat.Services.Interfaces
{
    /// <summary>
    /// Contains logic for interacting with the UI.
    /// </summary>
    internal interface IViewService
    {

        /// <summary>
        /// Shows or closes a view with steps performed by an algorithm.
        /// </summary>
        void ToggleStepsView();

        /// <summary>
        /// Shows or closes the main view.
        /// </summary>
        void ToggleMainWindow();

        /// <summary>
        /// Shows or closes a view that creates a new <see cref="Matrix"/>.
        /// </summary>
        void ToggleNewMatrixDialog();

        /// <summary>
        /// Shows or closes a view that parses text into a <see cref="Matrix"/>.
        /// </summary>
        void ToggleMatrixInputDialog();

        /// <summary>
        /// Executes the <paramref name="action"/>, so that it can access the view.
        /// </summary>
        /// <param name="action">Work to do.</param>
        void Execute(Action action);

        /// <summary>
        /// Executes the <paramref name="callback"/>, so that it can access the view.
        /// </summary>
        /// <typeparam name="TResult">The data to <see langword="return"/>.</typeparam>
        /// <param name="callback">Work to do.</param>
        /// <returns>A <see langword="new"/> <see cref="Task"/> that can be used to access the <paramref name="callback"/>.</returns>
        Task<TResult> ExecuteAsync<TResult>(Func<TResult> callback);

    }
}
