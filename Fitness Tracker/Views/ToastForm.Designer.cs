namespace Fitness_Tracker.Views
{
    partial class frmToastForm
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
            this.panelBadgeColor = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelBadgeColor
            // 
            this.panelBadgeColor.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelBadgeColor.Location = new System.Drawing.Point(0, 0);
            this.panelBadgeColor.Name = "panelBadgeColor";
            this.panelBadgeColor.Size = new System.Drawing.Size(10, 100);
            this.panelBadgeColor.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Century Gothic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(104, 11);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(161, 17);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Achievement Unlocked!";
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(107, 42);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(257, 40);
            this.lblMessage.TabIndex = 2;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Fitness_Tracker.Properties.Resources.check_1_;
            this.pictureBox1.Location = new System.Drawing.Point(27, 25);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(50, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // frmToastForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(378, 100);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.panelBadgeColor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmToastForm";
            this.Text = "ToastForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelBadgeColor;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}