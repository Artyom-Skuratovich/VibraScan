namespace VibraScan.Application.VibrationMeasurements.Queries.GetCharts
{
    public abstract record BaseChart(string MeasurementDomain, float Rms, IReadOnlyList<float> Values);
}