using ClosedXML.Excel;
using VibraScan.Application.Reports.Commands;
using VibraScan.Infrastructure.Reports.Charts;

namespace VibraScan.Infrastructure.Reports
{
    internal class ExcelReportStorageService(IReportChartGenerator chartGenerator) : IReportStorageService
    {
        private const int TableStartRow = 5;
        private readonly IReportChartGenerator _chartGenerator = chartGenerator;

        public async Task SaveReportAsync(ReportData data, string destinationPath, CancellationToken ct = default)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add();

            FillMetadata(worksheet, data);
            var endOfTableRow = FillDataGrid(worksheet, data);

            var chartBytes = _chartGenerator.GenerateTrendChart(data.MeasurementProfileName, data.Columns, data.Rows);
            InsertChart(worksheet, chartBytes, endOfTableRow + 2);

            await SaveToFile(workbook, destinationPath, ct);
        }

        private static void FillMetadata(IXLWorksheet worksheet, ReportData data)
        {
            StyleMetadataCell(worksheet.Cell("A1"), data.EngineName.ToUpperInvariant(), 14, true);
            StyleMetadataCell(worksheet.Cell("A2"), $"Параметр измерения: {data.MeasurementProfileName}", 11, isItalic: true);
            StyleMetadataCell(worksheet.Cell("A3"), $"Ось оизмерения: {data.AxisType}", 11, isItalic: true);
        }

        private static int FillDataGrid(IXLWorksheet worksheet, ReportData data)
        {
            var dateHeader = worksheet.Cell(TableStartRow, 1);
            dateHeader.Value = "Дата";
            StyleHeaderCell(dateHeader);

            for (int i = 0; i < data.Columns.Count; i++)
            {
                var cell = worksheet.Cell(TableStartRow, i + 2);
                cell.Value = $"Опора {data.Columns[i]}";
                StyleHeaderCell(cell);
            }

            var currentRow = TableStartRow + 1;

            foreach (var row in data.Rows)
            {
                var dateCell = worksheet.Cell(currentRow, 1);

                dateCell.Value = row.Date;
                dateCell.Style.DateFormat.Format = "dd.MM.yyyy";
                StyleHeaderCell(dateCell);

                for (int i = 0; i < data.Columns.Count; i++)
                {
                    var valueCell = worksheet.Cell(currentRow, i + 2);
                    ApplyBaseCentering(valueCell);

                    if (row.Values[i].HasValue)
                    {
                        valueCell.Value = row.Values[i]!.Value;
                        valueCell.Style.NumberFormat.Format = "0.00";
                    }
                    else
                    {
                        valueCell.Value = "-";
                    }
                }

                currentRow++;
            }

            worksheet.Columns(1, data.Columns.Count + 1).AdjustToContents();

            var tableRange = worksheet.Range(TableStartRow, 1, currentRow - 1, data.Columns.Count + 1);
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            return currentRow - 1;
        }

        private static void InsertChart(IXLWorksheet worksheet, byte[] chartBytes, int rowPosition)
        {
            using var ms = new MemoryStream(chartBytes);
            worksheet.AddPicture(ms, "TrendChart").MoveTo(worksheet.Cell(rowPosition, 1));
        }

        private static async Task SaveToFile(XLWorkbook workbook, string destinationPath, CancellationToken ct)
        {
            EnsureDirectoryExists(destinationPath);

            await Task.Run(() =>
            {
                ct.ThrowIfCancellationRequested();

                using var fs = new FileStream(
                    destinationPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 4096,
                    useAsync: false);

                workbook.SaveAs(fs);
            }, ct);
        }

        private static void EnsureDirectoryExists(string destinationPath)
        {
            var directory = Path.GetDirectoryName(destinationPath);

            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static void StyleMetadataCell(IXLCell cell, string value, int fontSize, bool isBold = false, bool isItalic = false)
        {
            cell.Value = value;
            cell.Style.Font.FontSize = fontSize;
            cell.Style.Font.Bold = isBold;
            cell.Style.Font.Italic = isItalic;
        }

        private static void StyleHeaderCell(IXLCell cell)
        {
            cell.Style.Font.Bold = true;
            ApplyBaseCentering(cell);
        }

        private static void ApplyBaseCentering(IXLCell cell)
        {
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }
    }
}