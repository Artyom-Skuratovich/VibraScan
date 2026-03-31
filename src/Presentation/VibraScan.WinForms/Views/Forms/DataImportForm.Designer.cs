namespace VibraScan.WinForms.Views.Forms
{
    partial class DataImportForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tlpLoading = new TableLayoutPanel();
            flowLayoutPanel2 = new FlowLayoutPanel();
            tableLayoutPanel3 = new TableLayoutPanel();
            label3 = new Label();
            lblSegment = new Label();
            pbSegment = new ProgressBar();
            flowLayoutPanel1 = new FlowLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            label1 = new Label();
            lblOverall = new Label();
            pbOverall = new ProgressBar();
            btnCancel = new Button();
            tlpLoading.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tlpLoading
            // 
            tlpLoading.ColumnCount = 1;
            tlpLoading.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpLoading.Controls.Add(flowLayoutPanel2, 0, 1);
            tlpLoading.Controls.Add(flowLayoutPanel1, 0, 0);
            tlpLoading.Controls.Add(btnCancel, 0, 2);
            tlpLoading.Dock = DockStyle.Fill;
            tlpLoading.Location = new Point(0, 0);
            tlpLoading.Name = "tlpLoading";
            tlpLoading.RowCount = 3;
            tlpLoading.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tlpLoading.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tlpLoading.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tlpLoading.Size = new Size(434, 211);
            tlpLoading.TabIndex = 0;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Anchor = AnchorStyles.Bottom;
            flowLayoutPanel2.AutoSize = true;
            flowLayoutPanel2.Controls.Add(tableLayoutPanel3);
            flowLayoutPanel2.Controls.Add(pbSegment);
            flowLayoutPanel2.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel2.Location = new Point(14, 87);
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
            tableLayoutPanel3.Controls.Add(lblSegment, 1, 0);
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
            // lblSegment
            // 
            lblSegment.AutoSize = true;
            lblSegment.Location = new Point(78, 0);
            lblSegment.Name = "lblSegment";
            lblSegment.Size = new Size(38, 15);
            lblSegment.TabIndex = 1;
            lblSegment.Text = "label4";
            // 
            // pbSegment
            // 
            pbSegment.Location = new Point(3, 24);
            pbSegment.Name = "pbSegment";
            pbSegment.Size = new Size(400, 23);
            pbSegment.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.Bottom;
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.Controls.Add(tableLayoutPanel2);
            flowLayoutPanel1.Controls.Add(pbOverall);
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.Location = new Point(14, 17);
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
            tableLayoutPanel2.Controls.Add(lblOverall, 1, 0);
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
            // lblOverall
            // 
            lblOverall.AutoSize = true;
            lblOverall.Location = new Point(78, 0);
            lblOverall.Name = "lblOverall";
            lblOverall.Size = new Size(38, 15);
            lblOverall.TabIndex = 1;
            lblOverall.Text = "label2";
            // 
            // pbOverall
            // 
            pbOverall.Location = new Point(3, 24);
            pbOverall.Name = "pbOverall";
            pbOverall.Size = new Size(400, 23);
            pbOverall.TabIndex = 2;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.None;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            btnCancel.Location = new Point(157, 155);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(120, 40);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Отмена";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // DataImportForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(434, 211);
            Controls.Add(tlpLoading);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "DataImportForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DataImportForm";
            tlpLoading.ResumeLayout(false);
            tlpLoading.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tlpLoading;
        private FlowLayoutPanel flowLayoutPanel1;
        private ProgressBar pbOverall;
        private TableLayoutPanel tableLayoutPanel2;
        private Label label1;
        private Label lblOverall;
        private FlowLayoutPanel flowLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private Label label3;
        private Label lblSegment;
        private ProgressBar pbSegment;
        private Button btnCancel;
    }
}