using RaspMat.Extensions;
using RaspMat.Helpers;
using RaspMat.Models;
using RaspMat.Services.Interfaces;
using System;
using System.Data;
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
            SourceDim = 3;
            TargetDim = 3;
        }

        /// <summary>
        /// Field for <see cref="SourceDim"/>.
        /// </summary>
        private ushort _sourceDim;

        /// <summary>
        /// Source dimension size.
        /// </summary>
        public ushort SourceDim
        {
            get => _sourceDim;
            set
            {
                SetProperty(ref _sourceDim, value);
                SourceGrid.SetColumns(value).SetRows(value);
            }
        }

        /// <summary>
        /// Field for <see cref="TargetDim"/>.
        /// </summary>
        private ushort _targetDim;

        /// <summary>
        /// Target dimension size.
        /// </summary>
        public ushort TargetDim
        {
            get => _targetDim;
            set
            {
                SetProperty(ref _targetDim, value);
                TargetGrid.SetColumns(value).SetRows(value);
            }
        }

        /// <summary>
        /// Field for <see cref="SourceGrid"/>.
        /// </summary>
        private DataTable _sourceGrid = new DataTable();

        /// <summary>
        /// The grid representing the source dimension.
        /// </summary>
        public DataTable SourceGrid
        {
            get => _sourceGrid;
            set => SetProperty(ref _sourceGrid, value);
        }

        /// <summary>
        /// Field for <see cref="TargetGrid"/>.
        /// </summary>
        private DataTable _targetGrid = new DataTable();

        /// <summary>
        /// The grid representing the target dimension.
        /// </summary>
        public DataTable TargetGrid
        {
            get => _targetGrid;
            set => SetProperty(ref _targetGrid, value);
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
                    _finishCommand = _commandingService.CreateFromAction(() => IsFree = false, () =>
                    {
                        var sourceMatrix = Matrix.Parse(SourceGrid);
                        var targetMatrix = Matrix.Parse(TargetGrid);
                        var matrix = Algorithms.BasisChangeMatrix(sourceMatrix, targetMatrix);
                        _eventService.Send(new Events.LoadMatrixEvent(matrix));
                    }, () => IsFree = true);
                }
                return _finishCommand;
            }
        }

        /// <summary>
        /// Used upon selecting a row.
        /// </summary>
        public ICommand GridSelectedRowComm { get; set; } = null;

    }
}
