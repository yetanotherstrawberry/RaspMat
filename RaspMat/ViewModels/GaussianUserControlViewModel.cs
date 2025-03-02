using RaspMat.Extensions;
using RaspMat.Models;
using RaspMat.Services.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using static RaspMat.Models.Events;

namespace RaspMat.ViewModels
{
    /// <summary>
    /// ViewModel for the Gaussian elimination algorithm of a <see cref="Matrix"/>.
    /// </summary>
    internal class GaussianUserControlViewModel : ViewModelBase, IEventReceiver<LoadMatrixEvent>, IDisposable
    {

        private readonly Action _lockUI, _unlockUI;
        private readonly Func<bool> _checkIsFree;
        private readonly ISerializationService _serializationService;
        private readonly IViewService _viewService;
        private readonly IEventService _eventService;
        private readonly ICommandingService _commandingService;

        /// <summary>
        /// Generates <see cref="ICommand"/> from <paramref name="task"/> using <see cref="_commandingService"/>.
        /// </summary>
        /// <param name="task">Work to do.</param>
        /// <returns>A <see langword="new"/> <see cref="ICommand"/>.</returns>
        private ICommand GenerateCommand(Func<Task> task = null)
        {
            return _commandingService.CreateFromTask(_lockUI, task, _unlockUI, _checkIsFree);
        }

        /// <summary>
        /// Generates <see cref="ICommand"/> from <paramref name="action"/> using <see cref="_commandingService"/>.
        /// </summary>
        /// <typeparam name="TParameter">Parameter for <see cref="ICommand.Execute(object)"/> and <see cref="ICommand.CanExecute(object)"/>.</typeparam>
        /// <param name="action">Work to do.</param>
        /// <returns>A <see langword="new"/> <see cref="ICommand"/>.</returns>
        private ICommand GenerateCommand<TParameter>(Action<TParameter> action)
        {
            return _commandingService.CreateFromAction(_lockUI, action, _unlockUI, _ => _checkIsFree?.Invoke() ?? true);
        }

        /// <summary>
        /// Generates <see cref="ICommand"/> from <paramref name="action"/> using <see cref="_commandingService"/>.
        /// </summary>
        /// <param name="action">Work to do.</param>
        /// <returns>A <see langword="new"/> <see cref="ICommand"/>.</returns>
        private ICommand GenerateCommand(Action action)
        {
            return _commandingService.CreateFromAction(_lockUI, action, _unlockUI, _checkIsFree);
        }

        /// <summary>
        /// Swaps selected 2 rows of <see cref="CurrentMatrix"/>. Throws when more than 2 or no rows selected. Selection of 1 row is valid.
        /// </summary>
        public ICommand MatSwapRowsComm
        {
            get
            {
                if (_matSwapRowsComm is null)
                {
                    _matSwapRowsComm = GenerateCommand(() =>
                    {
                        if (SelectedRows.Count > 2) throw new ArgumentOutOfRangeException(nameof(SelectedRows.Count));
                        CurrentMatrix = Matrix.SwapMatrix(CurrentMatrix.Rows, SelectedRows.First(), SelectedRows.Last()) * CurrentMatrix;
                    });
                }
                return _matSwapRowsComm;
            }
        }

        /// <summary>
        /// Field for <see cref="MatSwapRowsComm"/>.
        /// </summary>
        private ICommand _matSwapRowsComm;

        /// <summary>
        /// Multiplies <see cref="CurrentMatrix"/> by a scalar.
        /// </summary>
        public ICommand MatScaleComm
        {
            get
            {
                if (_matScaleComm is null) _matScaleComm = GenerateCommand<string>(scalar => CurrentMatrix *= Fraction.Parse(scalar));
                return _matScaleComm;
            }
        }

        /// <summary>
        /// Field for <see cref="MatScaleComm"/>.
        /// </summary>
        private ICommand _matScaleComm;

        /// <summary>
        /// Multiplies selected rows of <see cref="CurrentMatrix"/> by a scalar from the parameter. Any selection (including no rows) is valid.
        /// </summary>
        public ICommand RowScaleComm
        {
            get
            {
                if (_rowsScaleComm is null)
                {
                    _rowsScaleComm = GenerateCommand<string>(scalar =>
                    {
                        var temp = CurrentMatrix;
                        foreach (var rowId in SelectedRows)
                        {
                            temp = Matrix.MultiplicationMatrix(temp.Rows, rowId, Fraction.Parse(scalar)) * temp;
                        }
                        CurrentMatrix = temp;
                    });
                }
                return _rowsScaleComm;
            }
        }

