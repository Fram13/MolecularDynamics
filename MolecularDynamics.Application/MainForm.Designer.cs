namespace MolecularDynamics.Application
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.createConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startStopIntegrationButton = new System.Windows.Forms.ToolStripButton();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.startStopIntegrationButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(694, 25);
            this.toolStrip.TabIndex = 2;
            this.toolStrip.Text = "toolStrip";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createConfigurationToolStripMenuItem,
            this.LoadConfigurationToolStripMenuItem,
            this.SaveConfigurationToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(101, 22);
            this.toolStripDropDownButton1.Text = "Конфигурация";
            // 
            // createConfigurationToolStripMenuItem
            // 
            this.createConfigurationToolStripMenuItem.Name = "createConfigurationToolStripMenuItem";
            this.createConfigurationToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.createConfigurationToolStripMenuItem.Text = "Создать";
            this.createConfigurationToolStripMenuItem.Click += new System.EventHandler(this.CreateConfigurationToolStripMenuItem_Click);
            // 
            // LoadConfigurationToolStripMenuItem
            // 
            this.LoadConfigurationToolStripMenuItem.Name = "LoadConfigurationToolStripMenuItem";
            this.LoadConfigurationToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.LoadConfigurationToolStripMenuItem.Text = "Загрузить";
            this.LoadConfigurationToolStripMenuItem.Click += new System.EventHandler(this.LoadConfigurationToolStripMenuItem_Click);
            // 
            // SaveConfigurationToolStripMenuItem
            // 
            this.SaveConfigurationToolStripMenuItem.Enabled = false;
            this.SaveConfigurationToolStripMenuItem.Name = "SaveConfigurationToolStripMenuItem";
            this.SaveConfigurationToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.SaveConfigurationToolStripMenuItem.Text = "Сохранить";
            this.SaveConfigurationToolStripMenuItem.Click += new System.EventHandler(this.SaveConfigurationToolStripMenuItem_Click);
            // 
            // startStopIntegrationButton
            // 
            this.startStopIntegrationButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.startStopIntegrationButton.Image = ((System.Drawing.Image)(resources.GetObject("startStopIntegrationButton.Image")));
            this.startStopIntegrationButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.startStopIntegrationButton.Name = "startStopIntegrationButton";
            this.startStopIntegrationButton.Size = new System.Drawing.Size(42, 22);
            this.startStopIntegrationButton.Text = "Старт";
            this.startStopIntegrationButton.Click += new System.EventHandler(this.startStopIntegrationButton_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(12, 28);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(454, 500);
            this.dataGridView.TabIndex = 3;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "Бинарные файлы (*.bin)|*.bin";
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Бинарные файлы (*.bin)|*.bin";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 550);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.toolStrip);
            this.Name = "MainForm";
            this.Text = "Моделирование процессов модификации металлических тонких пленок";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem createConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LoadConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton startStopIntegrationButton;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}

