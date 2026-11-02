using RaspMat.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RaspMat.Converters
{
    /// <summary>
    /// Convertion between <see cref="string"/> and <see cref="Fraction"/> implementing <see cref="IValueConverter"/>.
    /// </summary>
    internal class StringToFractionConverter : IValueConverter
    {

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value?.ToString();
            return string.IsNullOrWhiteSpace(stringValue) ? default : Fraction.Parse(stringValue);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? string.Empty;
        }

    }
}
