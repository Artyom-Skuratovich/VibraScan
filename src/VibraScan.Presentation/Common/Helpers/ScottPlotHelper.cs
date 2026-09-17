using ScottPlot;
using ScottPlot.Colormaps;
using ScottPlot.Panels;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VibraScan.Application.VibrationMeasurements.Queries.GetCharts;

namespace VibraScan.Presentation.Common.Helpers
{
    public static class ScottPlotHelper
    {
        private const float MarkerRadius = 12f;
        private const float ToolTipMaxXDistance = 15f;
        private const float ToolTipMaxYDistance = 20f;

        private static readonly CustomInterpolated TemperatureColormap = new([
            Color.FromHex("#10B981"),
            Color.FromHex("#A7F3D0"),
            Color.FromHex("#FBBF24"),
            Color.FromHex("#F97316"),
            Color.FromHex("#EF4444")
        ]);

        public static readonly DependencyProperty PlotDataProperty =
            DependencyProperty.RegisterAttached(
                "PlotData",
                typeof(IReadOnlyList<float>),
                typeof(ScottPlotHelper),
                new PropertyMetadata(null, OnPlotDataChanged));

        public static IReadOnlyList<float> GetPlotData(DependencyObject obj) => (IReadOnlyList<float>)obj.GetValue(PlotDataProperty);
        public static void SetPlotData(DependencyObject obj, IReadOnlyList<float> value) => obj.SetValue(PlotDataProperty, value);

        public static readonly DependencyProperty PlotHistoryDataProperty =
            DependencyProperty.RegisterAttached(
                "PlotHistoryData",
                typeof(IReadOnlyList<RmsHistoryPoint>),
                typeof(ScottPlotHelper),
                new PropertyMetadata(null, OnPlotHistoryDataChanged));

        public static IReadOnlyList<RmsHistoryPoint> GetPlotHistoryData(DependencyObject obj) => (IReadOnlyList<RmsHistoryPoint>)obj.GetValue(PlotHistoryDataProperty);
        public static void SetPlotHistoryData(DependencyObject obj, IReadOnlyList<RmsHistoryPoint> value) => obj.SetValue(PlotHistoryDataProperty, value);

        public static readonly DependencyProperty MinRmsProperty =
            DependencyProperty.RegisterAttached("MinRms", typeof(float), typeof(ScottPlotHelper), new PropertyMetadata(4.0f));

        public static readonly DependencyProperty MaxRmsProperty =
            DependencyProperty.RegisterAttached("MaxRms", typeof(float), typeof(ScottPlotHelper), new PropertyMetadata(7.0f));

        public static float GetMinRms(DependencyObject obj) => (float)obj.GetValue(MinRmsProperty);
        public static void SetMinRms(DependencyObject obj, float value) => obj.SetValue(MinRmsProperty, value);

        public static float GetMaxRms(DependencyObject obj) => (float)obj.GetValue(MaxRmsProperty);
        public static void SetMaxRms(DependencyObject obj, float value) => obj.SetValue(MaxRmsProperty, value);

        #region PlotData

