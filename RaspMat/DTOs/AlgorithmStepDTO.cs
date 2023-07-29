namespace RaspMat.DTOs
{
    /// <summary>
    /// Stores a text and <typeparamref name="TResult"/> returned by an algorithm.
    /// </summary>
    internal class AlgorithmStepDTO<TResult>
    {

        /// <summary>
        /// Gets the text describing a single step of the algorithm.
        /// </summary>
        public string Step { get; }

        /// <summary>
        /// Gets the result of performing described step.
        /// </summary>
        public TResult Result { get; }

        /// <summary>
        /// Creates a class that holds information about a step of an algorithm and its result.
        /// </summary>
        /// <param name="step">Text describing a single step of the algorithm.</param>
        /// <param name="result">Result of performing the described step.</param>
        public AlgorithmStepDTO(string step, TResult result)
        {
            Step = step;
            Result = result;
        }

    }
}
