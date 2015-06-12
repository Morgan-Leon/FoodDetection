namespace WindowsApplication1
{
    partial class vectShapes
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // vectShapes
            // 
            this.AllowDrop = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Name = "vectShapes";
            this.Size = new System.Drawing.Size(238, 165);
            this.DoubleClick += new System.EventHandler(this.UserControl1_DoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UserControl1_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.UserControl1_MouseMove);
            this.Resize += new System.EventHandler(this.vectShapes_Resize);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.UserControl1_Paint);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UserControl1_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion


    }
}
