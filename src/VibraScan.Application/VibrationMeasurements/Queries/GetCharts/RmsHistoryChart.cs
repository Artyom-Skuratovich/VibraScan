namespace VibraScan.Application.VibrationMeasurements.Queries.GetCharts
{
    public record RmsHistoryChart(IReadOnlyList<RmsHistoryPoint> Values, string Title = "История изменений RMS")
        : BaseChart<RmsHistoryPoint>(Title, Values);
}