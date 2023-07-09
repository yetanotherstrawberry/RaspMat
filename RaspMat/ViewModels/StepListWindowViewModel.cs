using Prism.Mvvm;
using RaspMat.Interfaces;
using RaspMat.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RaspMat.ViewModels
{
    /// <summary>
    /// ViewModel for the list of <see cref="IAlgorithmResult{T}"/> where <c>T</c> is <see cref="Matrix"/>.
    /// </summary>
    internal class StepListWindowViewModel : BindableBase
    {

        /// <summary>
        /// Field for <see cref="Steps"/>.
        /// </summary>
        private IList<IAlgorithmResult<Matrix>> steps;

        /// <summary>
        /// Steps of the algorithm shown to the user.
        /// </summary>
        public IList<IAlgorithmResult<Matrix>> Steps
        {
            get => steps;
            private set => SetProperty(ref steps, value);
        }

        /// <summary>
        /// Creates a new instance of <see cref="StepListWindowViewModel"/> with <paramref name="steps"/> for a <see cref="Matrix"/>.
        /// </summary>
        /// <param name="steps"><see cref="IAlgorithmResult{T}.Step"/>s to be shown to the user.</param>
        public StepListWindowViewModel(IList<IAlgorithmResult<Matrix>> steps)
        {
            Steps = steps;
        }

    }
}
