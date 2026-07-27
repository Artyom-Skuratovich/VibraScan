using VibraScan.Domain.ValueObjects;

namespace VibraScan.Application.VibrationMeasurements.Queries.GetCharts
{
    public record TimeDomainChart(float Rms, IReadOnlyList<float> Values)
        : SingleMeasurementChart(MeasurementDomain.Time.Name, Rms, Values);
}