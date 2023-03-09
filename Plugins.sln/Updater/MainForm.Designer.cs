namespace Updater
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            progbarDownload = new ProgressBar();
            tableLayoutPanel1 = new TableLayoutPanel();
            lb_progress = new Label();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // progbarDownload
            // 
            progbarDownload.Dock = DockStyle.Fill;
            progbarDownload.Location = new Point(10, 60);
            progbarDownload.Margin = new Padding(10);
            progbarDownload.Name = "progbarDownload";
            progbarDownload.Size = new Size(480, 30);
            progbarDownload.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(progbarDownload, 0, 1);
            tableLayoutPanel1.Controls.Add(lb_progress, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(500, 100);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // lb_progress
            // 
            lb_progress.AutoSize = true;
            lb_progress.Dock = DockStyle.Fill;
            lb_progress.Location = new Point(10, 10);
            lb_progress.Margin = new Padding(10);
            lb_progress.Name = "lb_progress";
            lb_progress.Size = new Size(480, 30);
            lb_progress.TabIndex = 1;
            lb_progress.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(500, 100);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(2);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MainForm";
            TopMost = true;
            Load += MainForm_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ProgressBar progbarDownload;
        private TableLayoutPanel tableLayoutPanel1;
        private Label lb_progress;
    }
}