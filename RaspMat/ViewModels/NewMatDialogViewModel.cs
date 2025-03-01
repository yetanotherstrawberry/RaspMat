using RaspMat.Models;
using RaspMat.Services.Interfaces;
using System.Windows.Input;
using static RaspMat.Models.Events;

namespace RaspMat.ViewModels
{
    internal class NewMatDialogViewModel : ViewModelBase
    {

        private readonly IViewService _viewService;
        private readonly IEventService _eventService;
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
                    _closeDialogCommand = _commandingService.CreateFromAction(action: () =>
                    {
                        var rows = int.Parse(Rows);
                        var columns = int.Parse(Columns);
                        var fill = Fraction.Parse(Fill);
                        _eventService.Send(new LoadMatrixEvent(new Matrix(rows, columns, (row, column) => fill)));
                        _viewService.ToggleNewMatDialog();
                    });
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
