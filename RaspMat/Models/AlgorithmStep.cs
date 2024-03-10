namespace RaspMat.Models
{
    /// <summary>
    /// Stores a text and <typeparamref name="TResult"/> returned by an algorithm.
    /// </summary>
    internal readonly struct AlgorithmStep<TResult>
    {

        /// <summary>
        /// The text describing a single step of the algorithm.
        /// </summary>
        public string Step { get; }

        /// <summary>
        /// The result of performing the described step.
        /// </summary>
        public TResult Result { get; }

        /// <summary>
        /// Holds information about a step of an algorithm and its result.
        /// </summary>
        /// <param name="step">Text describing a single step of the algorithm.</param>
        /// <param name="result">Result of performing the described step.</param>
        public AlgorithmStep(string step, TResult result)
        {
            Step = step;
            Result = result;
        }

    }
}
