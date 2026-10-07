using ScottPlot;
using ScottPlot.TickGenerators;
using VibraScan.Application.Reports.Commands;

namespace VibraScan.Infrastructure.Reports.Charts
{
    internal class ScottPlotChartGenerator : IReportChartGenerator
    {
        private const int DefaultMarkerSize = 6;
        private const int RotationAngle = 45;

        public byte[] GenerateTrendChart(string title, List<string> columnNames, List<ReportRow> dataRows, int width = 850, int height = 450)
        {
            using var plot = new Plot();

            ConfigurePlotLayout(plot, title);

            var validRows = dataRows
                .Where(r => r.Values.Any(v => v.HasValue))
                .OrderBy(r => r.Date)
                .ToList();

            DrawTrendLines(plot, columnNames, dataRows);
            ConfigureDateTimeAxis(plot, validRows);

            return plot.GetImageBytes(width, height, ImageFormat.Png);
        }

        private static void ConfigureDateTimeAxis(Plot plot, List<ReportRow> validRows)
        {
            var tickPositions = validRows.Select(r => r.Date.ToOADate()).ToArray();
            var tickLabels = validRows.Select(r => r.Date.ToString("dd.MM.yyyy")).ToArray();

            var manualTicks = new NumericManual();

            for (int i = 0; i < tickPositions.Length; i++)
            {
                manualTicks.Add(new Tick(tickPositions[i], tickLabels[i]));
            }

            plot.Axes.Bottom.TickGenerator = manualTicks;

            plot.Axes.Bottom.TickLabelStyle.Rotation = RotationAngle;
            plot.Axes.Bottom.TickLabelStyle.Alignment = Alignment.UpperLeft;
        }

        private static void ConfigurePlotLayout(Plot plot, string title)
        {
            plot.Title(title);
            plot.YLabel("Rms");
            plot.ShowLegend(Alignment.UpperRight);

            plot.Layout.Fixed(new PixelPadding(60, 30, 70, 40));
        }

        private static void DrawTrendLines(Plot plot, List<string> columnNames, List<ReportRow> dataRows)
        {
            for (int i = 0; i < columnNames.Count; i++)
            {
                var pointData = dataRows
                    .Where(r => r.Values[i].HasValue)
                    .Select(r => new { r.Date, Value = r.Values[i] })
                    .ToList();

                if (pointData.Count > 0)
                {
                    var xs = pointData.Select(p => p.Date.ToOADate()).ToArray();
                    var ys = pointData.Select(p => p.Value!.Value).ToArray();

                    var line = plot.Add.Scatter(xs, ys);
                    line.LegendText = $"Опора {columnNames[i]}";
                    line.MarkerSize = DefaultMarkerSize;
                }
            }
        }
    }
}