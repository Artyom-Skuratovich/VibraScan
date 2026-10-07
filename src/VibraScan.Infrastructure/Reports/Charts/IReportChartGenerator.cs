using VibraScan.Application.Reports.Commands;

namespace VibraScan.Infrastructure.Reports.Charts
{
    internal interface IReportChartGenerator
    {
        byte[] GenerateTrendChart(
            string title,
            List<string> columnNames,
            List<ReportRow> dataRows,
            int width = 850,
            int height = 450);
    }
}