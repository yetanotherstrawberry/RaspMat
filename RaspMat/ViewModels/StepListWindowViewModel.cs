using RaspMat.Helpers;
using RaspMat.Models;
using RaspMat.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using static RaspMat.Helpers.Events;

namespace RaspMat.ViewModels
{
    /// <summary>
    /// ViewModel for the list of <see cref="AlgorithmStep{T}"/> where <c>T</c> is <see cref="Matrix"/>.
    /// </summary>
    internal class StepListWindowViewModel : ViewModelBase, IObserver<LoadStepsEvent>
    {

        /// <summary>
        /// Service for messaging.
        /// </summary>
        private readonly IEventService _eventService;

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
                if (_loadMatrix == null)
                {
                    _loadMatrix = ICommandHelpers.CreateAsyncICommand<Matrix>(matrix => _eventService.Send(new LoadMatrixEvent(matrix)));
                }
                return _loadMatrix;
            }
        }

        /// <summary>
        /// Field for <see cref="LoadMatrix"/>.
        /// </summary>
        private ICommand _loadMatrix;

        void IObserver<LoadStepsEvent>.OnNext(LoadStepsEvent value) => Steps = value.Data;

        public StepListWindowViewModel(IEventService eventService)
        {
            _eventService = eventService;

            _eventService.Subscribe(this);
        }

    }
}
