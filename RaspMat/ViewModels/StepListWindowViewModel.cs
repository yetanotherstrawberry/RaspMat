using Prism.Events;
using Prism.Mvvm;
using RaspMat.DTOs;
using RaspMat.Helpers;
using RaspMat.Models;
using System.Collections.Generic;
using System.Windows.Input;

namespace RaspMat.ViewModels
{
    /// <summary>
    /// ViewModel for the list of <see cref="AlgorithmStepDTO{T}"/> where <c>T</c> is <see cref="Matrix"/>.
    /// </summary>
    internal class StepListWindowViewModel : BindableBase
    {

        private readonly IEventAggregator _eventAggregator;

        private void Update(ICollection<AlgorithmStepDTO<Matrix>> steps)
        {
            Steps = steps;
        }

        /// <summary>
        /// Steps of the algorithm shown to the user.
        /// </summary>
        public ICollection<AlgorithmStepDTO<Matrix>> Steps
        {
            get => _steps;
            private set => SetProperty(ref _steps, value);
        }
        private ICollection<AlgorithmStepDTO<Matrix>> _steps;

        /// <summary>
        /// Loads a <see cref="Matrix"/> selected by the user to the main view.
        /// </summary>
        public ICommand LoadMatrix
        {
            get
            {
                if (_loadMatrix is null)
                {
                    _loadMatrix = new AsyncDelegateCommand<Matrix>(_eventAggregator.GetEvent<Events.LoadMatrixEvent>().Publish);
                }
                return _loadMatrix;
            }
        }
        private ICommand _loadMatrix;

        /// <summary>
        /// Creates a new instance of <see cref="StepListWindowViewModel"/> with <paramref name="steps"/> for a <see cref="Matrix"/>.
        /// </summary>
        /// <param name="eventAggregator">Event aggregator that will use <see cref="Events.LoadStepsEvent"/> to update <see cref="Steps"/>.</param>
        public StepListWindowViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<Events.LoadStepsEvent>().Subscribe(Update, ThreadOption.UIThread);
        }

        ~StepListWindowViewModel()
        {
            _eventAggregator.GetEvent<Events.LoadStepsEvent>().Unsubscribe(Update);
        }

    }
}
