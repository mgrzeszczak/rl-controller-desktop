namespace rlc_gui
{
    partial class Window
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
            this.twoControllersCheckBox = new System.Windows.Forms.CheckBox();
            this.onOffButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.idComboBox1 = new System.Windows.Forms.ComboBox();
            this.controllerStatus1 = new System.Windows.Forms.Panel();
            this.controller1GroupBox = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.controller2GroupBox = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.idComboBox2 = new System.Windows.Forms.ComboBox();
            this.controllerStatus2 = new System.Windows.Forms.Panel();
            this.serverStatusLabel = new System.Windows.Forms.Label();
            this.controller1GroupBox.SuspendLayout();
            this.controller2GroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // twoControllersCheckBox
            // 
            this.twoControllersCheckBox.Location = new System.Drawing.Point(12, 46);
            this.twoControllersCheckBox.Name = "twoControllersCheckBox";
            this.twoControllersCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.twoControllersCheckBox.Size = new System.Drawing.Size(111, 17);
            this.twoControllersCheckBox.TabIndex = 0;
            this.twoControllersCheckBox.Text = "Two controllers";
            this.twoControllersCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.twoControllersCheckBox.UseVisualStyleBackColor = true;
            this.twoControllersCheckBox.CheckedChanged += new System.EventHandler(this.twoControllersCheckBox_CheckedChanged);
            // 
            // onOffButton
            // 
            this.onOffButton.Location = new System.Drawing.Point(129, 40);
            this.onOffButton.Name = "onOffButton";
            this.onOffButton.Size = new System.Drawing.Size(111, 23);
            this.onOffButton.TabIndex = 5;
            this.onOffButton.Text = "Start";
            this.onOffButton.UseVisualStyleBackColor = true;
            this.onOffButton.Click += new System.EventHandler(this.onOffButton_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 26);
            this.label3.TabIndex = 6;
            this.label3.Text = "Server status:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // idComboBox1
            // 
            this.idComboBox1.FormattingEnabled = true;
            this.idComboBox1.Location = new System.Drawing.Point(58, 19);
            this.idComboBox1.Name = "idComboBox1";
            this.idComboBox1.Size = new System.Drawing.Size(47, 21);
            this.idComboBox1.TabIndex = 9;
            this.idComboBox1.SelectedIndexChanged += new System.EventHandler(this.idComboBox1_SelectedIndexChanged);
            // 
            // controllerStatus1
            // 
            this.controllerStatus1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.controllerStatus1.Location = new System.Drawing.Point(58, 46);
            this.controllerStatus1.Name = "controllerStatus1";
            this.controllerStatus1.Size = new System.Drawing.Size(47, 20);
            this.controllerStatus1.TabIndex = 10;
            // 
            // controller1GroupBox
            // 
            this.controller1GroupBox.Controls.Add(this.label9);
            this.controller1GroupBox.Controls.Add(this.label7);
            this.controller1GroupBox.Controls.Add(this.idComboBox1);
            this.controller1GroupBox.Controls.Add(this.controllerStatus1);
            this.controller1GroupBox.Location = new System.Drawing.Point(12, 69);
            this.controller1GroupBox.Name = "controller1GroupBox";
            this.controller1GroupBox.Size = new System.Drawing.Size(111, 75);
            this.controller1GroupBox.TabIndex = 14;
            this.controller1GroupBox.TabStop = false;
            this.controller1GroupBox.Text = "Controller 1";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(7, 46);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 20);
            this.label9.TabIndex = 11;
            this.label9.Text = "Status";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(7, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 21);
            this.label7.TabIndex = 10;
            this.label7.Text = "ID";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // controller2GroupBox
            // 
            this.controller2GroupBox.Controls.Add(this.label8);
            this.controller2GroupBox.Controls.Add(this.label10);
            this.controller2GroupBox.Controls.Add(this.idComboBox2);
            this.controller2GroupBox.Controls.Add(this.controllerStatus2);
            this.controller2GroupBox.Enabled = false;
            this.controller2GroupBox.Location = new System.Drawing.Point(129, 69);
            this.controller2GroupBox.Name = "controller2GroupBox";
            this.controller2GroupBox.Size = new System.Drawing.Size(111, 75);
            this.controller2GroupBox.TabIndex = 15;
            this.controller2GroupBox.TabStop = false;
            this.controller2GroupBox.Text = "Controller 2";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(7, 46);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 20);
            this.label8.TabIndex = 11;
            this.label8.Text = "Status";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(7, 19);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(45, 21);
            this.label10.TabIndex = 10;
            this.label10.Text = "ID";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // idComboBox2
            // 
            this.idComboBox2.FormattingEnabled = true;
            this.idComboBox2.Location = new System.Drawing.Point(58, 19);
            this.idComboBox2.Name = "idComboBox2";
            this.idComboBox2.Size = new System.Drawing.Size(47, 21);
            this.idComboBox2.TabIndex = 9;
            this.idComboBox2.SelectedIndexChanged += new System.EventHandler(this.idComboBox2_SelectedIndexChanged);
            // 
            // controllerStatus2
            // 
            this.controllerStatus2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.controllerStatus2.Location = new System.Drawing.Point(58, 46);
            this.controllerStatus2.Name = "controllerStatus2";
            this.controllerStatus2.Size = new System.Drawing.Size(47, 20);
            this.controllerStatus2.TabIndex = 10;
            // 
            // serverStatusLabel
            // 
            this.serverStatusLabel.Location = new System.Drawing.Point(130, 11);
            this.serverStatusLabel.Name = "serverStatusLabel";
            this.serverStatusLabel.Size = new System.Drawing.Size(110, 26);
            this.serverStatusLabel.TabIndex = 16;
            this.serverStatusLabel.Text = "Off";
            this.serverStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(253, 156);
            this.Controls.Add(this.serverStatusLabel);
            this.Controls.Add(this.controller2GroupBox);
            this.Controls.Add(this.controller1GroupBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.onOffButton);
            this.Controls.Add(this.twoControllersCheckBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Window";
            this.Text = "RL Controller Server";
            this.controller1GroupBox.ResumeLayout(false);
            this.controller2GroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox twoControllersCheckBox;
        private System.Windows.Forms.Button onOffButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox idComboBox1;
        private System.Windows.Forms.Panel controllerStatus1;
        private System.Windows.Forms.GroupBox controller1GroupBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox controller2GroupBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox idComboBox2;
        private System.Windows.Forms.Panel controllerStatus2;
        private System.Windows.Forms.Label serverStatusLabel;
    }
}

