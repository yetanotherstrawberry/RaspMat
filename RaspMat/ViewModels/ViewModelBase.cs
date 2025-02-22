using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;

namespace RaspMat.ViewModels
{
    /// <summary>
    /// Base class for ViewModels.
    /// </summary>
    internal abstract class ViewModelBase : ObservableObject
    {

        /// <summary>
        /// Error handler for <see cref="IObserver{T}.OnError(Exception)"/>. This method <see langword="throw"/>s the <paramref name="error"/>.
        /// </summary>
        /// <param name="error"><see cref="Exception"/> that represents the error.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnError(Exception error) => throw error;

        /// <summary>
        /// Handler for <see cref="IObserver{T}.OnCompleted"/>. This method does nothing.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnCompleted() { }

    }
}
