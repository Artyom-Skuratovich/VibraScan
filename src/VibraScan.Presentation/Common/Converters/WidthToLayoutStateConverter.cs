using System.Globalization;
using System.Windows.Data;

namespace VibraScan.Presentation.Common.Converters
{
    public class WidthToLayoutStateConverter : IValueConverter
    {
        private const double DefaultThresholdWidth = 900;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double currentWidth && currentWidth > 0)
            {
                var thresholdWidth = DefaultThresholdWidth;

                if (parameter is string paramString && double.TryParse(paramString, out var parsedValue))
                {
                    thresholdWidth = parsedValue;
                }

                return currentWidth < thresholdWidth;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}