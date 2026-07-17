namespace VibraScan.Application.VibrationMeasurements.Queries.GetCharts
{
    public record FrequencyDomainChart(string MeasurementDomain, float Rms, IReadOnlyList<float> Values, float AmplitudeRange)
        : BaseChart(MeasurementDomain, Rms, Values);
}