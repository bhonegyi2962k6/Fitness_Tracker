namespace Fitness_Tracker.Views
{
    partial class frmCycling
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
            this.btnCyclingRecord = new Guna.UI2.WinForms.Guna2Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtCyclingDuration = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtCyclingDistance = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtCyclingSpeed = new Guna.UI2.WinForms.Guna2TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCyclingRecord
            // 
            this.btnCyclingRecord.BorderRadius = 5;
            this.btnCyclingRecord.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCyclingRecord.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCyclingRecord.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCyclingRecord.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnCyclingRecord.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.btnCyclingRecord.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCyclingRecord.ForeColor = System.Drawing.Color.White;
            this.btnCyclingRecord.Location = new System.Drawing.Point(732, 559);
            this.btnCyclingRecord.Name = "btnCyclingRecord";
            this.btnCyclingRecord.Size = new System.Drawing.Size(122, 46);
            this.btnCyclingRecord.TabIndex = 34;
            this.btnCyclingRecord.Text = "Record";
            this.btnCyclingRecord.Click += new System.EventHandler(this.btnCyclingRecord_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(453, 339);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 21);
            this.label4.TabIndex = 33;
            this.label4.Text = "Distance:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(453, 465);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 21);
            this.label3.TabIndex = 32;
            this.label3.Text = "Ride Duration:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(453, 226);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 21);
            this.label2.TabIndex = 31;
            this.label2.Text = "Speed:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(427, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 34);
            this.label1.TabIndex = 30;
            this.label1.Text = "Cycling";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Fitness_Tracker.Properties.Resources.cycling;
            this.pictureBox1.Location = new System.Drawing.Point(65, 184);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(344, 339);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 35;
            this.pictureBox1.TabStop = false;
            // 
            // txtCyclingDuration
            // 
            this.txtCyclingDuration.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.txtCyclingDuration.BorderThickness = 2;
            this.txtCyclingDuration.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCyclingDuration.DefaultText = "";
            this.txtCyclingDuration.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtCyclingDuration.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtCyclingDuration.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCyclingDuration.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCyclingDuration.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCyclingDuration.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCyclingDuration.ForeColor = System.Drawing.Color.Black;
            this.txtCyclingDuration.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCyclingDuration.IconLeft = global::Fitness_Tracker.Properties.Resources.hourglass;
            this.txtCyclingDuration.IconLeftSize = new System.Drawing.Size(25, 25);
            this.txtCyclingDuration.Location = new System.Drawing.Point(647, 438);
            this.txtCyclingDuration.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtCyclingDuration.Name = "txtCyclingDuration";
            this.txtCyclingDuration.PasswordChar = '\0';
            this.txtCyclingDuration.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtCyclingDuration.PlaceholderText = "Enter Your Riding Duration";
            this.txtCyclingDuration.SelectedText = "";
            this.txtCyclingDuration.Size = new System.Drawing.Size(321, 48);
            this.txtCyclingDuration.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtCyclingDuration.TabIndex = 29;
            // 
            // txtCyclingDistance
            // 
            this.txtCyclingDistance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.txtCyclingDistance.BorderThickness = 2;
            this.txtCyclingDistance.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCyclingDistance.DefaultText = "";
            this.txtCyclingDistance.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtCyclingDistance.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtCyclingDistance.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCyclingDistance.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCyclingDistance.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCyclingDistance.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCyclingDistance.ForeColor = System.Drawing.Color.Black;
            this.txtCyclingDistance.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCyclingDistance.IconLeft = global::Fitness_Tracker.Properties.Resources.distance;
            this.txtCyclingDistance.IconLeftSize = new System.Drawing.Size(25, 25);
            this.txtCyclingDistance.Location = new System.Drawing.Point(647, 312);
            this.txtCyclingDistance.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtCyclingDistance.Name = "txtCyclingDistance";
            this.txtCyclingDistance.PasswordChar = '\0';
            this.txtCyclingDistance.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtCyclingDistance.PlaceholderText = "Enter Your Riding Distance";
            this.txtCyclingDistance.SelectedText = "";
            this.txtCyclingDistance.Size = new System.Drawing.Size(321, 48);
            this.txtCyclingDistance.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtCyclingDistance.TabIndex = 28;
            // 
            // txtCyclingSpeed
            // 
            this.txtCyclingSpeed.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.txtCyclingSpeed.BorderThickness = 2;
            this.txtCyclingSpeed.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCyclingSpeed.DefaultText = "";
            this.txtCyclingSpeed.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtCyclingSpeed.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtCyclingSpeed.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCyclingSpeed.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCyclingSpeed.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCyclingSpeed.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCyclingSpeed.ForeColor = System.Drawing.Color.Black;
            this.txtCyclingSpeed.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCyclingSpeed.IconLeft = global::Fitness_Tracker.Properties.Resources.speedometer;
            this.txtCyclingSpeed.IconLeftSize = new System.Drawing.Size(25, 25);
            this.txtCyclingSpeed.Location = new System.Drawing.Point(647, 199);
            this.txtCyclingSpeed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtCyclingSpeed.Name = "txtCyclingSpeed";
            this.txtCyclingSpeed.PasswordChar = '\0';
            this.txtCyclingSpeed.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtCyclingSpeed.PlaceholderText = "Enter Your Riding Steps";
            this.txtCyclingSpeed.SelectedText = "";
            this.txtCyclingSpeed.Size = new System.Drawing.Size(321, 48);
            this.txtCyclingSpeed.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtCyclingSpeed.TabIndex = 27;
            // 
            // frmCycling
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnCyclingRecord);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtCyclingDuration);
            this.Controls.Add(this.txtCyclingDistance);
            this.Controls.Add(this.txtCyclingSpeed);
            this.Name = "frmCycling";
            this.Size = new System.Drawing.Size(1037, 700);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private Guna.UI2.WinForms.Guna2Button btnCyclingRecord;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2TextBox txtCyclingDuration;
        private Guna.UI2.WinForms.Guna2TextBox txtCyclingDistance;
        private Guna.UI2.WinForms.Guna2TextBox txtCyclingSpeed;
    }
}
