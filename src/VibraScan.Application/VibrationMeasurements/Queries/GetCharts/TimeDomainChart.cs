namespace VibraScan.Application.VibrationMeasurements.Queries.GetCharts
{
    public record TimeDomainChart(string MeasurementDomain, float Rms, float[] Values)
        : BaseChart(MeasurementDomain, Rms, Values);
}