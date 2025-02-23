using RaspMat.Models;
using RaspMat.Properties;
using RaspMat.Services.Interfaces;
using RaspMat.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Windows.Input;
using static RaspMat.Helpers.Events;

namespace RaspMat.ViewModels
{
    /// <summary>
    /// ViewModel for the view performing operations on <see cref="Fraction"/>s.
    /// </summary>
    internal class FractionUserControlViewModel : ViewModelBase, IEventReceiver<OperationPerformedEvent>
    {

        private readonly IEventService _eventService;
        private readonly ICommandingService _commandingService;
        private readonly IViewService _viewService;

        /// <summary>
        /// Allowed operations on <see cref="LeftFraction"/> and <see cref="RightFraction"/>. Passed to <see cref="CalculateComm"/>.
        /// </summary>
        public enum OperationType
        {
            Add = 1,
            Subtract,
            Multiply,
            Divide,
        }

        /// <summary>
        /// Allowed <see cref="OperationType"/>s.
        /// </summary>
        public IReadOnlyDictionary<OperationType, string> Operations { get; } = new Dictionary<OperationType, string>
        {
            { OperationType.Add, Resources.PLUS_SIGN },
            { OperationType.Subtract, Resources.MINUS_SIGN },
            { OperationType.Multiply, Resources.MULTIPLY_SIGN },
            { OperationType.Divide, Resources.DIVIDE_SIGN },
        };

        /// <summary>
        /// History of operations.
        /// </summary>
        public ObservableCollection<string> History { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Numerator of the left <see cref="Fraction"/> of the equation.
        /// </summary>
        public string LeftFractionUpperInput
        {
            get => _leftFractionUpperInput;
            set => SetProperty(ref _leftFractionUpperInput, value);
        }

        /// <summary>
        /// Field for <see cref="LeftFractionUpperInput"/>.
        /// </summary>
        private string _leftFractionUpperInput = string.Empty;

        /// <summary>
        /// Denominator of the left <see cref="Fraction"/> of the equation.
        /// </summary>
        public string LeftFractionLowerInput
        {
            get => _leftFractionLowerInput;
            set => SetProperty(ref _leftFractionLowerInput, value);
        }

        /// <summary>
        /// Field for <see cref="LeftFractionLowerInput"/>.
        /// </summary>
        private string _leftFractionLowerInput = string.Empty;

        /// <summary>
        /// Numerator of the right <see cref="Fraction"/> of the equation.
        /// </summary>
        public string RightFractionUpperInput
        {
            get => _rightFractionUpperInput;
            set => SetProperty(ref _rightFractionUpperInput, value);
        }

        /// <summary>
        /// Field for <see cref="RightFractionUpperInput"/>.
        /// </summary>
        private string _rightFractionUpperInput = string.Empty;

        /// <summary>
        /// Denominator of the right <see cref="Fraction"/> of the equation.
        /// </summary>
        public string RightFractionLowerInput
        {
            get => _rightFractionLowerInput;
            set => SetProperty(ref _rightFractionLowerInput, value);
        }

        /// <summary>
        /// Field for <see cref="RightFractionLowerInput"/>.
        /// </summary>
        private string _rightFractionLowerInput = string.Empty;

        /// <summary>
        /// Passes <see cref="LeftFractionUpperInput"/> and <see cref="LeftFractionLowerInput"/> to <see cref="Fraction.Parse(string, string)"/>.
        /// </summary>
        private Fraction LeftFraction => Fraction.Parse(LeftFractionUpperInput, LeftFractionLowerInput);

        /// <summary>
        /// Passes <see cref="RightFractionUpperInput"/> and <see cref="RightFractionLowerInput"/> to <see cref="Fraction.Parse(string, string)"/>.
        /// </summary>
        private Fraction RightFraction => Fraction.Parse(RightFractionUpperInput, RightFractionLowerInput);

        /// <summary>
        /// A <see cref="Fraction"/> that is the result of the performed operation.
        /// </summary>
        public Fraction Result
        {
            get => _result;
            private set => SetProperty(ref _result, value);
        }

        /// <summary>
        /// Field for <see cref="Result"/>.
        /// </summary>
        private Fraction _result;

        /// <summary>
        /// Executes the requested operation. Requires <see cref="OperationType"/> passed as a parameter.
        /// </summary>
        public ICommand CalculateComm
        {
            get
            {
                if (_calculateCommand is null)
                {
                    _calculateCommand = _commandingService.CreateFromAction<OperationType?>(() => IsFree = false, operationType =>
                    {
                        var one = BigInteger.One.ToString();
                        if (string.IsNullOrWhiteSpace(LeftFractionLowerInput)) LeftFractionLowerInput = one;
                        if (string.IsNullOrWhiteSpace(RightFractionLowerInput)) RightFractionLowerInput = one;

                        switch (operationType)
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

                        _eventService.Send(new OperationPerformedEvent(string.Join(Resources.EQUATION_SPACER, LeftFraction, Operations[operationType.Value], RightFraction, Resources.EQUALITY_SIGN, Result)));
                    }, () => IsFree = true);
                }
                return _calculateCommand;
            }
        }

        /// <summary>
        /// Field for <see cref="CalculateComm"/>.
        /// </summary>
        private ICommand _calculateCommand;

        /// <summary>
        /// Indicates whether there is an ongoing (<see langword="false"/>) operation.
        /// </summary>
        public bool IsFree
        {
            get => _isFree;
            set
            {
                SetProperty(ref _isFree, value);
                _commandingService.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Field for <see cref="IsFree"/>.
        /// </summary>
        private bool _isFree = true;

        public void Receive(OperationPerformedEvent value) => _viewService.Execute(() => History.Insert(0, value.Data));

        public FractionUserControlViewModel(IEventService eventService, ICommandingService commandingService, IViewService viewService)
        {
            _eventService = eventService;
            _commandingService = commandingService;
            _viewService = viewService;

            _eventService.Subscribe<FractionUserControlViewModel, OperationPerformedEvent>(this);
        }

    }
}
