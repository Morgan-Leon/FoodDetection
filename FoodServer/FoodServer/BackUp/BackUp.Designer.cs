namespace FoodServer.BackUp
{
    partial class BackUp
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
            this.buttonBackup = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonRestore = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonBackup
            // 
            this.buttonBackup.Location = new System.Drawing.Point(45, 97);
            this.buttonBackup.Margin = new System.Windows.Forms.Padding(4);
            this.buttonBackup.Name = "buttonBackup";
            this.buttonBackup.Size = new System.Drawing.Size(156, 60);
            this.buttonBackup.TabIndex = 0;
            this.buttonBackup.Text = "备份所有数据库";
            this.buttonBackup.UseVisualStyleBackColor = true;
            this.buttonBackup.Click += new System.EventHandler(this.buttonBackup_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonRestore);
            this.groupBox1.Controls.Add(this.buttonBackup);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(514, 230);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "备份及恢复全部数据库";
            // 
            // buttonRestore
            // 
            this.buttonRestore.Location = new System.Drawing.Point(299, 97);
            this.buttonRestore.Margin = new System.Windows.Forms.Padding(4);
            this.buttonRestore.Name = "buttonRestore";
            this.buttonRestore.Size = new System.Drawing.Size(156, 60);
            this.buttonRestore.TabIndex = 1;
            this.buttonRestore.Text = "恢复所有数据库";
            this.buttonRestore.UseVisualStyleBackColor = true;
            this.buttonRestore.Click += new System.EventHandler(this.buttonRestore_Click);
            // 
            // BackUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 262);
            this.Controls.Add(this.groupBox1);
            this.Name = "BackUp";
            this.Text = "备份及恢复数据库";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonBackup;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonRestore;
    }
}