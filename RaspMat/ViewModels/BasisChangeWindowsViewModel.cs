using RaspMat.Extensions;
using RaspMat.Services.Interfaces;
using System;
using System.Windows.Controls;
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
        /// Field for <see cref="SourceRows"/>.
        /// </summary>
        private int _sourceRows = 3;

        /// <summary>
        /// Indicates the number of rows in the matrix.
        /// </summary>
        public int SourceRows
        {
            get => _sourceRows;
            private set => SetProperty(ref _sourceRows, value);
        }

        /// <summary>
        /// Field for <see cref="SourceColumns"/>.
        /// </summary>
        private int _sourceColumns = 3;

        /// <summary>
        /// Indicates the number of columns in the matrix.
        /// </summary>
        public int SourceColumns
        {
            get => _sourceColumns;
            private set => SetProperty(ref _sourceColumns, value);
        }

        private ICommand _generateMatrix;

        public ICommand GenerateMatrix
        {
            get
            {
                if (_generateMatrix is null)
                {
                    _generateMatrix = _commandingService.CreateFromAction();
                    var a= new DataGrid();
                    
                }
                return _generateMatrix;
            }

        }

    }
}
