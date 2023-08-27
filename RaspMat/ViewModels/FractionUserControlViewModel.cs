using Prism.Events;
using Prism.Mvvm;
using RaspMat.Helpers;
using RaspMat.Models;
using RaspMat.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Windows.Input;

namespace RaspMat.ViewModels
{
    /// <summary>
    /// ViewModel for the view performing operations on <see cref="Fraction"/>s.
    /// </summary>
    internal class FractionUserControlViewModel : BindableBase
    {

        private class OperationPerformedEvent : PubSubEvent<string> { }

        private readonly IEventAggregator _eventAggregator;

        private enum OperationType
        {
            Add,
            Subtract,
            Multiply,
            Divide,
        }

        private readonly IReadOnlyDictionary<string, OperationType> _operations = new Dictionary<string, OperationType>
        {
            { Resources.PLUS_SIGN, OperationType.Add },
            { Resources.MINUS_SIGN, OperationType.Subtract },
            { Resources.MULTIPLY_SIGN, OperationType.Multiply },
            { Resources.DIVIDE_SIGN, OperationType.Divide },
        };

        /// <summary>
        /// Available operations.
        /// </summary>
        public IEnumerable<string> Operations => _operations.Keys;

        /// <summary>
        /// History of operations.
        /// </summary>
        public ObservableCollection<string> History { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Numerator of the left <see cref="Fraction"/> of the equation.
        /// </summary>
        public string LeftFractionUpperInput { get; set; }

        /// <summary>
        /// Denominator of the left <see cref="Fraction"/> of the equation.
        /// </summary>
        public string LeftFractionLowerInput { get; set; }

        /// <summary>
        /// Numerator of the right <see cref="Fraction"/> of the equation.
        /// </summary>
        public string RightFractionUpperInput { get; set; }

        /// <summary>
        /// Denominator of the right <see cref="Fraction"/> of the equation.
        /// </summary>
        public string RightFractionLowerInput { get; set; }

        private Fraction LeftFraction => Fraction.Parse(LeftFractionUpperInput, LeftFractionLowerInput);

        private Fraction RightFraction => Fraction.Parse(RightFractionUpperInput, RightFractionLowerInput);

        /// <summary>
        /// A <see cref="Fraction"/> that is the result of the performed operation.
        /// </summary>
        public Fraction Result
        {
            get => _result;
            private set => SetProperty(ref _result, value);
        }
        private Fraction _result;

        /// <summary>
        /// Executes the requested operation. Requires <see cref="string"/> representation of it from <see cref="Operations"/> passed as a parameter.
        /// </summary>
        public ICommand CalculateComm
        {
            get
            {
                if (_calculateCommand is null)
                {
                    Expression<Func<bool>> isFreeExpr = () => IsFree;
                    var isFreeCheck = isFreeExpr.Compile();

                    _calculateCommand = new AsyncDelegateCommand<string>(operationType =>
                    {
                        switch (_operations[operationType])
                        {
                            case OperationType.Add:
                                Result = LeftFraction + RightFraction;
                                break;
                            case OperationType.Subtract:
                                Result = LeftFraction - RightFraction;
                                break;
                            case OperationType.Multiply:
                                Result = LeftFraction * RightFraction;
                                break;
                            case OperationType.Divide:
                                Result = LeftFraction / RightFraction;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(operationType));
                        }
                        _eventAggregator.GetEvent<OperationPerformedEvent>()
                            .Publish(string.Join(Resources.EQUATION_SPACER, LeftFraction, operationType, RightFraction, Resources.EQUALITY_SIGN, Result));
                    }, () => IsFree = false, () => IsFree = true, _ => isFreeCheck(), isFreeExpr);
                }
                return _calculateCommand;
            }
        }
        private ICommand _calculateCommand;

        /// <summary>
        /// Indicates whether there is an ongoing (<see langword="false"/>) operation.
        /// </summary>
        public bool IsFree
        {
            get => _isFree;
            set => SetProperty(ref _isFree, value);
        }
        private bool _isFree = true;

        private void UpdateHistory(string operationString)
        {
            History.Insert(0, operationString);
        }

        /// <summary>
        /// Creates a new <see cref="FractionUserControlViewModel"/> and subscribes <see cref="UpdateHistory(string)"/> to <see cref="OperationPerformedEvent"/>.
        /// </summary>
        /// <param name="eventAggregator">An instance of <see cref="IEventAggregator"/> used to register <see cref="OperationPerformedEvent"/>.</param>
        public FractionUserControlViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OperationPerformedEvent>().Subscribe(UpdateHistory, ThreadOption.UIThread);
        }

        ~FractionUserControlViewModel()
        {
            _eventAggregator.GetEvent<OperationPerformedEvent>().Unsubscribe(UpdateHistory);
        }

    }
}
