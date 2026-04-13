using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Globalization;
using System.Windows.Data;

namespace VibraScan.Presentation.Resources.Converters
{
    public class ValuesToSeriesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float[] values)
            {
                return new ISeries[]
                {
                    new LineSeries<float>
                    {
                        Values = values,
                        Fill = null,
                        GeometryFill = null,
                        GeometryStroke = null,
                        Stroke = new SolidColorPaint(SKColors.Blue, 2),
                        LineSmoothness = 0
                    }
                };
            }

            return Array.Empty<ISeries>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}