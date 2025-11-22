using CommunityToolkit.Mvvm.ComponentModel;

namespace RaspMat.ViewModels
{
    /// <summary>
    /// Base class for ViewModels.
    /// </summary>
    internal abstract class ViewModelBase : ObservableObject
    {

        /// <summary>
        /// Field for <see cref="IsFree"/>.
        /// </summary>
        private bool _isFree = true;

        /// <summary>
        /// Indicates whether the UI should be enabled.
        /// </summary>
        public bool IsFree
        {
            get => _isFree;
            protected set => SetProperty(ref _isFree, value);
        }

    }
}
