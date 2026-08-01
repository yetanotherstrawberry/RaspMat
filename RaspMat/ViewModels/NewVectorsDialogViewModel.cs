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
    internal class NewVectorsDialogViewModel : ViewModelBase
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

        public NewVectorsDialogViewModel(ICommandingService commandingService, IEventService eventService, IViewService viewService)
        {
            _eventService = eventService.ThrowIfNull();
            _commandingService = commandingService.ThrowIfNull();
            _viewService = viewService.ThrowIfNull();
        }

    }
}
