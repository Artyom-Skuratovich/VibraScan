namespace VibraScan.Application.VibrationMeasurements.Queries.GetCharts
{
    public abstract record SingleMeasurementChart(string Title, float Rms, IReadOnlyList<float> Values)
        : BaseChart<float>(Title, Values);
}