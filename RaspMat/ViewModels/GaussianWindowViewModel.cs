using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RaspMat.DTOs;
using RaspMat.Helpers;
using RaspMat.Interfaces;
using RaspMat.Models;
using RaspMat.Properties;
using RaspMat.Services;
using RaspMat.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace RaspMat.ViewModels
{
    /// <summary>
    /// ViewModel for the Gaussian elimination algorithm of a <see cref="Matrix"/>.
    /// </summary>
    internal class GaussianWindowViewModel : BindableBase
    {

        private readonly Action _lockUI, _unlockUI;
        private readonly Expression<Func<bool>> _checkIsFreeExpr;
        private readonly Func<bool> _checkIsFree;
        private readonly ISerializationService _serializationService;
        private readonly IDialogService _dialogService;
        private readonly IFileService _fileService;

        private ICommand GenerateCommand(Action action, bool blockable = true)
        {
            return blockable ? new AsyncDelegateCommand(action, _lockUI, _unlockUI, _checkIsFree, _checkIsFreeExpr) : new AsyncDelegateCommand(action);
        }

        private ICommand GenerateCommand<TParameter>(Action<TParameter> action, bool blockable = true)
        {
            return blockable ? new AsyncDelegateCommand<TParameter>(action, _lockUI, _unlockUI, _ => _checkIsFree(), _checkIsFreeExpr) : new AsyncDelegateCommand<TParameter>(action);
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
                if (_matScaleComm is null)
                {
                    _matScaleComm = GenerateCommand<string>(scalar => CurrentMatrix *= Fraction.Parse(scalar));
                }
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
                    _matAddIComm = GenerateCommand<bool>(left => CurrentMatrix = Matrix.AddI(CurrentMatrix, left));
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
                    _matSliceComm = GenerateCommand<bool>(left => CurrentMatrix = Matrix.Slice(CurrentMatrix, left));
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
                    _matGaussComm = GenerateCommand<bool>(reduced =>
                    {
                        var steps = CurrentMatrix.GaussianElimination(reduced);

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
        /// 
        /// </summary>
        public ICommand UserInputComm
        {
            get
            {
                if (_userInputComm is null)
                {
                    _userInputComm = new DelegateCommand(() => _dialogService.ShowDialog(Resources._NEW_MAT_DIALOG, res =>
                    {
                        GenerateCommand(() =>
                        {
                            if (res.Result != ButtonResult.OK) return;

                            var filler = res.Parameters.GetValue<bool>(Resources._ADD_ZEROS) ? Resources._ZERO : Resources._CELL_DEFAULT;

                            var ret = DataTableHelpers.CreateStrDataTable(
                                res.Parameters.GetValue<int>(Resources._ROWS),
                                res.Parameters.GetValue<int>(Resources._COLS),
                                (row, column) => filler);

                            MatrixDataTable = ret;
                        }).Execute(default);
                    }));
                }
                return _userInputComm;
            }
        }
        private ICommand _userInputComm;

        public ICommand GridSelectedRowComm { get; }
        public ICommand SerializeComm { get; }
        public ICommand DeserializeComm { get; }

        /// <summary>
        /// Shows a view containg that list of steps taken by an algorithm.
        /// </summary>
        public ICommand StepListWindowComm
        {
            get
            {
                if (_stepListViewComm is null)
                {
                    _stepListViewComm = new DelegateCommand(() =>
                    {
                        // TODO: Use service to create window
                        var vm = new StepListWindowViewModel(Steps);
                        PropertyChanged += (object sender, PropertyChangedEventArgs args) =>
                        {
                            if (args.PropertyName.Equals(nameof(Steps)))
                            {
                                vm.ChangeSteps(Steps);
                            }
                        };

                        var window = new StepListWindow
                        {
                            DataContext = vm,
                        };
                        window.Show();
                    });
                }
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
            set
            {
                SetProperty(ref _matrixDataTable, value);
                SelectedRows.Clear();
            }
        }
        private DataTable _matrixDataTable = new Matrix(3, 4, (row, column) => row + column + 1).ToDataTable();

        /// <summary>
        /// Indexes of rows of the <see cref="MatrixDataTable"/> currently selected by the user.
        /// </summary>
        private ICollection<int> SelectedRows { get; } = new List<int>();

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
            private set
            {
                SetProperty(ref _isFree, value);
            }
        }
        private bool _isFree = true;

        /// <summary>
        /// Steps performed by algorithms. Notifies about the value change.
        /// </summary>
        public IList<AlgorithmStepDTO<Matrix>> Steps
        {
            get => steps;
            set => SetProperty(ref steps, value);
        }
        private IList<AlgorithmStepDTO<Matrix>> steps = new List<AlgorithmStepDTO<Matrix>>();

        /// <summary>
        /// Gets the index of a row in the <see cref="Matrix"/>. -1 if not found.
        /// </summary>
        /// <param name="rowView"><see cref="DataRow"/> of the <see cref="MatrixDataTable"/>.</param>
        /// <returns>Index of the row or -1.</returns>
        private int DataGridRowToMatRow(DataRowView rowView)
        {
            var ret = MatrixDataTable.Rows.IndexOf(rowView.Row);
            return ret;
        }

        /// <summary>
        /// Creates a new <see cref="GaussianWindowViewModel"/> for a <see cref="Matrix"/> and implements its commands.
        /// </summary>
        /// <param name="dialogService">Instance of a <see cref="IDialogService"/> that will be used for user input.</param>
        /// <param name="serializationService">Instance of a <see cref="ISerializationService"/> that will be used for <see cref="Matrix"/> serialization.</param>
        public GaussianWindowViewModel(IDialogService dialogService, ISerializationService serializationService, IFileService fileService)
        {
            _dialogService = dialogService;
            _fileService = fileService;
            _serializationService = serializationService;

            _lockUI = () => IsFree = false;
            _unlockUI = () => IsFree = true;
            _checkIsFreeExpr = () => IsFree;
            _checkIsFree = _checkIsFreeExpr.Compile();


            IAsyncCommandService asyncService = new AsyncCommandService(_lockUI, _unlockUI);

            GridSelectedRowComm = asyncService.GenerateAsyncActionCommand<SelectionChangedEventArgs>(args =>
            {
                // Remove all unselected rows.
                foreach (DataRowView item in args.RemovedItems)
                {
                    SelectedRows.Remove(DataGridRowToMatRow(item));
                }

                // Add all selected rows.
                foreach (DataRowView item in args.AddedItems)
                {
                    SelectedRows.Add(DataGridRowToMatRow(item));
                }
            });
            SerializeComm = new DelegateCommand(() =>
            {
                var stream = fileService.NewFile(); // FileDialog must be called from the main thread.
                asyncService.TryExecAsync(Task.Run(() =>
                {
                    serializationService.Serialize(CurrentMatrix, stream);
                }));
            });
            DeserializeComm = new DelegateCommand(() =>
            {
                var stream = fileService.OpenFile(); // FileDialog must be called from the main thread.
                asyncService.TryExecAsync(Task.Run(() =>
                {
                    var matResult = serializationService.Deserialize<Matrix>(stream);
                    if (!matResult.Successful) return;
                    CurrentMatrix = matResult.Result;
                }));
            });
        }

    }
}
