using RaspMat.Services.Interfaces;
using System;
using System.Data;

namespace RaspMat.Services
{
    /// <summary>
    /// Implements <see cref="IMathService"/> using a <see cref="DataTable"/>.
    /// </summary>
    internal class DataTableMathService : IMathService, IDisposable
    {

        /// <summary>
        /// <see cref="DataTable"/> used for computations.
        /// </summary>
        private readonly DataTable _dataTab = new DataTable();

        /// <inheritdoc/>
        public TResult Compute<TResult>(string equation)
        {
            if (string.IsNullOrWhiteSpace(equation)) throw new ArgumentNullException(nameof(equation));
            var ret = _dataTab.Compute(equation, null);
            return DBNull.Value.Equals(ret) ? throw new ArithmeticException(nameof(equation)) : (TResult)ret;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _dataTab.Dispose();
        }

    }
}
