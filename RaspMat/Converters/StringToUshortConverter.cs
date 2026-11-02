using System;
using System.Globalization;
using System.Windows.Data;

namespace RaspMat.Converters
{
    /// <summary>
    /// Convertion between <see cref="string"/> and <see cref="ushort"/> implementing <see cref="IValueConverter"/>.
    /// </summary>
    internal class StringToUshortConverter : IValueConverter
    {

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ushort.TryParse(value?.ToString(), out var parsed) ? parsed : 0;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? string.Empty;
        }

    }
}
