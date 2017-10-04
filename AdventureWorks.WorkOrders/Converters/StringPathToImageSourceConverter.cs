using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AdventureWorks.WorkOrders.Converters
{
    [ValueConversion(typeof(string), typeof(ImageSource))]
    public class StringPathToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                Uri source = new Uri((string)value, UriKind.RelativeOrAbsolute);
                return new BitmapImage(source);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert back not supported.
            return value;
        }
    }
}
