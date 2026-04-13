using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VibraScan.Presentation.Resources.Converters
{
    public class TypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is not null) && (parameter is Type target))
            {
                return target.IsAssignableFrom(value.GetType()) ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}