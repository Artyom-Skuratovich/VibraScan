namespace VibraScan.Application.Reports.Commands
{
    public record ReportRow(
        DateTime Date,
        List<float?> Values);
}