using VibraScan.Application.DataImport;
using VibraScan.Application.DataImport.Commands.StartBulkImport;
using VibraScan.WinForms.Views.Abstractions;
using VibraScan.WinForms.Views.Forms.Base;

namespace VibraScan.WinForms.Views.Forms
{
    public partial class DataImportForm : BaseView, IDataImportView
    {
        private int _totalFiles;

        public DataImportForm()
        {
            InitializeComponent();

            btnCancel.Click += (s, e) => CancelRequested?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? CancelRequested;

        public void PrepareForImport(int totalFiles)
        {
            InvokeIfNeeded(() =>
            {
                _totalFiles = totalFiles;
                var isBulk = _totalFiles > 1;

                if (!isBulk)
                {
                    tlpLoading.RowStyles[0] = new RowStyle(SizeType.Absolute, 0);
                }
                else
                {
                    pbOverall.Maximum = totalFiles;
                    pbOverall.Value = 0;
                }

                tlpLoading.PerformLayout();
            });
        }

        public void SetCancelable(bool canCancel)
        {
            InvokeIfNeeded(() =>
            {
                btnCancel.Enabled = canCancel;
            });
        }

        public void ShowResults(ImportResult result)
        {
            throw new NotImplementedException();
        }

        public void ShowResults(BulkImportResult result)
        {
            throw new NotImplementedException();
        }

        public void UpdateFileProgress(double percentage, string stage)
        {
            InvokeIfNeeded(() =>
            {
                lblSegment.Text = stage;
                pbSegment.Value = (int)Math.Clamp(percentage, 0, 100);
            });
        }

        public void UpdateOverallProgress(int processedCount, string description)
        {
            if (_totalFiles < 2)
            {
                return;
            }

            InvokeIfNeeded(() =>
            {
                lblOverall.Text = description;
                pbOverall.Value = processedCount;
            });
        }
    }
}