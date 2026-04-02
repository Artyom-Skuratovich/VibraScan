using VibraScan.Application.DataImport;
using VibraScan.Application.DataImport.Commands.StartBulkImport;
using VibraScan.WinForms.Views.Abstractions;
using VibraScan.WinForms.Views.Controls;
using VibraScan.WinForms.Views.Forms.Base;

namespace VibraScan.WinForms.Views.Forms
{
    public partial class DataImportForm : BaseView, IDataImportView
    {
        private ImportProgressControl? _progressControl;

        public event EventHandler? CancelRequested;

        public DataImportForm()
        {
            InitializeComponent();
        }

        public void PrepareForImport(int totalFiles)
        {
            InvokeIfNeeded(() =>
            {
                _progressControl = new ImportProgressControl();
                _progressControl.Init(totalFiles);
                _progressControl.CancelRequested += (s, e) => CancelRequested?.Invoke(this, EventArgs.Empty);

                SwitchView(_progressControl);
            });
        }

        public void SetCancelable(bool canCancel)
        {
            InvokeIfNeeded(() =>
            {
                _progressControl?.SetCancelEnabled(canCancel);
            });
        }

        public void ShowResults(ImportResult result)
        {
            InvokeIfNeeded(() =>
            {
                var view = new ImportResultsControl(result);
                SwitchView(view);
            });
        }

        public void ShowResults(BulkImportResult result)
        {
            InvokeIfNeeded(() =>
            {
                var view = new ImportResultsControl(result);
                SwitchView(view);
            });
        }

        public void UpdateFileProgress(double percentage, string stage)
        {
            InvokeIfNeeded(() =>
            {
                _progressControl?.UpdateFileProgress(percentage, stage);
            });
        }

        public void UpdateOverallProgress(int processedCount, string description)
        {
            InvokeIfNeeded(() =>
            {
                _progressControl?.UpdateOverallProgress(processedCount, description);
            });
        }

        private void SwitchView(UserControl control)
        {
            _mainPanel.Controls.Clear();

            control.AutoSize = true;
            _mainPanel.Controls.Add(control);

            PerformLayout();

            Size = PreferredSize;
        }
    }
}