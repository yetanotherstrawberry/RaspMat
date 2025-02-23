using RaspMat.Models;
using System;

namespace RaspMat.Services.Interfaces
{
    /// <summary>
    /// Contains logic for interacting with view for steps of an algorithm.
    /// </summary>
    internal interface IViewService
    {

        /// <summary>
        /// Shows or closes a view with steps performed by an algorithm.
        /// </summary>
        void ToggleStepWindow();

        /// <summary>
        /// Shows or closes a view that creates a new <see cref="Matrix"/>.
        /// </summary>
        void ToggleNewMatDialog();

        /// <summary>
        /// Executes the <paramref name="action"/>, so that it can access the view.
        /// </summary>
        /// <param name="action">Work to do.</param>
        void Execute(Action action);

    }
}
