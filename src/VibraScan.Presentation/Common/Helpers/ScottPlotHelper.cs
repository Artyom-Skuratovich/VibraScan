using ScottPlot;
using ScottPlot.WPF;
using System.Windows;

namespace VibraScan.Presentation.Common.Helpers
{
    public static class ScottPlotHelper
    {
        public static readonly DependencyProperty PlotDataProperty =
            DependencyProperty.RegisterAttached(
                "PlotData",
                typeof(IReadOnlyList<float>),
                typeof(ScottPlotHelper),
                new PropertyMetadata(null, OnPlotDataChanged));

        public static IReadOnlyList<float> GetPlotData(DependencyObject obj) => (IReadOnlyList<float>)obj.GetValue(PlotDataProperty);
        public static void SetPlotData(DependencyObject obj, IReadOnlyList<float> value) => obj.SetValue(PlotDataProperty, value);

        private static void OnPlotDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WpfPlot wpfPlot)
            {
                wpfPlot.Plot.Clear();

                if (e.NewValue is IReadOnlyList<float> data && data.Count > 0)
                {
                    var signal = wpfPlot.Plot.Add.Signal(data);

                    signal.Color = Color.FromHex("#2563EB");
                    signal.LineWidth = 1;

                    wpfPlot.Plot.Axes.Margins(0, 0.1);
                }

                wpfPlot.Refresh();
            }
        }
    }
}