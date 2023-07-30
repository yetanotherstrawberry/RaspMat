using Prism.Mvvm;
using RaspMat.DTOs;
using RaspMat.Models;
using System.Collections.Generic;

namespace RaspMat.ViewModels
{
    /// <summary>
    /// ViewModel for the list of <see cref="AlgorithmStepDTO{T}"/> where <c>T</c> is <see cref="Matrix"/>.
    /// </summary>
    internal class StepListWindowViewModel : BindableBase
    {

        /// <summary>
        /// Steps of the algorithm shown to the user.
        /// </summary>
        public IList<AlgorithmStepDTO<Matrix>> Steps
        {
            get => _steps;
            set => SetProperty(ref _steps, value);
        }
        private IList<AlgorithmStepDTO<Matrix>> _steps;

        /// <summary>
        /// Creates a new instance of <see cref="StepListWindowViewModel"/> with <paramref name="steps"/> for a <see cref="Matrix"/>.
        /// </summary>
        /// <param name="steps"><see cref="IAlgorithmResult{T}.Step"/>s to be shown to the user.</param>
        public StepListWindowViewModel(IList<AlgorithmStepDTO<Matrix>> steps)
        {
            _steps = steps;
        }

    }
}
