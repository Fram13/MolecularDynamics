namespace MolecularDynamics.Application
{
    partial class VisualizationForm
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
            this.components = new System.ComponentModel.Container();
            this.glControl = new OpenTK.GLControl();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // glControl
            // 
            this.glControl.BackColor = System.Drawing.Color.Black;
            this.glControl.Location = new System.Drawing.Point(12, 12);
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(800, 800);
            this.glControl.TabIndex = 0;
            this.glControl.VSync = true;
            this.glControl.Load += new System.EventHandler(this.GlControl_Load);
            this.glControl.Paint += new System.Windows.Forms.PaintEventHandler(this.GlControl_Paint);
            this.glControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GlControl_MouseMove);
            this.glControl.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.GlControl_MouseWheel);
            this.glControl.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.glControl_PreviewKeyDown);
            // 
            // timer
            // 
            this.timer.Interval = 25;
            this.timer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // VisualizationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(828, 823);
            this.Controls.Add(this.glControl);
            this.Name = "VisualizationForm";
            this.Text = "Визуализация";
            this.ResumeLayout(false);

        }

        #endregion

        private OpenTK.GLControl glControl;
        private System.Windows.Forms.Timer timer;
    }
}