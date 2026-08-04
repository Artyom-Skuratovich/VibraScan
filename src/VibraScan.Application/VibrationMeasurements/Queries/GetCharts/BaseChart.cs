namespace VibraScan.Application.VibrationMeasurements.Queries.GetCharts
{
    public abstract record BaseChart<T>(string Title, IReadOnlyList<T> Values);
}