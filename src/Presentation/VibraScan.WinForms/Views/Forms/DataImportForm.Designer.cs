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
            _mainPanel = new Panel();
            SuspendLayout();
            // 
            // _mainPanel
            // 
            _mainPanel.AutoSize = true;
            _mainPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _mainPanel.Dock = DockStyle.Fill;
            _mainPanel.Location = new Point(3, 3);
            _mainPanel.Name = "_mainPanel";
            _mainPanel.Size = new Size(428, 205);
            _mainPanel.TabIndex = 0;
            // 
            // DataImportForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(434, 211);
            Controls.Add(_mainPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "DataImportForm";
            Padding = new Padding(3);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DataImportForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel _mainPanel;
    }
}