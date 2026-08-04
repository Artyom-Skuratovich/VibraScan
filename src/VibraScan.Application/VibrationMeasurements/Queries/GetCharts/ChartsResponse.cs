namespace VibraScan.Application.VibrationMeasurements.Queries.GetCharts
{
    public record ChartsResponse(
        ChartBundle<TimeDomainChart>? TimeDomain,
        ChartBundle<FrequencyDomainChart>? FrequencyDomain);
}