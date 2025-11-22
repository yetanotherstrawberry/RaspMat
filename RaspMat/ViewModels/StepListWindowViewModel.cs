using RaspMat.Extensions;
using RaspMat.Models;
using RaspMat.Services.Interfaces;
using System.Collections.Generic;
using System.Windows.Input;
using static RaspMat.Models.Events;

namespace RaspMat.ViewModels
{
    /// <summary>
    /// ViewModel for the list of <see cref="AlgorithmStep{T}"/> where <c>T</c> is <see cref="Matrix"/>.
    /// </summary>
    internal class StepListWindowViewModel : ViewModelBase, IEventReceiver<LoadStepsEvent>
    {

        /// <summary>
        /// Used for communication with other ViewModels.
        /// </summary>
        private readonly IEventService _eventService;

        /// <summary>
        /// Used for the creation of <see cref="ICommand"/>s.
        /// </summary>
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

        /// <inheritdoc/>
        public void Receive(LoadStepsEvent value) => Steps = value.Data;

        public StepListWindowViewModel(IEventService eventService, ICommandingService commandingService)
        {
            _eventService = eventService.ThrowIfNull();
            _commandingService = commandingService.ThrowIfNull();

            _eventService.Subscribe<StepListWindowViewModel, LoadStepsEvent>(this);
        }

    }
}
