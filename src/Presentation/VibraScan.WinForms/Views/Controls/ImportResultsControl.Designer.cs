namespace VibraScan.WinForms.Views.Controls
{
    partial class ImportResultsControl
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            _tlpContent = new TableLayoutPanel();
            _lblTotalSummary = new Label();
            _tlpResults = new TableLayoutPanel();
            _tlpContent.SuspendLayout();
            SuspendLayout();
            // 
            // _tlpContent
            // 
            _tlpContent.AutoSize = true;
            _tlpContent.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _tlpContent.ColumnCount = 1;
            _tlpContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _tlpContent.Controls.Add(_lblTotalSummary, 0, 0);
            _tlpContent.Controls.Add(_tlpResults, 0, 1);
            _tlpContent.Dock = DockStyle.Fill;
            _tlpContent.Location = new Point(0, 0);
            _tlpContent.Name = "_tlpContent";
            _tlpContent.RowCount = 2;
            _tlpContent.RowStyles.Add(new RowStyle());
            _tlpContent.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _tlpContent.Size = new Size(58, 41);
            _tlpContent.TabIndex = 0;
            // 
            // _lblTotalSummary
            // 
            _lblTotalSummary.AutoSize = true;
            _lblTotalSummary.Dock = DockStyle.Fill;
            _lblTotalSummary.Location = new Point(10, 10);
            _lblTotalSummary.Margin = new Padding(10);
            _lblTotalSummary.Name = "_lblTotalSummary";
            _lblTotalSummary.Size = new Size(38, 15);
            _lblTotalSummary.TabIndex = 0;
            _lblTotalSummary.Text = "label1";
            // 
            // _tlpResults
            // 
            _tlpResults.AutoSize = true;
            _tlpResults.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _tlpResults.ColumnCount = 1;
            _tlpResults.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _tlpResults.Dock = DockStyle.Top;
            _tlpResults.Location = new Point(3, 38);
            _tlpResults.Name = "_tlpResults";
            _tlpResults.RowCount = 1;
            _tlpResults.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _tlpResults.Size = new Size(52, 0);
            _tlpResults.TabIndex = 1;
            // 
            // ImportResultsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(_tlpContent);
            Name = "ImportResultsControl";
            Size = new Size(58, 41);
            _tlpContent.ResumeLayout(false);
            _tlpContent.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel _tlpContent;
        private Label _lblTotalSummary;
        private TableLayoutPanel _tlpResults;
    }
}
