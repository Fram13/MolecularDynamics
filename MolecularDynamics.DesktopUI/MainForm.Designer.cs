namespace MolecularDynamics.DesktopUI
{
    partial class MainForm
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // glControl
            // 
            this.glControl.BackColor = System.Drawing.Color.Black;
            this.glControl.Location = new System.Drawing.Point(13, 13);
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(259, 237);
            this.glControl.TabIndex = 0;
            this.glControl.VSync = false;
            this.glControl.Paint += new System.Windows.Forms.PaintEventHandler(GLControl_Paint);
            this.glControl.MouseMove += new System.Windows.Forms.MouseEventHandler(GlControl_MouseMove);
            this.glControl.MouseWheel += new System.Windows.Forms.MouseEventHandler(GlControl_MouseWheel);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.glControl);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.Load += new System.EventHandler(MainForm_Load);
            this.Resize += new System.EventHandler(MainForm_Resize);
            //
            // timer1
            //
            this.timer1.Tick += new System.EventHandler(Timer1_Tick);
        }

        #endregion

        private OpenTK.GLControl glControl;
        private System.Windows.Forms.Timer timer1;
    }
}

