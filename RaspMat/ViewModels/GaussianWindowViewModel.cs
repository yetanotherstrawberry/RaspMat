using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RaspMat.Helpers;
using RaspMat.Interfaces;
using RaspMat.Models;
using RaspMat.Properties;
using RaspMat.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        public ICommand MatSwapRowsComm { get; }
        public ICommand MatScaleComm { get; }
        public ICommand RowScaleComm { get; }
        public ICommand MatAddIComm { get; }
        public ICommand MatRemoveIComm { get; }
        public ICommand MatTransposeComm { get; }
        public ICommand MatGaussComm { get; }
        public ICommand MatEchelonComm { get; }
        public ICommand UserInputComm { get; }
        public ICommand GridSelectedRowComm { get; }
        public ICommand SerializeComm { get; }
        public ICommand DeserializeComm { get; }
        public ICommand StepListWindowComm { get; }

        /// <summary>
        /// Field for <see cref="MatrixDataTable"/>.
        /// </summary>
        private DataTable matrixDataTable = new Matrix(new Fraction[][]
        {
            new Fraction[]
            {
                1, 2, 2,
            },
            new Fraction[]
            {
                1, 0, 1,
            },
            new Fraction[]
            {
                1, 1, 1,
            },
        }).ToDataTable();

        /// <summary>
        /// <see cref="DataTable"/> visible to the user. Will be converted to <see cref="Matrix"/> when <see cref="CurrentMatrix"/> is accessed.
        /// </summary>
        public DataTable MatrixDataTable
        {
            get => matrixDataTable;
            set
            {
                SetProperty(ref matrixDataTable, value);
                SelectedRows.Clear();
            }
        }

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
        /// Field for <see cref="IsFree"/>.
        /// </summary>
        private bool isFree = true;

        /// <summary>
        /// Indicates whether UI should be blocked because of an ongoing operation.
        /// </summary>
        public bool IsFree
        {
            get => isFree;
            private set => SetProperty(ref isFree, value);
        }

        /// <summary>
        /// Field for <see cref="Steps"/>.
        /// </summary>
        private IList<IAlgorithmResult<Matrix>> steps = new List<IAlgorithmResult<Matrix>>();

        /// <summary>
        /// Steps performed by algorithms. Notifies about the value change.
        /// </summary>
        public IList<IAlgorithmResult<Matrix>> Steps
        {
            get => steps;
            set => SetProperty(ref steps, value);
        }

        /// <summary>
        /// Field for <see cref="Scalar"/>.
        /// </summary>
        private string scalar = string.Empty;

        /// <summary>
        /// Scalar from the user.
        /// </summary>
        public string Scalar
        {
            get => scalar;
            set => SetProperty(ref scalar, value);
        }

        /// <summary>
        /// Converts <see cref="Scalar"/> to a <see cref="Fraction"/>.
        /// </summary>
        private Fraction FracScalar => Fraction.Parse(Scalar);

        /// <summary>
        /// Performs a Gaussian Elimination on <see cref="CurrentMatrix"/> and replaces it. Assigns <see cref="Steps"/> based on the algorithm.
        /// </summary>
        private void Gauss(bool reducedEchelon = true)
        {
            var steps = CurrentMatrix.TotalGaussianElimination(reducedEchelon);

            if (steps.Count > 0)
            {
                Steps = steps;
                CurrentMatrix = steps.Last().Result;
            }
        }

        /// <summary>
        /// Gets the number of a row in the <see cref="Matrix"/>.
        /// </summary>
        /// <param name="rowView"><see cref="DataRow"/> of the <see cref="MatrixDataTable"/>.</param>
        /// <returns>Number of the row.</returns>
        private int DataGridRowToMatRow(DataRowView rowView)
        {
            return MatrixDataTable.Rows.IndexOf(rowView.Row);
        }

        /// <summary>
        /// Creates a new <see cref="GaussianWindowViewModel"/> for a <see cref="Matrix"/> and implements its commands.
        /// </summary>
        /// <param name="dialogService">Instance of a <see cref="IDialogService"/> that will be used for user input.</param>
        /// <param name="serializationService">Instance of a <see cref="ISerializationService"/> that will be used for <see cref="Matrix"/> serialization.</param>
        public GaussianWindowViewModel(IDialogService dialogService, ISerializationService serializationService, IErrorService errorService)
        {
            IAsyncCommandService asyncSrv = new AsyncCommandService(() => IsFree = false, () => IsFree = true, exception => errorService.Error(exception));

            MatGaussComm = asyncSrv.GenerateAsyncActionCommand(() => Gauss());
            MatEchelonComm = asyncSrv.GenerateAsyncActionCommand(() => Gauss(reducedEchelon: false));
            MatTransposeComm = asyncSrv.GenerateAsyncActionCommand(() =>
            {
                CurrentMatrix = Matrix.Transpose(CurrentMatrix);
            });
            MatRemoveIComm = asyncSrv.GenerateAsyncActionCommand<bool?>(matrixLeftSide =>
            {
                CurrentMatrix = Matrix.Slice(CurrentMatrix, matrixLeftSide ?? throw new ArgumentNullException(paramName: nameof(matrixLeftSide)));
            });
            MatAddIComm = asyncSrv.GenerateAsyncActionCommand<bool?>(matrixLeftSide =>
            {
                CurrentMatrix = Matrix.AddI(CurrentMatrix, matrixLeftSide ?? throw new ArgumentNullException(paramName: nameof(matrixLeftSide)));
            });
            MatScaleComm = asyncSrv.GenerateAsyncActionCommand(() =>
            {
                CurrentMatrix *= FracScalar;
                Scalar = string.Empty;
            });
            RowScaleComm = asyncSrv.GenerateAsyncActionCommand(() =>
            {
                foreach (var rowId in SelectedRows.ToArray()) // ToArray() will copy the list in order to avoid modyfing the enumerated collection.
                {
                    CurrentMatrix = Matrix.MultiplicationMatrix(CurrentMatrix.Rows, rowId, FracScalar) * CurrentMatrix;
                }
                Scalar = string.Empty;
            });
            MatSwapRowsComm = asyncSrv.GenerateAsyncActionCommand(() =>
            {
                if (SelectedRows.Count > 2)
                    throw new ArgumentOutOfRangeException(
                        paramName: nameof(SelectedRows.Count),
                        actualValue: SelectedRows.Count,
                        message: string.Format(Resources.ERR_ROWS, 2));
                CurrentMatrix = Matrix.SwapMatrix(CurrentMatrix, SelectedRows.First(), SelectedRows.Last()) * CurrentMatrix;
            });
            UserInputComm = new DelegateCommand(() =>
            {
                dialogService.ShowDialog(Resources._NEW_MAT_DIALOG, res =>
                {
                    asyncSrv.TryExecAsync(Task.Run(() =>
                    {
                        if (res.Result != ButtonResult.OK) return;

                        var filler = res.Parameters.GetValue<bool>(Resources._ADD_ZEROS) ? Resources._ZERO : Resources._CELL_DEFAULT;

                        var ret = DataTableHelpers.CreateStrDataTable(
                            res.Parameters.GetValue<int>(Resources._ROWS),
                            res.Parameters.GetValue<int>(Resources._COLS),
                            (row, column) => filler);

                        MatrixDataTable = ret;
                    }));
                });
            });
            GridSelectedRowComm = asyncSrv.GenerateAsyncActionCommand<SelectionChangedEventArgs>(args =>
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
            SerializeComm = asyncSrv.GenerateAsyncActionCommand(() =>
            {
                serializationService.Serialize(CurrentMatrix);
            });
            DeserializeComm = asyncSrv.GenerateAsyncActionCommand(() =>
            {
                var matResult = serializationService.Deserialize<Matrix>();
                if (!matResult.Successful) return;
                CurrentMatrix = matResult.Result;
            });
        }

    }
}
