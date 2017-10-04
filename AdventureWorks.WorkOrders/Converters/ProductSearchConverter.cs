using System;
using System.Globalization;
using System.Windows.Data;

namespace AdventureWorks.WorkOrders.Converters
{
    [ValueConversion(typeof(string), typeof(int))]
    class ProductSearchConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == values)
            {
                return false;
            }

            return string.Format("{0}:{1}", values[0], values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

