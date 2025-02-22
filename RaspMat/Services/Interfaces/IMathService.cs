namespace RaspMat.Services.Interfaces
{
    /// <summary>
    /// A service for computing equations.
    /// </summary>
    internal interface IMathService
    {

        /// <summary>
        /// Parses the <paramref name="equation"/>.
        /// </summary>
        /// <typeparam name="TResult">Cast of the result.</typeparam>
        /// <param name="equation">A <see cref="string"/> representation of the equation, like "1+1".</param>
        /// <returns>Result of the <paramref name="equation"/> casted to <typeparamref name="TResult"/>.</returns>
        TResult Compute<TResult>(string equation);

    }
}
