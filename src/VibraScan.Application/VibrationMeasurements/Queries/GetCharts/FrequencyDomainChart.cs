using VibraScan.Domain.ValueObjects;

namespace VibraScan.Application.VibrationMeasurements.Queries.GetCharts
{
    public record FrequencyDomainChart(float Rms, IReadOnlyList<float> Values, float AmplitudeRange)
        : SingleMeasurementChart(MeasurementDomain.Frequency.Name, Rms, Values);
}