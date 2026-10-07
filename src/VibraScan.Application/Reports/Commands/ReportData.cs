using VibraScan.Domain.ValueObjects;

namespace VibraScan.Application.Reports.Commands
{
    public record ReportData(
        string EngineName,
        string MeasurementProfileName,
        AxisType AxisType,
        List<string> Columns,
        List<ReportRow> Rows);
}