        private static void OnPlotDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WpfPlot plot)
            {
                EnsureMouseTrackingInitialized(plot, WpfPlotSignalMouseMove);

                plot.Plot.Clear();

                if (e.NewValue is IReadOnlyList<float> data && (data.Count > 0))
                {
                    var signal = plot.Plot.Add.Signal(data);

                    signal.Color = Color.FromHex("#2563EB");
                    signal.LineWidth = 1;

                    plot.Plot.Axes.Margins(0, 0.1);
                }

                plot.Refresh();
            }
        }

        private static void WpfPlotSignalMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not WpfPlot plot) return;

            var data = GetPlotData(plot);

            if ((data == null) || (data.Count == 0))
            {
                HidePlotToolTip(plot);
                return;
            }

            var signal = plot.Plot.GetPlottables<Signal>().FirstOrDefault();
            if (signal == null) return;

            var mousePos = e.GetPosition(plot);
            var mousePixel = new Pixel((float)mousePos.X, (float)mousePos.Y);
            var mouseCoord = plot.Plot.GetCoordinates(mousePixel);

            var xOffset = signal.Data.XOffset;
            var xStep = signal.Data.Period;

            var index = (int)Math.Round((mouseCoord.X - xOffset) / xStep);

            if ((index >= 0) && (index < data.Count))
            {
                var pointPixel = plot.Plot.GetPixel(new Coordinates(index * xStep + xOffset, data[index]));

                if ((Math.Abs(mousePixel.X - pointPixel.X) < ToolTipMaxXDistance) && (Math.Abs(mousePixel.Y - pointPixel.Y) < ToolTipMaxYDistance))
                {
                    ShowPlotToolTip(plot, mousePos, $"y={data[index]} x={index}");
                    return;
                }
            }

            HidePlotToolTip(plot);
        }

        private static void ShowPlotToolTip(WpfPlot plot, Point mousePosition, string text)
        {
            if (plot.ToolTip is not ToolTip tooltip)
            {
                tooltip = new ToolTip
                {
                    Placement = System.Windows.Controls.Primitives.PlacementMode.Relative,
                    PlacementTarget = plot
                };
                plot.ToolTip = tooltip;
            }

            tooltip.Content = text;
            tooltip.HorizontalOffset = mousePosition.X + 15;
            tooltip.VerticalOffset = mousePosition.Y + 15;
            tooltip.IsOpen = true;
        }

        private static void HidePlotToolTip(WpfPlot plot)
        {
            if (plot.ToolTip is ToolTip tooltip)
            {
                tooltip.IsOpen = false;
            }
        }

        #endregion

        #region PlotHistoryData

        private static void OnPlotHistoryDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WpfPlot plot)
            {
                EnsureColorBarInitialized(plot);
                EnsureMouseTrackingInitialized(plot, WpfPlotMouseMove);

                plot.Plot.Clear();

                if (e.NewValue is IReadOnlyList<RmsHistoryPoint> data && (data.Count > 0))
                {
                    DrawHistoryMarkers(plot, data);
                    CreateHoverLines(plot);

                    plot.Plot.Axes.DateTimeTicksBottom();
                    plot.Plot.Axes.Margins(0.1, 0.2);
                }

                plot.Refresh();
            }
        }

        private static void EnsureColorBarInitialized(WpfPlot plot)
        {
            if (plot.Plot.Axes.GetPanels().Any(p => p is ColorBar))
            {
                return;
            }

            var minRms = GetMinRms(plot);
            var maxRms = GetMaxRms(plot);

            var dummy = new double[1, 1];
            var heatmap = plot.Plot.Add.Heatmap(dummy);
            heatmap.Colormap = TemperatureColormap;
            heatmap.ManualRange = new ScottPlot.Range(minRms, maxRms);
            heatmap.IsVisible = false;

            var colorbar = plot.Plot.Add.ColorBar(heatmap);
            colorbar.Edge = Edge.Right;
        }


        private static void DrawHistoryMarkers(WpfPlot plot, IReadOnlyList<RmsHistoryPoint> data)
        {
            var minRms = GetMinRms(plot);
            var maxRms = GetMaxRms(plot);

            foreach (var point in data)
            {
                var x = point.Timestamp.ToOADate();
                var y = point.Rms;

                double fraction = (maxRms - minRms) > 0 ? (y - minRms) / (maxRms - minRms) : 0;
                fraction = Math.Clamp(fraction, 0.0, 1.0);
                var pointColor = TemperatureColormap.GetColor(fraction);

                var marker = plot.Plot.Add.Marker(x, y);
                marker.Color = pointColor;
                marker.Size = MarkerRadius * 2;
                marker.Shape = MarkerShape.FilledCircle;

                marker.MarkerStyle.OutlineColor = Color.FromHex("#FFFFFF");
                marker.MarkerStyle.OutlineWidth = 1;
            }
        }

        private static void CreateHoverLines(WpfPlot plot)
        {
            var crosshair = plot.Plot.Add.Crosshair(0, 0);

            crosshair.IsVisible = false;
            crosshair.LineColor = Color.FromHex("#64748B");
            crosshair.LineWidth = 1;
            crosshair.LinePattern = LinePattern.Dashed;

            ConfigureLabelStyles(crosshair.HorizontalLine.LabelStyle);
            ConfigureLabelStyles(crosshair.VerticalLine.LabelStyle);
        }

        private static void ConfigureLabelStyles(LabelStyle label)
        {
            label.ForeColor = Colors.White;
            label.BackgroundColor = Color.FromHex("#64748B");
            label.Bold = true;
            label.Padding = 6;
        }

        private static void UpdateHoverElements(WpfPlot plot, Crosshair crosshair, RmsHistoryPoint? point)
        {
            if (point is null) return;

            var exactX = point.Timestamp.ToOADate();
            var exactY = point.Rms;

            crosshair.Position = new Coordinates(exactX, exactY);

            crosshair.HorizontalLine.LabelText = $"{point.Rms}";
            crosshair.VerticalLine.LabelText = point.Timestamp.ToString("dd.MM.yyyy HH:mm:ss");

            var dataRect = plot.Plot.RenderManager.LastRender.DataRect;
            var pointPixel = plot.Plot.GetPixel(crosshair.Position);

            var xCenter = dataRect.Left + (dataRect.Width / 2);
            var yCenter = dataRect.Top + (dataRect.Height / 2);

            crosshair.VerticalLine.ManualLabelAlignment = (pointPixel.X < xCenter) ? Alignment.UpperLeft : Alignment.UpperRight;
            crosshair.HorizontalLine.ManualLabelAlignment = (pointPixel.Y < yCenter) ? Alignment.LowerRight : Alignment.LowerLeft;

            crosshair.IsVisible = true;
            plot.Refresh();
        }

        private static void WpfPlotMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not WpfPlot plot) return;

            var data = GetPlotHistoryData(plot);
            if ((data == null) || (data.Count == 0)) return;

            var crosshair = plot.Plot.GetPlottables<Crosshair>().FirstOrDefault();
            if (crosshair == null) return;

            var mousePosition = e.GetPosition(plot);
            var mousePixel = new Pixel(mousePosition.X, mousePosition.Y);

            if (TryGetPointUnderMouse(plot, data, mousePixel, out var closestPoint))
            {
                UpdateHoverElements(plot, crosshair, closestPoint);
                return;
            }

            if (crosshair.IsVisible)
            {
                crosshair.IsVisible = false;
                plot.Refresh();
            }
        }

        private static bool TryGetPointUnderMouse(WpfPlot plot, IReadOnlyList<RmsHistoryPoint> data, Pixel mousePixel, out RmsHistoryPoint? closestPoint)
        {
            closestPoint = null;
            var minPixelDistance = double.MaxValue;

            foreach (var point in data)
            {
                var pointX = point.Timestamp.ToOADate();
                var pointY = point.Rms;
                var pointPixel = plot.Plot.GetPixel(new Coordinates(pointX, pointY));

                var distance = pointPixel.DistanceFrom(mousePixel);

                if (distance < minPixelDistance)
                {
                    minPixelDistance = distance;
                    closestPoint = point;
                }
            }

            return (closestPoint is not null) && (minPixelDistance <= MarkerRadius);
        }

        #endregion

        private static void EnsureMouseTrackingInitialized(WpfPlot plot, MouseEventHandler moveHandler)
        {
            plot.MouseMove -= moveHandler;
            plot.MouseMove += moveHandler;
        }
    }
}