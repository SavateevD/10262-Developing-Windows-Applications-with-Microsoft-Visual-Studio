using System;
using System.Globalization;
using System.Windows.Data;

namespace AdventureWorks.WorkOrders.Converters
{
    /// <summary>
    /// Provides a value converter that inverts the supplied boolean value.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool sourceValue = (bool)value;
            return !sourceValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool sourceValue = (bool)value;
            return !sourceValue;
        }
    }
}
