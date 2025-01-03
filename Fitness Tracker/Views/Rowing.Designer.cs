namespace Fitness_Tracker.Views
{
    partial class frmRowing
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnRowingRecord = new Guna.UI2.WinForms.Guna2Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRowingTimeTaken = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtRowingDistance = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtRowingStrokes = new Guna.UI2.WinForms.Guna2TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Fitness_Tracker.Properties.Resources.Rowing_man;
            this.pictureBox1.Location = new System.Drawing.Point(68, 183);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(344, 339);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 53;
            this.pictureBox1.TabStop = false;
            // 
            // btnRowingRecord
            // 
            this.btnRowingRecord.BorderRadius = 5;
            this.btnRowingRecord.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnRowingRecord.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnRowingRecord.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnRowingRecord.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnRowingRecord.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.btnRowingRecord.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRowingRecord.ForeColor = System.Drawing.Color.White;
            this.btnRowingRecord.Location = new System.Drawing.Point(735, 558);
            this.btnRowingRecord.Name = "btnRowingRecord";
            this.btnRowingRecord.Size = new System.Drawing.Size(122, 46);
            this.btnRowingRecord.TabIndex = 52;
            this.btnRowingRecord.Text = "Record";
            this.btnRowingRecord.Click += new System.EventHandler(this.btnRowingRecord_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(456, 338);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 21);
            this.label4.TabIndex = 51;
            this.label4.Text = "Distance:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(456, 464);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 21);
            this.label3.TabIndex = 50;
            this.label3.Text = "Time Taken:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(456, 225);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 21);
            this.label2.TabIndex = 49;
            this.label2.Text = "Total Strokes:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(430, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 34);
            this.label1.TabIndex = 48;
            this.label1.Text = "Rowing";
            // 
            // txtRowingTimeTaken
            // 
            this.txtRowingTimeTaken.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.txtRowingTimeTaken.BorderThickness = 2;
            this.txtRowingTimeTaken.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtRowingTimeTaken.DefaultText = "";
            this.txtRowingTimeTaken.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtRowingTimeTaken.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtRowingTimeTaken.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtRowingTimeTaken.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtRowingTimeTaken.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtRowingTimeTaken.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRowingTimeTaken.ForeColor = System.Drawing.Color.Black;
            this.txtRowingTimeTaken.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtRowingTimeTaken.IconLeft = global::Fitness_Tracker.Properties.Resources.hourglass;
            this.txtRowingTimeTaken.IconLeftSize = new System.Drawing.Size(25, 25);
            this.txtRowingTimeTaken.Location = new System.Drawing.Point(650, 437);
            this.txtRowingTimeTaken.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtRowingTimeTaken.Name = "txtRowingTimeTaken";
            this.txtRowingTimeTaken.PasswordChar = '\0';
            this.txtRowingTimeTaken.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtRowingTimeTaken.PlaceholderText = "Enter Your Rowed Time";
            this.txtRowingTimeTaken.SelectedText = "";
            this.txtRowingTimeTaken.Size = new System.Drawing.Size(321, 48);
            this.txtRowingTimeTaken.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtRowingTimeTaken.TabIndex = 47;
            // 
            // txtRowingDistance
            // 
            this.txtRowingDistance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.txtRowingDistance.BorderThickness = 2;
            this.txtRowingDistance.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtRowingDistance.DefaultText = "";
            this.txtRowingDistance.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtRowingDistance.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtRowingDistance.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtRowingDistance.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtRowingDistance.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtRowingDistance.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRowingDistance.ForeColor = System.Drawing.Color.Black;
            this.txtRowingDistance.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtRowingDistance.IconLeft = global::Fitness_Tracker.Properties.Resources.distance;
            this.txtRowingDistance.IconLeftSize = new System.Drawing.Size(25, 25);
            this.txtRowingDistance.Location = new System.Drawing.Point(650, 311);
            this.txtRowingDistance.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtRowingDistance.Name = "txtRowingDistance";
            this.txtRowingDistance.PasswordChar = '\0';
            this.txtRowingDistance.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtRowingDistance.PlaceholderText = "Enter Your Rowed Distance";
            this.txtRowingDistance.SelectedText = "";
            this.txtRowingDistance.Size = new System.Drawing.Size(321, 48);
            this.txtRowingDistance.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtRowingDistance.TabIndex = 46;
            // 
            // txtRowingStrokes
            // 
            this.txtRowingStrokes.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.txtRowingStrokes.BorderThickness = 2;
            this.txtRowingStrokes.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtRowingStrokes.DefaultText = "";
            this.txtRowingStrokes.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtRowingStrokes.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtRowingStrokes.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtRowingStrokes.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtRowingStrokes.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtRowingStrokes.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRowingStrokes.ForeColor = System.Drawing.Color.Black;
            this.txtRowingStrokes.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtRowingStrokes.IconLeft = global::Fitness_Tracker.Properties.Resources.man_in_canoe;
            this.txtRowingStrokes.IconLeftSize = new System.Drawing.Size(25, 25);
            this.txtRowingStrokes.Location = new System.Drawing.Point(650, 198);
            this.txtRowingStrokes.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtRowingStrokes.Name = "txtRowingStrokes";
            this.txtRowingStrokes.PasswordChar = '\0';
            this.txtRowingStrokes.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtRowingStrokes.PlaceholderText = "Enter Your Rowing Total Strokes";
            this.txtRowingStrokes.SelectedText = "";
            this.txtRowingStrokes.Size = new System.Drawing.Size(321, 48);
            this.txtRowingStrokes.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtRowingStrokes.TabIndex = 45;
            // 
            // frmRowing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnRowingRecord);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtRowingTimeTaken);
            this.Controls.Add(this.txtRowingDistance);
            this.Controls.Add(this.txtRowingStrokes);
            this.Name = "frmRowing";
            this.Size = new System.Drawing.Size(1037, 700);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private Guna.UI2.WinForms.Guna2Button btnRowingRecord;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2TextBox txtRowingTimeTaken;
        private Guna.UI2.WinForms.Guna2TextBox txtRowingDistance;
        private Guna.UI2.WinForms.Guna2TextBox txtRowingStrokes;
    }
}
