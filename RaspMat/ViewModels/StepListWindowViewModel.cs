using RaspMat.Models;
using RaspMat.Services.Interfaces;
using RaspMat.ViewModels.Interfaces;
using System.Collections.Generic;
using System.Windows.Input;
using static RaspMat.Helpers.Events;

namespace RaspMat.ViewModels
{
    /// <summary>
    /// ViewModel for the list of <see cref="AlgorithmStep{T}"/> where <c>T</c> is <see cref="Matrix"/>.
    /// </summary>
    internal class StepListWindowViewModel : ViewModelBase, IEventReceiver<LoadStepsEvent>
    {

        private readonly IEventService _eventService;
        private readonly ICommandingService _commandingService;

        /// <summary>
        /// Steps of the algorithm shown to the user.
        /// </summary>
        public ICollection<AlgorithmStep<Matrix>> Steps
        {
            get => _steps;
            private set => SetProperty(ref _steps, value);
        }

        /// <summary>
        /// Field for <see cref="Steps"/>.
        /// </summary>
        private ICollection<AlgorithmStep<Matrix>> _steps;

        /// <summary>
        /// Loads a <see cref="Matrix"/> selected by the user to the main view.
        /// </summary>
        public ICommand LoadMatrix
        {
            get
            {
                if (_loadMatrix is null) _loadMatrix = _commandingService.CreateFromAction<Matrix>(action: matrix => _eventService.Send(new LoadMatrixEvent(matrix)));
                return _loadMatrix;
            }
        }

        /// <summary>
        /// Field for <see cref="LoadMatrix"/>.
        /// </summary>
        private ICommand _loadMatrix;

        public void Receive(LoadStepsEvent value) => Steps = value.Data;

        public StepListWindowViewModel(IEventService eventService, ICommandingService commandingService)
        {
            _eventService = eventService;
            _commandingService = commandingService;

            _eventService.Subscribe<StepListWindowViewModel, LoadStepsEvent>(this);
        }

    }
}
