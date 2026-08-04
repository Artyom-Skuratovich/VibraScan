namespace VibraScan.Application.VibrationMeasurements.Queries.GetCharts
{
    public record ChartBundle<T>(
        T MeasurementChart,
        RmsHistoryChart RmsHistoryChart) where T : SingleMeasurementChart;
}