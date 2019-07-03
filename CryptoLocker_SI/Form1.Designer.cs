namespace CryptoLocker_SI
{
    partial class Form1
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
            this.btnDecript = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lstFilesEncryted = new System.Windows.Forms.ListBox();
            this.lblListFiles = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDecript
            // 
            this.btnDecript.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDecript.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnDecript.Location = new System.Drawing.Point(525, 0);
            this.btnDecript.Name = "btnDecript";
            this.btnDecript.Size = new System.Drawing.Size(115, 60);
            this.btnDecript.TabIndex = 6;
            this.btnDecript.Text = "Decript";
            this.btnDecript.UseVisualStyleBackColor = true;
            this.btnDecript.Click += new System.EventHandler(this.btnDecript_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lblListFiles);
            this.splitContainer1.Panel1.Controls.Add(this.btnDecript);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lstFilesEncryted);
            this.splitContainer1.Size = new System.Drawing.Size(640, 387);
            this.splitContainer1.SplitterDistance = 60;
            this.splitContainer1.TabIndex = 7;
            // 
            // lstFilesEncryted
            // 
            this.lstFilesEncryted.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstFilesEncryted.FormattingEnabled = true;
            this.lstFilesEncryted.Location = new System.Drawing.Point(0, 0);
            this.lstFilesEncryted.Name = "lstFilesEncryted";
            this.lstFilesEncryted.Size = new System.Drawing.Size(640, 323);
            this.lstFilesEncryted.TabIndex = 0;
            // 
            // lblListFiles
            // 
            this.lblListFiles.AutoSize = true;
            this.lblListFiles.Font = new System.Drawing.Font("Minion Pro", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblListFiles.Location = new System.Drawing.Point(12, 24);
            this.lblListFiles.Name = "lblListFiles";
            this.lblListFiles.Size = new System.Drawing.Size(189, 26);
            this.lblListFiles.TabIndex = 7;
            this.lblListFiles.Text = "List of Files Encripted:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 387);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CryptoLocker";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDecript;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox lstFilesEncryted;
        private System.Windows.Forms.Label lblListFiles;
    }
}

