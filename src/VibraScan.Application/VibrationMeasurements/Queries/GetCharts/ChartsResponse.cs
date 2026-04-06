namespace VibraScan.Application.VibrationMeasurements.Queries.GetCharts
{
    public record ChartsResponse(
        TimeDomainChart? TimeDomainChart,
        FrequencyDomainChart? FrequencyDomainChart);
}