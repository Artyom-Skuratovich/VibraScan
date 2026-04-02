namespace VibraScan.WinForms.Views.Controls
{
    partial class ImportResultItemControl
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
            tableLayoutPanel1 = new TableLayoutPanel();
            _lblSummary = new Label();
            _btnDetails = new Button();
            _txtErrorDetails = new TextBox();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.Controls.Add(_lblSummary, 0, 0);
            tableLayoutPanel1.Controls.Add(_btnDetails, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Top;
            tableLayoutPanel1.Location = new Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(400, 40);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // _lblSummary
            // 
            _lblSummary.Dock = DockStyle.Fill;
            _lblSummary.Location = new Point(3, 0);
            _lblSummary.Name = "_lblSummary";
            _lblSummary.Size = new Size(354, 40);
            _lblSummary.TabIndex = 0;
            _lblSummary.Text = "label1";
            _lblSummary.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _btnDetails
            // 
            _btnDetails.Anchor = AnchorStyles.Right;
            _btnDetails.Cursor = Cursors.Hand;
            _btnDetails.FlatStyle = FlatStyle.Flat;
            _btnDetails.Location = new Point(367, 7);
            _btnDetails.Name = "_btnDetails";
            _btnDetails.Size = new Size(30, 25);
            _btnDetails.TabIndex = 1;
            _btnDetails.Text = "▼";
            _btnDetails.UseVisualStyleBackColor = true;
            _btnDetails.Click += ToggleDetails;
            // 
            // _txtErrorDetails
            // 
            _txtErrorDetails.Dock = DockStyle.Fill;
            _txtErrorDetails.Location = new Point(3, 43);
            _txtErrorDetails.Multiline = true;
            _txtErrorDetails.Name = "_txtErrorDetails";
            _txtErrorDetails.ReadOnly = true;
            _txtErrorDetails.ScrollBars = ScrollBars.Vertical;
            _txtErrorDetails.Size = new Size(400, 0);
            _txtErrorDetails.TabIndex = 1;
            _txtErrorDetails.Visible = false;
            // 
            // ImportResultItemControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(_txtErrorDetails);
            Controls.Add(tableLayoutPanel1);
            Name = "ImportResultItemControl";
            Padding = new Padding(3);
            Size = new Size(406, 46);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Label _lblSummary;
        private TextBox _txtErrorDetails;
        private Button _btnDetails;
    }
}
