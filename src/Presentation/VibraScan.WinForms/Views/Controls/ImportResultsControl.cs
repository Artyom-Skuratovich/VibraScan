using VibraScan.Application.DataImport;
using VibraScan.Application.DataImport.Commands.StartBulkImport;

namespace VibraScan.WinForms.Views.Controls
{
    public partial class ImportResultsControl : UserControl
    {
        public ImportResultsControl(object result)
        {
            InitializeComponent();
            SetupHeaderAndList(result);
        }

        private void AddRow(ImportResult result, bool startExpanded = false)
        {
            var rowIndex = _tlpResults.RowCount;
            _tlpResults.RowCount++;

            _tlpResults.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var row = new ImportResultItemControl(result, startExpanded)
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Margin = new Padding(0, 0, 0, 5)
            };

            _tlpResults.Controls.Add(row, 0, rowIndex);
        }

        private void SetupHeaderAndList(object result)
        {
            _tlpResults.SuspendLayout();

            _tlpResults.Controls.Clear();
            _tlpResults.RowStyles.Clear();
            _tlpResults.RowCount = 0;

            if (result is BulkImportResult bulk)
            {
                var errorCount = bulk.Details.Count(x => !x.IsSuccess);

                _lblTotalSummary.Text = $"Импорт завершён. Записей обработано: {bulk.TotalProcessed} | Не удалсось обработать файлов: {errorCount}";

                foreach (var detail in bulk.Details)
                {
                    AddRow(detail);
                }
            }
            else if (result is ImportResult single)
            {
                _lblTotalSummary.Text = single.IsSuccess ? "Файл успешно импортирован" : "Ошибка при импорте файла";
                AddRow(single, !single.IsSuccess);
            }

            _tlpResults.ResumeLayout(true);
        }
    }
}