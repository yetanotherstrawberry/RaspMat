using RaspMat.Interfaces;

namespace RaspMat.Models
{
    /// <summary>
    /// Stores a text and <see cref="Matrix"/> returned by an algorithm.
    /// </summary>
    internal class MatAlgorithmResult : IAlgorithmResult<Matrix>
    {

        /// <summary>
        /// Gets the text describing a single step of the algorithm.
        /// </summary>
        public string Step { get; }

        /// <summary>
        /// Gets the <see cref="Matrix"/> after performing described step.
        /// </summary>
        public Matrix Result { get; }

        /// <summary>
        /// Creates a class that holds information about a step of an algorithm that operates on a <see cref="Matrix"/> with a text description.
        /// </summary>
        /// <param name="step">Text describing a single step of the algorithm.</param>
        /// <param name="matrix"><see cref="Matrix"/> after performing the described step.</param>
        public MatAlgorithmResult(string step, Matrix matrix)
        {
            Step = step;
            Result = matrix;
        }

    }
}
