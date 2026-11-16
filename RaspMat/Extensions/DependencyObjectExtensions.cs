using System.ComponentModel;
using System.Windows;

namespace RaspMat.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="DependencyObject"/> <see langword="class"/>.
    /// </summary>
    internal static class DependencyObjectExtensions
    {

        /// <summary>
        /// Determines whether the specified <paramref name="dependencyObject"/> is not in design mode.
        /// </summary>
        /// <typeparam name="TDependencyObject">A <see cref="DependencyObject"/>.</typeparam>
        /// <param name="dependencyObject">The <see cref="DependencyObject"/> to check.</param>
        /// <returns>If the <paramref name="dependencyObject"/> is not in design mode, a <see langword="true"/> will be <see langword="return"/>ed.</returns>
        public static bool IsNotInDesign<TDependencyObject>(this TDependencyObject dependencyObject) where TDependencyObject : DependencyObject
        {
            return !DesignerProperties.GetIsInDesignMode(dependencyObject);
        }

    }
}
