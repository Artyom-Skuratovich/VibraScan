using System.Globalization;
using System.Windows.Data;

namespace VibraScan.Presentation.Common.Converters
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Enum || parameter is not Enum)
            {
                return false;
            }

            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is Enum && value is bool isChecked && isChecked)
            {
                return parameter;
            }

            return Binding.DoNothing;
        }
    }
}