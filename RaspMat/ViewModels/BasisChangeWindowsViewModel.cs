using RaspMat.Extensions;
using RaspMat.Services.Interfaces;
using System.Windows.Input;

namespace RaspMat.ViewModels
{
    internal class BasisChangeWindowsViewModel : ViewModelBase
    {

        private readonly IEventService _eventService;

        private readonly IViewService _viewService;

        private readonly ICommandingService _commandingService;

        public BasisChangeWindowsViewModel(IEventService eventService, IViewService viewService, ICommandingService commandingService)
        {
            _eventService = eventService.ThrowIfNull();
            _viewService = viewService.ThrowIfNull();
            _commandingService = commandingService.ThrowIfNull();
        }

        /// <summary>
        /// Field for <see cref="StepIndex"/>.
        /// </summary>
        private int _stepIndex;

        /// <summary>
        /// The current step.
        /// </summary>
        public int StepIndex
        {
            get => _stepIndex;
            private set => SetProperty(ref _stepIndex, value);
        }

        /// <summary>
        /// Field for <see cref="Rows"/>.
        /// </summary>
        private int _rows = 3;

        /// <summary>
        /// Indicates the number of rows in the matrix.
        /// </summary>
        public int Rows
        {
            get => _rows;
            private set => SetProperty(ref _rows, value);
        }

        /// <summary>
        /// Field for <see cref="Columns"/>.
        /// </summary>
        private int _columns = 3;

        /// <summary>
        /// Indicates the number of columns in the matrix.
        /// </summary>
        public int Columns
        {
            get => _columns;
            private set => SetProperty(ref _columns, value);
        }

        private ICommand _nextStepCommand;

        public ICommand NextStepCommand
        {
            get
            {
                if (_nextStepCommand is null)
                {
                    _nextStepCommand = _commandingService.CreateFromAction(action: () =>
                    {

                    });
                }
                return _nextStepCommand;
            }
        }

    }
}
