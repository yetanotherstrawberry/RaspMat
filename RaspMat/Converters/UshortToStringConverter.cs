using System;
using System.Globalization;
using System.Windows.Data;

namespace RaspMat.Converters
{
    /// <summary>
    /// Convertion between <see cref="string"/> and <see cref="ushort"/> implementing <see cref="IValueConverter"/>.
    /// </summary>
    internal class UshortToStringConverter : IValueConverter
    {

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as ushort?)?.ToString(culture) ?? string.Empty;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ushort.TryParse(value?.ToString(), NumberStyles.Integer & ~NumberStyles.AllowLeadingSign, culture, out var parsed) ? parsed : 0;
        }

    }
}
