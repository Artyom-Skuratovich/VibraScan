namespace VibraScan.WinForms.Views.Controls
{
    partial class ImportProgressControl
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
            _tlpContainer = new TableLayoutPanel();
            flowLayoutPanel2 = new FlowLayoutPanel();
            tableLayoutPanel3 = new TableLayoutPanel();
            label3 = new Label();
            _lblSegment = new Label();
            _pbSegment = new ProgressBar();
            flowLayoutPanel1 = new FlowLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            label1 = new Label();
            _lblOverall = new Label();
            _pbOverall = new ProgressBar();
            _btnCancel = new Button();
            _tlpContainer.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // _tlpContainer
            // 
            _tlpContainer.AutoSize = true;
            _tlpContainer.ColumnCount = 1;
            _tlpContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _tlpContainer.Controls.Add(flowLayoutPanel2, 0, 1);
            _tlpContainer.Controls.Add(flowLayoutPanel1, 0, 0);
            _tlpContainer.Controls.Add(_btnCancel, 0, 2);
            _tlpContainer.Dock = DockStyle.Fill;
            _tlpContainer.Location = new Point(0, 0);
            _tlpContainer.Name = "_tlpContainer";
            _tlpContainer.Padding = new Padding(0, 3, 0, 0);
            _tlpContainer.RowCount = 3;
            _tlpContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            _tlpContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            _tlpContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            _tlpContainer.Size = new Size(412, 171);
            _tlpContainer.TabIndex = 1;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Anchor = AnchorStyles.Bottom;
            flowLayoutPanel2.AutoSize = true;
            flowLayoutPanel2.Controls.Add(tableLayoutPanel3);
            flowLayoutPanel2.Controls.Add(_pbSegment);
            flowLayoutPanel2.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel2.Location = new Point(3, 62);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(406, 50);
            flowLayoutPanel2.TabIndex = 1;
            flowLayoutPanel2.WrapContents = false;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.AutoSize = true;
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Controls.Add(label3, 0, 0);
            tableLayoutPanel3.Controls.Add(_lblSegment, 1, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(400, 15);
            tableLayoutPanel3.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 0);
            label3.Name = "label3";
            label3.Size = new Size(69, 15);
            label3.TabIndex = 0;
            label3.Text = "Состояние:";
            // 
            // _lblSegment
            // 
            _lblSegment.AutoSize = true;
            _lblSegment.Location = new Point(78, 0);
            _lblSegment.Name = "_lblSegment";
            _lblSegment.Size = new Size(38, 15);
            _lblSegment.TabIndex = 1;
            _lblSegment.Text = "label4";
            // 
            // _pbSegment
            // 
            _pbSegment.Location = new Point(3, 24);
            _pbSegment.Name = "_pbSegment";
            _pbSegment.Size = new Size(400, 23);
            _pbSegment.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.Bottom;
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.Controls.Add(tableLayoutPanel2);
            flowLayoutPanel1.Controls.Add(_pbOverall);
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.Location = new Point(3, 6);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(406, 50);
            flowLayoutPanel1.TabIndex = 0;
            flowLayoutPanel1.WrapContents = false;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.AutoSize = true;
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(label1, 0, 0);
            tableLayoutPanel2.Controls.Add(_lblOverall, 1, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(400, 15);
            tableLayoutPanel2.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(69, 15);
            label1.TabIndex = 0;
            label1.Text = "Состояние:";
            // 
            // _lblOverall
            // 
            _lblOverall.AutoSize = true;
            _lblOverall.Location = new Point(78, 0);
            _lblOverall.Name = "_lblOverall";
            _lblOverall.Size = new Size(38, 15);
            _lblOverall.TabIndex = 1;
            _lblOverall.Text = "label2";
            // 
            // _pbOverall
            // 
            _pbOverall.Location = new Point(3, 24);
            _pbOverall.Name = "_pbOverall";
            _pbOverall.Size = new Size(400, 23);
            _pbOverall.TabIndex = 2;
            // 
            // _btnCancel
            // 
            _btnCancel.Anchor = AnchorStyles.None;
            _btnCancel.Cursor = Cursors.Hand;
            _btnCancel.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _btnCancel.Location = new Point(146, 123);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(120, 40);
            _btnCancel.TabIndex = 2;
            _btnCancel.Text = "Отмена";
            _btnCancel.UseVisualStyleBackColor = true;
            // 
            // ImportProgressControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            Controls.Add(_tlpContainer);
            Name = "ImportProgressControl";
            Size = new Size(412, 171);
            _tlpContainer.ResumeLayout(false);
            _tlpContainer.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel _tlpContainer;
        private FlowLayoutPanel flowLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private Label label3;
        private Label _lblSegment;
        private ProgressBar _pbSegment;
        private FlowLayoutPanel flowLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Label label1;
        private Label _lblOverall;
        private ProgressBar _pbOverall;
        private Button _btnCancel;
    }
}
