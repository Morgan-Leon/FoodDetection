namespace FoodServer.statistic
{
    partial class Analysis
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
           // this.detectioninfoTableAdapter1 = new FoodServer.foodDataSetTableAdapters.detectioninfoTableAdapter();
          //  this.foodDataSet1 = new FoodServer.foodDataSet();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
          //  ((System.ComponentModel.ISupportInitialize)(this.foodDataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // detectioninfoTableAdapter1
            // 
//             this.detectioninfoTableAdapter1.ClearBeforeFill = true;
//             // 
//             // foodDataSet1
//             // 
//             this.foodDataSet1.DataSetName = "foodDataSet";
//             this.foodDataSet1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(2, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(747, 468);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // Analysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 472);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Analysis";
            this.Text = "Analysis";
            //((System.ComponentModel.ISupportInitialize)(this.foodDataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

//         private foodDataSetTableAdapters.detectioninfoTableAdapter detectioninfoTableAdapter1;
//         private foodDataSet foodDataSet1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}