using RaspMat.Helpers;
using RaspMat.Models;
using RaspMat.Properties;
using RaspMat.Services.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using static RaspMat.Helpers.Events;
using static RaspMat.Helpers.ICommandHelpers;

namespace RaspMat.ViewModels
{
    /// <summary>
    /// ViewModel for the Gaussian elimination algorithm of a <see cref="Matrix"/>.
    /// </summary>
    internal class GaussianUserControlViewModel : ViewModelBase, IObserver<LoadMatrixEvent>
    {

        private readonly Action _lockUI, _unlockUI;
        private readonly Predicate _checkIsFree;
        private readonly ISerializationService _serializationService;
        private readonly IStepViewService _stepViewService;
        private readonly IEventService _eventService;
        private readonly ICommandingService _commandingService;

        private ICommand GenerateCommand(Action start = null, Func<Task> task = null)
        {
            return CreateAsyncICommand(() =>
            {
                _lockUI?.Invoke();
                start?.Invoke();
            }, task, _unlockUI, () => _checkIsFree());
        }

        private ICommand GenerateCommand<TParameter>(Action<TParameter> action)
        {
            return _commandingService.CreateFromAction(_lockUI, action, _unlockUI, _ => _checkIsFree());
        }

        private ICommand GenerateCommand(Action action)
        {
            return CreateAsyncICommand(() =>
            {
                _lockUI?.Invoke();
                action?.Invoke();
            }, _unlockUI, () => _checkIsFree?.Invoke() ?? true);
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
                        if (SelectedRows.Count > 2)
                            throw new ArgumentOutOfRangeException(nameof(SelectedRows.Count), SelectedRows.Count, string.Format(Resources.ERR_ROWS, 2));
                        CurrentMatrix = Matrix.SwapMatrix(CurrentMatrix, SelectedRows.First(), SelectedRows.Last()) * CurrentMatrix;
                    });
                }
                return _matSwapRowsComm;
            }
        }
        private ICommand _matSwapRowsComm;

        /// <summary>
        /// Multiplies <see cref="CurrentMatrix"/> by a scalar.
        /// </summary>
        public ICommand MatScaleComm
        {
            get
            {
                if (_matScaleComm == null) _matScaleComm = GenerateCommand<string>(scalar => CurrentMatrix *= Fraction.Parse(scalar));
                return _matScaleComm;
            }
        }
        private ICommand _matScaleComm;

        /// <summary>
        /// Multiplies selected rows of <see cref="CurrentMatrix"/> by a scalar. Any selection (including no rows) is valid.
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
        private ICommand _rowsScaleComm;

        /// <summary>
        /// Adds identity-like rows to the selected half (left or right) of the <see cref="CurrentMatrix"/>.
        /// </summary>
        public ICommand MatAddIComm
        {
            get
            {
                if (_matAddIComm is null)
                {
                    _matAddIComm = GenerateCommand<bool?>(left => CurrentMatrix = Matrix.AddI(CurrentMatrix, left.Value));
                }
                return _matAddIComm;
            }
        }
        private ICommand _matAddIComm;

        /// <summary>
        /// Removes a selected half (left or right) of the <see cref="CurrentMatrix"/>.
        /// </summary>
        public ICommand MatSliceComm
        {
            get
            {
                if (_matSliceComm is null)
                {
                    _matSliceComm = GenerateCommand<bool?>(left => CurrentMatrix = Matrix.Slice(CurrentMatrix, left.Value));
                }
                return _matSliceComm;
            }
        }
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
        private ICommand _matGaussComm;

        /// <summary>
        /// Transposes (swaps rows with columns) the <see cref="CurrentMatrix"/>.
        /// </summary>
        public ICommand MatTransposeComm
        {
            get
            {
                if (_matTransposeComm is null)
                {
                    _matTransposeComm = GenerateCommand(() => CurrentMatrix = Matrix.Transpose(CurrentMatrix));
                }
                return _matTransposeComm;
            }
        }
        private ICommand _matTransposeComm;

        /// <summary>
        /// Handles the input from user. Recreates <see cref="MatrixDataTable"/> based on <see cref="IDialogResult"/> parameter.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <see cref="UserInputComm"/> has been <see cref="ICommand.Execute(object)"/>d,
        /// but <see cref="_userInputCommHander"/>'s <see cref="ICommand.CanExecute(object)"/> returns <see langword="false"/>.
        /// </exception>
        public ICommand UserInputComm
        {
            get
            {
                /* if (_userInputCommDialog is null)
                 {
                     var command = GenerateCommand(() => _dialogService.ShowDialog(Resources._NEW_MAT_DIALOG, res =>
                     {
                         if (_userInputCommHander is null)
                         {
                             _userInputCommHander = GenerateCommand<IDialogResult>(result =>
                             {
                                 if (result.Result != ButtonResult.OK) return;

                                 var filler = result.Parameters.GetValue<bool>(Resources._ADD_ZEROS) ? Resources._ZERO : Resources._CELL_DEFAULT;

                                 var ret = DataTableHelpers.CreateStrDataTable(
                                     result.Parameters.GetValue<int>(Resources._ROWS),
                                     result.Parameters.GetValue<int>(Resources._COLS),
                                     (row, column) => filler);

                                 MatrixDataTable = ret;
                             });
                         }
                         if (!_userInputCommHander.CanExecute(res))
                             throw new InvalidOperationException(nameof(ICommand.CanExecute));
                         _userInputCommHander.Execute(res);
                     }));
                     //command.ObservesCanExecute(_checkIsFreeExpr);
                     _userInputCommDialog = command;
                 }*/
                return _userInputCommDialog;
            }
        }
        private ICommand _userInputCommDialog;
        private ICommand _userInputCommHander;

        /// <summary>
        /// Fired when user (de)selects a row. Pass <see cref="IList"/> of selected <see cref="DataRowView"/>s as a parameter.
        /// </summary>
        public ICommand GridSelectedRowComm
        {
            get
            {
                if (_gridSelectedRowComm == null)
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
                if (_serializeComm == null) _serializeComm = GenerateCommand(() => _serializationService.Serialize(CurrentMatrix));
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
                if (_stepListViewComm == null) _stepListViewComm = GenerateCommand(_stepViewService.Toggle);
                return _stepListViewComm;
            }
        }
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
            set => MatrixDataTable = value.ToDataTable();
        }

        /// <summary>
        /// Indicates whether UI should be blocked (<see langword="false"/>) because of an ongoing operation.
        /// </summary>
        public bool IsFree
        {
            get => _isFree;
            private set => SetProperty(ref _isFree, value);
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

        void IObserver<LoadMatrixEvent>.OnNext(LoadMatrixEvent value) => CurrentMatrix = value.Data;

        public GaussianUserControlViewModel(ISerializationService serializationService, IStepViewService stepViewService, IEventService eventService, ICommandingService commandingService)
        {
            _eventService = eventService;
            _commandingService = commandingService;
            _serializationService = serializationService;
            _stepViewService = stepViewService;

            _lockUI = () => IsFree = false;
            _unlockUI = () => IsFree = true;
            _checkIsFree = () => IsFree;

            _eventService.Subscribe(this);
        }

    }
}
