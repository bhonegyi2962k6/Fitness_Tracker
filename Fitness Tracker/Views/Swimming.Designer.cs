namespace Fitness_Tracker.Views
{
    partial class frmSwimming
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSwimmingRecord = new Guna.UI2.WinForms.Guna2Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtSwimmingHeartRate = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtSwimmingTime = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtSwimmingLaps = new Guna.UI2.WinForms.Guna2TextBox();
            this.cboIntensity = new Guna.UI2.WinForms.Guna2ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cartesianChart1 = new LiveCharts.WinForms.CartesianChart();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(424, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 34);
            this.label1.TabIndex = 3;
            this.label1.Text = "Swimming";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(474, 213);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 21);
            this.label2.TabIndex = 4;
            this.label2.Text = "Laps:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(474, 452);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(186, 21);
            this.label3.TabIndex = 5;
            this.label3.Text = "Average Heart Rate:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(474, 326);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 21);
            this.label4.TabIndex = 6;
            this.label4.Text = "Time Taken:";
            // 
            // btnSwimmingRecord
            // 
            this.btnSwimmingRecord.BorderRadius = 5;
            this.btnSwimmingRecord.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnSwimmingRecord.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnSwimmingRecord.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnSwimmingRecord.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnSwimmingRecord.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.btnSwimmingRecord.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSwimmingRecord.ForeColor = System.Drawing.Color.White;
            this.btnSwimmingRecord.Location = new System.Drawing.Point(579, 575);
            this.btnSwimmingRecord.Name = "btnSwimmingRecord";
            this.btnSwimmingRecord.Size = new System.Drawing.Size(122, 46);
            this.btnSwimmingRecord.TabIndex = 16;
            this.btnSwimmingRecord.Text = "Record";
            this.btnSwimmingRecord.Click += new System.EventHandler(this.btnSwimmingRecord_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Fitness_Tracker.Properties.Resources.swimming_graphic_clipart_design_free_png;
            this.pictureBox1.Location = new System.Drawing.Point(58, 233);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(344, 275);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            // 
            // txtSwimmingHeartRate
            // 
            this.txtSwimmingHeartRate.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.txtSwimmingHeartRate.BorderThickness = 2;
            this.txtSwimmingHeartRate.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSwimmingHeartRate.DefaultText = "";
            this.txtSwimmingHeartRate.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtSwimmingHeartRate.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtSwimmingHeartRate.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtSwimmingHeartRate.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtSwimmingHeartRate.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtSwimmingHeartRate.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSwimmingHeartRate.ForeColor = System.Drawing.Color.Black;
            this.txtSwimmingHeartRate.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtSwimmingHeartRate.IconLeft = global::Fitness_Tracker.Properties.Resources.heart_rate;
            this.txtSwimmingHeartRate.IconLeftSize = new System.Drawing.Size(25, 25);
            this.txtSwimmingHeartRate.Location = new System.Drawing.Point(478, 477);
            this.txtSwimmingHeartRate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtSwimmingHeartRate.Name = "txtSwimmingHeartRate";
            this.txtSwimmingHeartRate.PasswordChar = '\0';
            this.txtSwimmingHeartRate.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtSwimmingHeartRate.PlaceholderText = "Enter Your Average Heart Rate ";
            this.txtSwimmingHeartRate.SelectedText = "";
            this.txtSwimmingHeartRate.Size = new System.Drawing.Size(321, 48);
            this.txtSwimmingHeartRate.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtSwimmingHeartRate.TabIndex = 2;
            // 
            // txtSwimmingTime
            // 
            this.txtSwimmingTime.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.txtSwimmingTime.BorderThickness = 2;
            this.txtSwimmingTime.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSwimmingTime.DefaultText = "";
            this.txtSwimmingTime.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtSwimmingTime.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtSwimmingTime.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtSwimmingTime.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtSwimmingTime.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtSwimmingTime.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSwimmingTime.ForeColor = System.Drawing.Color.Black;
            this.txtSwimmingTime.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtSwimmingTime.IconLeft = global::Fitness_Tracker.Properties.Resources.distance;
            this.txtSwimmingTime.IconLeftSize = new System.Drawing.Size(25, 25);
            this.txtSwimmingTime.Location = new System.Drawing.Point(478, 351);
            this.txtSwimmingTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtSwimmingTime.Name = "txtSwimmingTime";
            this.txtSwimmingTime.PasswordChar = '\0';
            this.txtSwimmingTime.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtSwimmingTime.PlaceholderText = "Enter Your Swimming Time";
            this.txtSwimmingTime.SelectedText = "";
            this.txtSwimmingTime.Size = new System.Drawing.Size(321, 48);
            this.txtSwimmingTime.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtSwimmingTime.TabIndex = 1;
            // 
            // txtSwimmingLaps
            // 
            this.txtSwimmingLaps.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.txtSwimmingLaps.BorderThickness = 2;
            this.txtSwimmingLaps.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSwimmingLaps.DefaultText = "";
            this.txtSwimmingLaps.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtSwimmingLaps.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtSwimmingLaps.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtSwimmingLaps.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtSwimmingLaps.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtSwimmingLaps.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSwimmingLaps.ForeColor = System.Drawing.Color.Black;
            this.txtSwimmingLaps.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtSwimmingLaps.IconLeft = global::Fitness_Tracker.Properties.Resources.reload;
            this.txtSwimmingLaps.IconLeftSize = new System.Drawing.Size(25, 25);
            this.txtSwimmingLaps.Location = new System.Drawing.Point(478, 238);
            this.txtSwimmingLaps.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtSwimmingLaps.Name = "txtSwimmingLaps";
            this.txtSwimmingLaps.PasswordChar = '\0';
            this.txtSwimmingLaps.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtSwimmingLaps.PlaceholderText = "Enter Your Swimming Laps";
            this.txtSwimmingLaps.SelectedText = "";
            this.txtSwimmingLaps.Size = new System.Drawing.Size(321, 48);
            this.txtSwimmingLaps.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtSwimmingLaps.TabIndex = 0;
            // 
            // cboIntensity
            // 
            this.cboIntensity.BackColor = System.Drawing.Color.Transparent;
            this.cboIntensity.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.cboIntensity.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboIntensity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboIntensity.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cboIntensity.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cboIntensity.Font = new System.Drawing.Font("Century Gothic", 10.2F);
            this.cboIntensity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cboIntensity.ItemHeight = 30;
            this.cboIntensity.Items.AddRange(new object[] {
            "Light",
            "Moderate",
            "Vigorous"});
            this.cboIntensity.Location = new System.Drawing.Point(478, 152);
            this.cboIntensity.Name = "cboIntensity";
            this.cboIntensity.Size = new System.Drawing.Size(321, 36);
            this.cboIntensity.TabIndex = 56;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(474, 128);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(122, 21);
            this.label5.TabIndex = 57;
            this.label5.Text = "Activity Type:";
            // 
            // cartesianChart1
            // 
            this.cartesianChart1.Location = new System.Drawing.Point(865, 233);
            this.cartesianChart1.Name = "cartesianChart1";
            this.cartesianChart1.Size = new System.Drawing.Size(401, 334);
            this.cartesianChart1.TabIndex = 59;
            this.cartesianChart1.Text = "cartesianChart1";
            // 
            // frmSwimming
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.cartesianChart1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cboIntensity);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnSwimmingRecord);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSwimmingHeartRate);
            this.Controls.Add(this.txtSwimmingTime);
            this.Controls.Add(this.txtSwimmingLaps);
            this.Name = "frmSwimming";
            this.Size = new System.Drawing.Size(1337, 900);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2TextBox txtSwimmingLaps;
        private Guna.UI2.WinForms.Guna2TextBox txtSwimmingTime;
        private Guna.UI2.WinForms.Guna2TextBox txtSwimmingHeartRate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private Guna.UI2.WinForms.Guna2Button btnSwimmingRecord;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Guna.UI2.WinForms.Guna2ComboBox cboIntensity;
        private System.Windows.Forms.Label label5;
        private LiveCharts.WinForms.CartesianChart cartesianChart1;
    }
}
