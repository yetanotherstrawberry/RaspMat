using RaspMat.Extensions;
using RaspMat.Models;
using RaspMat.Services.Interfaces;
using System.Windows.Input;
using static RaspMat.Models.Events;

namespace RaspMat.ViewModels
{
    /// <summary>
    /// ViewModel for direct input that can be parsed to a <see cref="Matrix"/>.
    /// </summary>
    internal class InputMatrixDialogViewModel : ViewModelBase
    {

        /// <summary>
        /// Used for communication with other ViewModels.
        /// </summary>
        private readonly IEventService _eventService;

        /// <summary>
        /// Used for the creation of the <see cref="ICommand"/>s.
        /// </summary>
        private readonly ICommandingService _commandingService;

        /// <summary>
        /// Used for communication with the UI.
        /// </summary>
        private readonly IViewService _viewService;

        public InputMatrixDialogViewModel(ICommandingService commandingService, IEventService eventService, IViewService viewService)
        {
            _eventService = eventService.ThrowIfNull();
            _commandingService = commandingService.ThrowIfNull();
            _viewService = viewService.ThrowIfNull();
        }

        /// <summary>
        /// Field for <see cref="Input"/>.
        /// </summary>
        private string _input = null;

        /// <summary>
        /// The input to parse.
        /// </summary>
        public string Input
        {
            get => _input;
            set => SetProperty(ref _input, value);
        }

        /// <summary>
        /// Field for <see cref="SaveCommand"/>.
        /// </summary>
        private ICommand _save = null;

        /// <summary>
        /// Saves the input from the user.
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                if (_save is null)
                {
                    _save = _commandingService.CreateFromAction(() => IsFree = false, () =>
                    {
                        _eventService.Send(new LoadMatrixEvent(Matrix.Parse(Input)));
                        _viewService.ToggleMatrixInputDialog();
                    }, () => IsFree = true);
                }
                return _save;
            }
        }

    }
}
