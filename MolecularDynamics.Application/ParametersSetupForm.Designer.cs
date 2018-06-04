namespace MolecularDynamics.Application
{
    partial class ParametersSetupForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.particleCountXTextBox = new System.Windows.Forms.TextBox();
            this.particleCountYTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.particleCountZTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.temperatureTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.staticLayerCountTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.periodTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.stepTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.threadsTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.createButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Число частиц по оси Ox:";
            // 
            // particleCountXTextBox
            // 
            this.particleCountXTextBox.Location = new System.Drawing.Point(147, 19);
            this.particleCountXTextBox.Name = "particleCountXTextBox";
            this.particleCountXTextBox.Size = new System.Drawing.Size(196, 20);
            this.particleCountXTextBox.TabIndex = 1;
            // 
            // particleCountYTextBox
            // 
            this.particleCountYTextBox.Location = new System.Drawing.Point(147, 45);
            this.particleCountYTextBox.Name = "particleCountYTextBox";
            this.particleCountYTextBox.Size = new System.Drawing.Size(196, 20);
            this.particleCountYTextBox.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Число частиц по оси Oy";
            // 
            // particleCountZTextBox
            // 
            this.particleCountZTextBox.Location = new System.Drawing.Point(147, 71);
            this.particleCountZTextBox.Name = "particleCountZTextBox";
            this.particleCountZTextBox.Size = new System.Drawing.Size(196, 20);
            this.particleCountZTextBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Число частиц по оси Oz:";
            // 
            // temperatureTextBox
            // 
            this.temperatureTextBox.Location = new System.Drawing.Point(173, 201);
            this.temperatureTextBox.Name = "temperatureTextBox";
            this.temperatureTextBox.Size = new System.Drawing.Size(182, 20);
            this.temperatureTextBox.TabIndex = 7;
            this.temperatureTextBox.Text = "200";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 204);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(146, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Температура вещества (К):";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.staticLayerCountTextBox);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.particleCountXTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.particleCountZTextBox);
            this.groupBox1.Controls.Add(this.particleCountYTextBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(359, 131);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Параметры кристаллической решетки";
            // 
            // staticLayerCountTextBox
            // 
            this.staticLayerCountTextBox.Location = new System.Drawing.Point(193, 97);
            this.staticLayerCountTextBox.Name = "staticLayerCountTextBox";
            this.staticLayerCountTextBox.Size = new System.Drawing.Size(150, 20);
            this.staticLayerCountTextBox.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(177, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Количество неподвижных слоев::";
            // 
            // periodTextBox
            // 
            this.periodTextBox.Location = new System.Drawing.Point(255, 175);
            this.periodTextBox.Name = "periodTextBox";
            this.periodTextBox.Size = new System.Drawing.Size(100, 20);
            this.periodTextBox.TabIndex = 6;
            this.periodTextBox.Text = "200";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 178);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(228, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Период появления новой частицы (1е-14 с):";
            // 
            // stepTextBox
            // 
            this.stepTextBox.Location = new System.Drawing.Point(188, 149);
            this.stepTextBox.Name = "stepTextBox";
            this.stepTextBox.Size = new System.Drawing.Size(167, 20);
            this.stepTextBox.TabIndex = 5;
            this.stepTextBox.Text = "0,1";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(22, 152);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(160, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Шаг интегрирования (1е-14 с):";
            // 
            // threadsTextBox
            // 
            this.threadsTextBox.Location = new System.Drawing.Point(205, 227);
            this.threadsTextBox.Name = "threadsTextBox";
            this.threadsTextBox.Size = new System.Drawing.Size(150, 20);
            this.threadsTextBox.TabIndex = 8;
            this.threadsTextBox.Text = "4";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 230);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(177, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Количество потоков вычислений:";
            // 
            // createButton
            // 
            this.createButton.Location = new System.Drawing.Point(25, 255);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(75, 23);
            this.createButton.TabIndex = 9;
            this.createButton.Text = "Применить";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
            // 
            // ParametersSetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 292);
            this.Controls.Add(this.createButton);
            this.Controls.Add(this.threadsTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.stepTextBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.periodTextBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.temperatureTextBox);
            this.Controls.Add(this.label4);
            this.Name = "ParametersSetupForm";
            this.Text = "Параметры моделирования";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox particleCountXTextBox;
        private System.Windows.Forms.TextBox particleCountYTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox particleCountZTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox temperatureTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox staticLayerCountTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox periodTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox stepTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox threadsTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button createButton;
    }
}