        /// <summary>
        /// Field for <see cref="RowScaleComm"/>.
        /// </summary>
        private ICommand _rowsScaleComm;

        /// <summary>
        /// Adds identity-like rows to the selected half (left or right based on the parameter - <see langword="true"/> for left) of the <see cref="CurrentMatrix"/>.
        /// </summary>
        public ICommand MatAddIComm
        {
            get
            {
                if (_matAddIComm is null) _matAddIComm = GenerateCommand<bool?>(left => CurrentMatrix = CurrentMatrix.WithIdentity(left.Value));
                return _matAddIComm;
            }
        }

        /// <summary>
        /// Field for <see cref="MatAddIComm"/>.
        /// </summary>
        private ICommand _matAddIComm;

        /// <summary>
        /// Removes a selected half (left or right based on the parameter - <see langword="true"/> for left) of the <see cref="CurrentMatrix"/>.
        /// </summary>
        public ICommand MatSliceComm
        {
            get
            {
                if (_matSliceComm is null) _matSliceComm = GenerateCommand<bool?>(left => CurrentMatrix = CurrentMatrix.Slice(left.Value));
                return _matSliceComm;
            }
        }

        /// <summary>
        /// Field for <see cref="MatSliceComm"/>.
        /// </summary>
        private ICommand _matSliceComm;

        /// <summary>
        /// Performes a Gaussian elimination on <see cref="CurrentMatrix"/>. A <see cref="bool"/> parameter sets whether the row echelon result should be reduced.
        /// </summary>
        public ICommand MatGaussComm
        {
            get
            {
                if (_matGaussComm is null)
                {
                    _matGaussComm = GenerateCommand<bool?>(reduced =>
                    {
                        var steps = CurrentMatrix.GaussianElimination(reduced.Value);

                        if (steps.Count > 0)
                        {
                            Steps = steps;
                            CurrentMatrix = Steps.Last().Result;
                        }
                    });
                }
                return _matGaussComm;
            }
        }

        /// <summary>
        /// Field for <see cref="MatGaussComm"/>.
        /// </summary>
        private ICommand _matGaussComm;

        /// <summary>
        /// Transposes (swaps rows with columns) the <see cref="CurrentMatrix"/>.
        /// </summary>
        public ICommand MatTransposeComm
        {
            get
            {
                if (_matTransposeComm is null) _matTransposeComm = GenerateCommand(() => CurrentMatrix = CurrentMatrix.Transpose());
                return _matTransposeComm;
            }
        }

        /// <summary>
        /// Field for <see cref="MatTransposeComm"/>.
        /// </summary>
        private ICommand _matTransposeComm;

        /// <summary>
        /// Handles the input from user. Recreates <see cref="MatrixDataTable"/> based on <see cref="IDialogResult"/> parameter.
        /// </summary>
        public ICommand UserInputComm
        {
            get
            {
                if (_userInputCommDialog is null) _userInputCommDialog = _commandingService.CreateFromAction(action: _viewService.ToggleNewMatDialog);
                return _userInputCommDialog;
            }
        }

        /// <summary>
        /// Field for <see cref="UserInputComm"/>.
        /// </summary>
        private ICommand _userInputCommDialog;

        /// <summary>
        /// Fired when user (de)selects a row. Pass <see cref="IList"/> of selected <see cref="DataRowView"/>s as a parameter.
        /// </summary>
        public ICommand GridSelectedRowComm
        {
            get
            {
                if (_gridSelectedRowComm is null)
                {
                    _gridSelectedRowComm = GenerateCommand<IList>(rowViewList =>
                    {
                        SelectedRows.Clear();
                        SelectedRows.UnionWith(rowViewList.Cast<DataRowView>().Select(rowView => rowView.Row).Select(MatrixDataTable.Rows.IndexOf));
                    });
                }
                return _gridSelectedRowComm;
            }
        }

        /// <summary>
        /// Field for <see cref="GridSelectedRowComm"/>.
        /// </summary>
        private ICommand _gridSelectedRowComm;

