using RaspMat.Models;
using RaspMat.Services.Interfaces;
using System.Windows.Input;
using static RaspMat.Models.Events;

namespace RaspMat.ViewModels
{
    /// <summary>
    /// ViewModel for creating a <see cref="Matrix"/>.
    /// </summary>
    internal class NewMatDialogViewModel : ViewModelBase
    {

        /// <summary>
        /// Used for communication with the UI.
        /// </summary>
        private readonly IViewService _viewService;

        /// <summary>
        /// Used for communication between ViewModels.
        /// </summary>
        private readonly IEventService _eventService;

        /// <summary>
        /// Used for creating the <see cref="ICommand"/>s.
        /// </summary>
        private readonly ICommandingService _commandingService;

        /// <summary>
        /// Creates a new <see cref="Matrix"/> and calls <see cref="LoadMatrixEvent"/>.
        /// </summary>
        public ICommand CloseDialogCommand
        {
            get
            {
                if (_closeDialogCommand is null)
                {
                    _closeDialogCommand = _commandingService.CreateFromAction(() => IsFree = false, () =>
                    {
                        var fill = Fraction.Parse(Fill);
                        _eventService.Send(new LoadMatrixEvent(new Matrix(int.Parse(Rows), int.Parse(Columns), (row, column) => fill)));
                        _viewService.ToggleNewMatDialog();

                    }, () => IsFree = true);
                }
                return _closeDialogCommand;
            }
        }

        /// <summary>
        /// Field for <see cref="CloseDialogCommand"/>.
        /// </summary>
        private ICommand _closeDialogCommand;

        /// <summary>
        /// Number of rows.
        /// </summary>
        public string Rows
        {
            get => _rows;
            set => SetProperty(ref _rows, value);
        }

        /// <summary>
        /// Field for <see cref="Rows"/>.
        /// </summary>
        private string _rows = string.Empty;

        /// <summary>
        /// Number of columns.
        /// </summary>
        public string Columns
        {
            get => _columns;
            set => SetProperty(ref _columns, value);
        }

        /// <summary>
        /// Field for <see cref="Columns"/>.
        /// </summary>
        private string _columns = string.Empty;

        /// <summary>
        /// Indicated whether the result should be filled with zeros.
        /// </summary>
        public string Fill
        {
            get => _fill;
            set => SetProperty(ref _fill, value);
        }

        /// <summary>
        /// Field for <see cref="Fill"/>.
        /// </summary>
        private string _fill = string.Empty;

        public NewMatDialogViewModel(ICommandingService commandingService, IEventService eventService, IViewService viewService)
        {
            _eventService = eventService;
            _commandingService = commandingService;
            _viewService = viewService;
        }

    }
}
