using RaspMat.Extensions;
using RaspMat.Models;
using RaspMat.Services.Interfaces;
using System;
using System.Windows.Input;

namespace RaspMat.ViewModels
{
    internal class BasisChangeWindowsViewModel : ViewModelBase
    {

        /// <summary>
        /// Used for communication between components.
        /// </summary>
        private readonly IEventService _eventService;

        /// <summary>
        /// Used to access the view.
        /// </summary>
        private readonly IViewService _viewService;

        /// <summary>
        /// Used for commanding.
        /// </summary>
        private readonly ICommandingService _commandingService;

        public BasisChangeWindowsViewModel(IEventService eventService, IViewService viewService, ICommandingService commandingService)
        {
            _eventService = eventService.ThrowIfNull();
            _viewService = viewService.ThrowIfNull();
            _commandingService = commandingService.ThrowIfNull();
        }

        /// <summary>
        /// Field for <see cref="RefreshStateCommand"/>.
        /// </summary>
        private ICommand _refreshStateCommand;

        /// <summary>
        /// Refreshes the state.
        /// </summary>
        public ICommand RefreshStateCommand
        {
            get
            {
                if (_refreshStateCommand is null)
                {
                    _refreshStateCommand = _commandingService.CreateFromAction(action: () =>
                    {

                    });
                }
                return _refreshStateCommand;
            }
        }

        /// <summary>
        /// Field for <see cref="RefreshStateCommand"/>.
        /// </summary>
        private ICommand _finishCommand;

        /// <summary>
        /// Finishes the operation by creating a <see cref="Matrix"/>.
        /// </summary>
        public ICommand FinishCommand
        {
            get
            {
                if (_finishCommand is null)
                {
                    _finishCommand = _commandingService.CreateFromAction(action: () =>
                    {

                    });
                }
                return _finishCommand;
            }
        }

    }
}