        /// <summary>
        /// Serializes the <see cref="CurrentMatrix"/> using <see cref="ISerializationService.Serialize{TDeserialized}(TDeserialized, Stream)"/>.
        /// </summary>
        public ICommand SerializeComm
        {
            get
            {
                if (_serializeComm is null) _serializeComm = GenerateCommand(() => _serializationService.Serialize(CurrentMatrix));
                return _serializeComm;
            }
        }

        /// <summary>
        /// Field for <see cref="SerializeComm"/>.
        /// </summary>
        private ICommand _serializeComm;

        /// <summary>
        /// Deserializes into <see cref="CurrentMatrix"/> using <see cref="ISerializationService.Deserialize{TDeserialized}(Stream)"/>.
        /// </summary>
        public ICommand DeserializeComm
        {
            get
            {
                if (_deserializeComm is null)
                {
                    _deserializeComm = GenerateCommand(task: async () =>
                    {
                        var mat = await _serializationService.Deserialize<Matrix>();
                        if (mat != null) CurrentMatrix = mat;
                    });
                }
                return _deserializeComm;
            }
        }
        private ICommand _deserializeComm;

        /// <summary>
        /// Shows a view containg that list of steps taken by an algorithm.
        /// </summary>
        public ICommand StepListWindowComm
        {
            get
            {
                if (_stepListViewComm is null) _stepListViewComm = GenerateCommand(_viewService.ToggleStepsView);
                return _stepListViewComm;
            }
        }

        /// <summary>
        /// Field for <see cref="StepListWindowComm"/>.
        /// </summary>
        private ICommand _stepListViewComm;

        /// <summary>
        /// <see cref="DataTable"/> visible to the user. Will be converted to <see cref="Matrix"/> when <see cref="CurrentMatrix"/> is accessed.
        /// </summary>
        public DataTable MatrixDataTable
        {
            get => _matrixDataTable;
            private set
            {
                SelectedRows.Clear();
                SetProperty(ref _matrixDataTable, value);
            }
        }

        /// <summary>
        /// Field for <see cref="MatrixDataTable"/>.
        /// </summary>
        private DataTable _matrixDataTable = new Matrix(3, 4, (row, column) => row + column + 1).ToDataTable();

        /// <summary>
        /// Indexes of rows of the <see cref="MatrixDataTable"/> currently selected by the user.
        /// </summary>
        private ISet<int> SelectedRows { get; } = new HashSet<int>();

        /// <summary>
        /// Currently displayed <see cref="MatrixDataTable"/> converted from or to <see cref="Matrix"/>.
        /// </summary>
        private Matrix CurrentMatrix
        {
            get => Matrix.Parse(MatrixDataTable);
            set
            {
                var matrix = MatrixDataTable;
                MatrixDataTable = value.ToDataTable();
                matrix.Dispose();
            }
        }

        /// <summary>
        /// Indicates whether UI should be blocked (<see langword="false"/>) because of an ongoing operation.
        /// </summary>
        public bool IsFree
        {
            get => _isFree;
            private set
            {
                SetProperty(ref _isFree, value);
                _commandingService.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Field for <see cref="IsFree"/>.
        /// </summary>
        private bool _isFree = true;

        /// <summary>
        /// Steps performed by algorithms. Fires <see cref="LoadStepsEvent"/> on change.
        /// </summary>
        public IList<AlgorithmStep<Matrix>> Steps
        {
            get => _steps;
            private set
            {
                if (SetProperty(ref _steps, value)) _eventService.Send(new LoadStepsEvent(Steps));
            }
        }

        /// <summary>
        /// Field for <see cref="Steps"/>.
        /// </summary>
        private IList<AlgorithmStep<Matrix>> _steps = new List<AlgorithmStep<Matrix>>();

        public void Receive(LoadMatrixEvent value)
        {
            CurrentMatrix = value.Data;
        }

        public void Dispose()
        {
            MatrixDataTable.Dispose();
        }

        public GaussianUserControlViewModel(ISerializationService serializationService, IViewService viewService, IEventService eventService, ICommandingService commandingService)
        {
            _checkIsFree = () => IsFree;
            _lockUI = () => IsFree = false;
            _unlockUI = () => IsFree = true;

            _eventService = eventService;
            _commandingService = commandingService;
            _serializationService = serializationService;
            _viewService = viewService;

            _eventService.Subscribe<GaussianUserControlViewModel, LoadMatrixEvent>(this);
        }

    }
}
