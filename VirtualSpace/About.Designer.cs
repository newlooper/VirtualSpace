namespace VirtualSpace
{
    partial class About
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lb_AppName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lb_Version = new System.Windows.Forms.Label();
            this.lb_Copyright = new System.Windows.Forms.Label();
            this.btn_OK = new System.Windows.Forms.Button();
            this.lbox_Env = new System.Windows.Forms.ListBox();
            this.llb_Company = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(72, 72);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // lb_AppName
            // 
            this.lb_AppName.AutoSize = true;
            this.lb_AppName.Font = new System.Drawing.Font("Arial", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lb_AppName.ForeColor = System.Drawing.Color.Black;
            this.lb_AppName.Location = new System.Drawing.Point(90, 17);
            this.lb_AppName.Name = "lb_AppName";
            this.lb_AppName.Size = new System.Drawing.Size(366, 67);
            this.lb_AppName.TabIndex = 1;
            this.lb_AppName.Text = "VirtualSpace";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "Version";
            // 
            // lb_Version
            // 
            this.lb_Version.AutoSize = true;
            this.lb_Version.Location = new System.Drawing.Point(106, 97);
            this.lb_Version.Name = "lb_Version";
            this.lb_Version.Size = new System.Drawing.Size(74, 24);
            this.lb_Version.TabIndex = 4;
            this.lb_Version.Text = "Version";
            // 
            // lb_Copyright
            // 
            this.lb_Copyright.AutoSize = true;
            this.lb_Copyright.Location = new System.Drawing.Point(12, 124);
            this.lb_Copyright.Name = "lb_Copyright";
            this.lb_Copyright.Size = new System.Drawing.Size(97, 24);
            this.lb_Copyright.TabIndex = 5;
            this.lb_Copyright.Text = "Copyright";
            // 
            // btn_OK
            // 
            this.btn_OK.Location = new System.Drawing.Point(638, 404);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(150, 34);
            this.btn_OK.TabIndex = 6;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // lbox_Env
            // 
            this.lbox_Env.FormattingEnabled = true;
            this.lbox_Env.ItemHeight = 24;
            this.lbox_Env.Location = new System.Drawing.Point(12, 221);
            this.lbox_Env.Name = "lbox_Env";
            this.lbox_Env.Size = new System.Drawing.Size(625, 172);
            this.lbox_Env.TabIndex = 7;
            // 
            // llb_Company
            // 
            this.llb_Company.AutoSize = true;
            this.llb_Company.Location = new System.Drawing.Point(12, 151);
            this.llb_Company.Name = "llb_Company";
            this.llb_Company.Size = new System.Drawing.Size(93, 24);
            this.llb_Company.TabIndex = 8;
            this.llb_Company.TabStop = true;
            this.llb_Company.Text = "Company";
            this.llb_Company.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llb_Company_LinkClicked);
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.llb_Company);
            this.Controls.Add(this.lbox_Env);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.lb_Copyright);
            this.Controls.Add(this.lb_Version);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lb_AppName);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lb_AppName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lb_Version;
        private System.Windows.Forms.Label lb_Copyright;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.ListBox lbox_Env;
        private System.Windows.Forms.LinkLabel llb_Company;
    }
}