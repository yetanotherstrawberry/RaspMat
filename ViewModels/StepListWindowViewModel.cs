using Prism.Mvvm;
using RaspMat.Interfaces;
using RaspMat.Models;
using System.Collections.Generic;

namespace RaspMat.ViewModels
{
    /// <summary>
    /// ViewModel for the list of <see cref="IAlgorithmResult{T}"/> Where <c>T</c> is <see cref="Matrix"/>.
    /// </summary>
    internal class StepListWindowViewModel : BindableBase
    {

        public IList<IAlgorithmResult<Matrix>> Steps { get; private set; }



    }
}
