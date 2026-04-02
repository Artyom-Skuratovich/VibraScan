using System.Text;
using VibraScan.Application.DataImport;

namespace VibraScan.WinForms.Views.Controls
{
    public partial class ImportResultItemControl : UserControl
    {
        private const int CollapsedHeight = 46;
        private const int ExpandedHeight = 156;

        public ImportResultItemControl(ImportResult result, bool startExpanded = false)
        {
            InitializeComponent();
            SetupView(result);

            if (startExpanded && !result.IsSuccess && (result.Error != null))
            {
                ToggleDetails(this, EventArgs.Empty);
            }
        }

        private void SetupView(ImportResult result)
        {
            Height = CollapsedHeight;
            _txtErrorDetails.Visible = false;

            _lblSummary.Text = $"[{result.SourceName}] {result.Message} (Обработано: {result.ProcessedEntitiesCount})";
            _txtErrorDetails.Text = "";

            if (!result.IsSuccess && result.Error != null)
            {
                BackColor = Color.MistyRose;
                _btnDetails.Visible = true;
                _lblSummary.Cursor = Cursors.Hand;
                _lblSummary.Click += ToggleDetails!;

                var errorInfo = new StringBuilder();
                errorInfo.AppendLine($"СООБЩЕНИЕ: {result.Error.Message}");

                if (!string.IsNullOrWhiteSpace(result.Error.StackTrace))
                {
                    errorInfo.AppendLine().AppendLine("СТЕК ВЫЗОВОВ:").AppendLine(result.Error.StackTrace);
                }

                _txtErrorDetails.Text = errorInfo.ToString();
            }
            else
            {
                BackColor = Color.Honeydew;
                _btnDetails.Visible = false;
                _txtErrorDetails.Text = string.Empty;

                _lblSummary.Cursor = Cursors.Default;
                _lblSummary.Click -= ToggleDetails!;
            }
        }

        private void ToggleDetails(object sender, EventArgs e)
        {
            _txtErrorDetails.Visible = !_txtErrorDetails.Visible;

            Height = _txtErrorDetails.Visible ? ExpandedHeight : CollapsedHeight;

            _btnDetails.Text = _txtErrorDetails.Visible ? "▲" : "▼";

            FindForm()?.PerformLayout();
        }
    }
}