namespace RaspMat.Services.Interfaces
{
    /// <summary>
    /// Contains logic for interacting with view for steps of an algorithm.
    /// </summary>
    internal interface IStepViewService
    {

        /// <summary>
        /// Shows or closes a view with steps performed by an algorithm.
        /// </summary>
        void Toggle();

    }
}
