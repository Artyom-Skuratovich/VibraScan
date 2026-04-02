namespace VibraScan.WinForms.Views.Controls
{
    public partial class ImportProgressControl : UserControl
    {
        public event EventHandler? CancelRequested;

        public ImportProgressControl()
        {
            InitializeComponent();
            _btnCancel.Click += (s, e) => CancelRequested?.Invoke(this, EventArgs.Empty);
        }

        public void Init(int totalFiles)
        {
            var isBulk = totalFiles > 1;
            _tlpContainer.RowStyles[0] = new RowStyle(SizeType.Percent, isBulk ? 33.33f : 0f);

            if (isBulk)
            {
                _pbOverall.Maximum = totalFiles;
                _pbOverall.Value = 0;
            }
        }

        public void UpdateFileProgress(double percentage, string stage)
        {
            _lblSegment.Text = stage;
            _pbSegment.Value = (int)Math.Clamp(percentage, 0, 100);
        }

        public void UpdateOverallProgress(int processedCount, string description)
        {
            _lblOverall.Text = description;
            _pbOverall.Value = processedCount;
        }

        public void SetCancelEnabled(bool enabled)
        {
            _btnCancel.Enabled = enabled;
        }
    }
}