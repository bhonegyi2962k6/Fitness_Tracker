namespace Fitness_Tracker.Views
{
    partial class frmWalking
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
            this.btnWalkingRecord = new Guna.UI2.WinForms.Guna2Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtWalkingTimeTaken = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtWalkingDistance = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtWalkingSteps = new Guna.UI2.WinForms.Guna2TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnWalkingRecord
            // 
            this.btnWalkingRecord.BorderRadius = 5;
            this.btnWalkingRecord.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnWalkingRecord.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnWalkingRecord.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnWalkingRecord.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnWalkingRecord.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.btnWalkingRecord.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWalkingRecord.ForeColor = System.Drawing.Color.White;
            this.btnWalkingRecord.Location = new System.Drawing.Point(731, 559);
            this.btnWalkingRecord.Name = "btnWalkingRecord";
            this.btnWalkingRecord.Size = new System.Drawing.Size(122, 46);
            this.btnWalkingRecord.TabIndex = 25;
            this.btnWalkingRecord.Text = "Record";
            this.btnWalkingRecord.Click += new System.EventHandler(this.btnWalkingRecord_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(452, 339);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 21);
            this.label4.TabIndex = 24;
            this.label4.Text = "Distance:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(452, 465);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 21);
            this.label3.TabIndex = 23;
            this.label3.Text = "Time Taken:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(452, 226);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 21);
            this.label2.TabIndex = 22;
            this.label2.Text = "Steps:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(426, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 34);
            this.label1.TabIndex = 21;
            this.label1.Text = "Walking";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Fitness_Tracker.Properties.Resources._6822a069d67be3e952214658c9a1fe9c_t;
            this.pictureBox1.Location = new System.Drawing.Point(64, 184);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(344, 339);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 26;
            this.pictureBox1.TabStop = false;
            // 
            // txtWalkingTimeTaken
            // 
            this.txtWalkingTimeTaken.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.txtWalkingTimeTaken.BorderThickness = 2;
            this.txtWalkingTimeTaken.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtWalkingTimeTaken.DefaultText = "";
            this.txtWalkingTimeTaken.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtWalkingTimeTaken.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtWalkingTimeTaken.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtWalkingTimeTaken.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtWalkingTimeTaken.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtWalkingTimeTaken.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWalkingTimeTaken.ForeColor = System.Drawing.Color.Black;
            this.txtWalkingTimeTaken.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtWalkingTimeTaken.IconLeft = global::Fitness_Tracker.Properties.Resources.hourglass;
            this.txtWalkingTimeTaken.IconLeftSize = new System.Drawing.Size(25, 25);
            this.txtWalkingTimeTaken.Location = new System.Drawing.Point(646, 438);
            this.txtWalkingTimeTaken.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtWalkingTimeTaken.Name = "txtWalkingTimeTaken";
            this.txtWalkingTimeTaken.PasswordChar = '\0';
            this.txtWalkingTimeTaken.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtWalkingTimeTaken.PlaceholderText = "Enter Your Walking Time Taken";
            this.txtWalkingTimeTaken.SelectedText = "";
            this.txtWalkingTimeTaken.Size = new System.Drawing.Size(321, 48);
            this.txtWalkingTimeTaken.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtWalkingTimeTaken.TabIndex = 20;
            // 
            // txtWalkingDistance
            // 
            this.txtWalkingDistance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.txtWalkingDistance.BorderThickness = 2;
            this.txtWalkingDistance.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtWalkingDistance.DefaultText = "";
            this.txtWalkingDistance.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtWalkingDistance.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtWalkingDistance.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtWalkingDistance.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtWalkingDistance.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtWalkingDistance.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWalkingDistance.ForeColor = System.Drawing.Color.Black;
            this.txtWalkingDistance.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtWalkingDistance.IconLeft = global::Fitness_Tracker.Properties.Resources.distance;
            this.txtWalkingDistance.IconLeftSize = new System.Drawing.Size(25, 25);
            this.txtWalkingDistance.Location = new System.Drawing.Point(646, 312);
            this.txtWalkingDistance.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtWalkingDistance.Name = "txtWalkingDistance";
            this.txtWalkingDistance.PasswordChar = '\0';
            this.txtWalkingDistance.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtWalkingDistance.PlaceholderText = "Enter Your Walking Distance";
            this.txtWalkingDistance.SelectedText = "";
            this.txtWalkingDistance.Size = new System.Drawing.Size(321, 48);
            this.txtWalkingDistance.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtWalkingDistance.TabIndex = 19;
            // 
            // txtWalkingSteps
            // 
            this.txtWalkingSteps.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.txtWalkingSteps.BorderThickness = 2;
            this.txtWalkingSteps.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtWalkingSteps.DefaultText = "";
            this.txtWalkingSteps.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtWalkingSteps.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtWalkingSteps.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtWalkingSteps.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtWalkingSteps.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtWalkingSteps.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWalkingSteps.ForeColor = System.Drawing.Color.Black;
            this.txtWalkingSteps.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtWalkingSteps.IconLeft = global::Fitness_Tracker.Properties.Resources.footsteps;
            this.txtWalkingSteps.IconLeftSize = new System.Drawing.Size(25, 25);
            this.txtWalkingSteps.Location = new System.Drawing.Point(646, 199);
            this.txtWalkingSteps.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtWalkingSteps.Name = "txtWalkingSteps";
            this.txtWalkingSteps.PasswordChar = '\0';
            this.txtWalkingSteps.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtWalkingSteps.PlaceholderText = "Enter Your Walking Steps";
            this.txtWalkingSteps.SelectedText = "";
            this.txtWalkingSteps.Size = new System.Drawing.Size(321, 48);
            this.txtWalkingSteps.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtWalkingSteps.TabIndex = 18;
            // 
            // frmWalking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnWalkingRecord);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtWalkingTimeTaken);
            this.Controls.Add(this.txtWalkingDistance);
            this.Controls.Add(this.txtWalkingSteps);
            this.Name = "frmWalking";
            this.Size = new System.Drawing.Size(1037, 700);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private Guna.UI2.WinForms.Guna2Button btnWalkingRecord;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2TextBox txtWalkingTimeTaken;
        private Guna.UI2.WinForms.Guna2TextBox txtWalkingDistance;
        private Guna.UI2.WinForms.Guna2TextBox txtWalkingSteps;
    }
}
