namespace Fitness_Tracker.Views
{
    partial class frmSchedule
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewSchedule = new Guna.UI2.WinForms.Guna2DataGridView();
            this.colScheduledId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActivity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colScheduleDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colScheduleStartTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDuration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cboActivity = new Guna.UI2.WinForms.Guna2ComboBox();
            this.dtpScheduleDate = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.btnMakeSchedule = new Guna.UI2.WinForms.Guna2Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtActivityDuration = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtActivityStartTime = new Guna.UI2.WinForms.Guna2TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSchedule)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewSchedule
            // 
            this.dataGridViewSchedule.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(200)))), ((int)(((byte)(207)))));
            this.dataGridViewSchedule.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewSchedule.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewSchedule.BackgroundColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewSchedule.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewSchedule.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dataGridViewSchedule.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colScheduledId,
            this.colNO,
            this.colActivity,
            this.colScheduleDate,
            this.colScheduleStartTime,
            this.colDuration,
            this.colDelete});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(133)))), ((int)(((byte)(147)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewSchedule.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewSchedule.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(199)))), ((int)(((byte)(206)))));
            this.dataGridViewSchedule.Location = new System.Drawing.Point(467, 87);
            this.dataGridViewSchedule.Name = "dataGridViewSchedule";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.RoyalBlue;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.MidnightBlue;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewSchedule.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewSchedule.RowHeadersVisible = false;
            this.dataGridViewSchedule.RowHeadersWidth = 51;
            this.dataGridViewSchedule.RowTemplate.Height = 50;
            this.dataGridViewSchedule.Size = new System.Drawing.Size(819, 702);
            this.dataGridViewSchedule.TabIndex = 0;
            this.dataGridViewSchedule.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.WetAsphalt;
            this.dataGridViewSchedule.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(200)))), ((int)(((byte)(207)))));
            this.dataGridViewSchedule.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dataGridViewSchedule.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dataGridViewSchedule.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dataGridViewSchedule.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dataGridViewSchedule.ThemeStyle.BackColor = System.Drawing.Color.Silver;
            this.dataGridViewSchedule.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(199)))), ((int)(((byte)(206)))));
            this.dataGridViewSchedule.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.dataGridViewSchedule.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridViewSchedule.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewSchedule.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dataGridViewSchedule.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dataGridViewSchedule.ThemeStyle.HeaderStyle.Height = 23;
            this.dataGridViewSchedule.ThemeStyle.ReadOnly = false;
            this.dataGridViewSchedule.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.dataGridViewSchedule.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dataGridViewSchedule.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewSchedule.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.Black;
            this.dataGridViewSchedule.ThemeStyle.RowsStyle.Height = 50;
            this.dataGridViewSchedule.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(133)))), ((int)(((byte)(147)))));
            this.dataGridViewSchedule.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridViewSchedule.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewSchedule_CellContentClick);
            // 
            // colScheduledId
            // 
            this.colScheduledId.HeaderText = "Schedule Id";
            this.colScheduledId.MinimumWidth = 6;
            this.colScheduledId.Name = "colScheduledId";
            this.colScheduledId.ReadOnly = true;
            this.colScheduledId.Visible = false;
            // 
            // colNO
            // 
            this.colNO.HeaderText = "No";
            this.colNO.MinimumWidth = 6;
            this.colNO.Name = "colNO";
            this.colNO.ReadOnly = true;
            // 
            // colActivity
            // 
            this.colActivity.HeaderText = "Activity";
            this.colActivity.MinimumWidth = 6;
            this.colActivity.Name = "colActivity";
            this.colActivity.ReadOnly = true;
            // 
            // colScheduleDate
            // 
            this.colScheduleDate.HeaderText = "Date";
            this.colScheduleDate.MinimumWidth = 6;
            this.colScheduleDate.Name = "colScheduleDate";
            this.colScheduleDate.ReadOnly = true;
            // 
            // colScheduleStartTime
            // 
            this.colScheduleStartTime.HeaderText = "Start Time";
            this.colScheduleStartTime.MinimumWidth = 6;
            this.colScheduleStartTime.Name = "colScheduleStartTime";
            this.colScheduleStartTime.ReadOnly = true;
            // 
            // colDuration
            // 
            this.colDuration.HeaderText = "Duration";
            this.colDuration.MinimumWidth = 6;
            this.colDuration.Name = "colDuration";
            this.colDuration.ReadOnly = true;
            // 
            // colDelete
            // 
            this.colDelete.HeaderText = "Delete";
            this.colDelete.MinimumWidth = 6;
            this.colDelete.Name = "colDelete";
            this.colDelete.Text = "Delete";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(55, 227);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 21);
            this.label4.TabIndex = 54;
            this.label4.Text = "Date:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(55, 352);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 21);
            this.label3.TabIndex = 53;
            this.label3.Text = "Start Time:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(55, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 21);
            this.label2.TabIndex = 52;
            this.label2.Text = "Activity:";
            // 
            // cboActivity
            // 
            this.cboActivity.BackColor = System.Drawing.Color.Transparent;
            this.cboActivity.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.cboActivity.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboActivity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboActivity.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cboActivity.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cboActivity.Font = new System.Drawing.Font("Century Gothic", 10.2F);
            this.cboActivity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cboActivity.ItemHeight = 30;
            this.cboActivity.Location = new System.Drawing.Point(59, 139);
            this.cboActivity.Name = "cboActivity";
            this.cboActivity.Size = new System.Drawing.Size(339, 36);
            this.cboActivity.TabIndex = 55;
            // 
            // dtpScheduleDate
            // 
            this.dtpScheduleDate.BorderRadius = 5;
            this.dtpScheduleDate.Checked = true;
            this.dtpScheduleDate.FillColor = System.Drawing.Color.DarkGray;
            this.dtpScheduleDate.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpScheduleDate.Format = System.Windows.Forms.DateTimePickerFormat.Long;
            this.dtpScheduleDate.Location = new System.Drawing.Point(59, 251);
            this.dtpScheduleDate.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dtpScheduleDate.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtpScheduleDate.Name = "dtpScheduleDate";
            this.dtpScheduleDate.Size = new System.Drawing.Size(337, 48);
            this.dtpScheduleDate.TabIndex = 56;
            this.dtpScheduleDate.Value = new System.DateTime(2024, 12, 31, 21, 38, 13, 14);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(55, 472);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 21);
            this.label1.TabIndex = 59;
            this.label1.Text = "Duration:";
            // 
            // btnMakeSchedule
            // 
            this.btnMakeSchedule.BorderRadius = 5;
            this.btnMakeSchedule.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnMakeSchedule.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnMakeSchedule.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnMakeSchedule.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnMakeSchedule.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.btnMakeSchedule.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMakeSchedule.ForeColor = System.Drawing.Color.White;
            this.btnMakeSchedule.Location = new System.Drawing.Point(140, 603);
            this.btnMakeSchedule.Name = "btnMakeSchedule";
            this.btnMakeSchedule.Size = new System.Drawing.Size(161, 64);
            this.btnMakeSchedule.TabIndex = 60;
            this.btnMakeSchedule.Text = "Make Schedule";
            this.btnMakeSchedule.Click += new System.EventHandler(this.btnMakeSchedule_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Century Gothic", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(803, 36);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(146, 22);
            this.label5.TabIndex = 61;
            this.label5.Text = "Schedule Table";
            // 
            // txtActivityDuration
            // 
            this.txtActivityDuration.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.txtActivityDuration.BorderThickness = 2;
            this.txtActivityDuration.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtActivityDuration.DefaultText = "";
            this.txtActivityDuration.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtActivityDuration.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtActivityDuration.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtActivityDuration.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtActivityDuration.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtActivityDuration.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtActivityDuration.ForeColor = System.Drawing.Color.Black;
            this.txtActivityDuration.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtActivityDuration.IconLeft = global::Fitness_Tracker.Properties.Resources.hourglass;
            this.txtActivityDuration.IconLeftSize = new System.Drawing.Size(25, 25);
            this.txtActivityDuration.Location = new System.Drawing.Point(59, 497);
            this.txtActivityDuration.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtActivityDuration.Name = "txtActivityDuration";
            this.txtActivityDuration.PasswordChar = '\0';
            this.txtActivityDuration.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtActivityDuration.PlaceholderText = "Enter the Activity\'s Duration";
            this.txtActivityDuration.SelectedText = "";
            this.txtActivityDuration.Size = new System.Drawing.Size(339, 48);
            this.txtActivityDuration.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtActivityDuration.TabIndex = 58;
            // 
            // txtActivityStartTime
            // 
            this.txtActivityStartTime.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.txtActivityStartTime.BorderThickness = 2;
            this.txtActivityStartTime.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtActivityStartTime.DefaultText = "";
            this.txtActivityStartTime.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtActivityStartTime.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtActivityStartTime.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtActivityStartTime.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtActivityStartTime.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtActivityStartTime.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtActivityStartTime.ForeColor = System.Drawing.Color.Black;
            this.txtActivityStartTime.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtActivityStartTime.IconLeft = global::Fitness_Tracker.Properties.Resources.calendar;
            this.txtActivityStartTime.IconLeftSize = new System.Drawing.Size(25, 25);
            this.txtActivityStartTime.Location = new System.Drawing.Point(59, 377);
            this.txtActivityStartTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtActivityStartTime.Name = "txtActivityStartTime";
            this.txtActivityStartTime.PasswordChar = '\0';
            this.txtActivityStartTime.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtActivityStartTime.PlaceholderText = "Enter the Activity\'s Start Time";
            this.txtActivityStartTime.SelectedText = "";
            this.txtActivityStartTime.Size = new System.Drawing.Size(339, 48);
            this.txtActivityStartTime.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtActivityStartTime.TabIndex = 57;
            // 
            // frmSchedule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnMakeSchedule);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtActivityDuration);
            this.Controls.Add(this.txtActivityStartTime);
            this.Controls.Add(this.dtpScheduleDate);
            this.Controls.Add(this.cboActivity);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridViewSchedule);
            this.Name = "frmSchedule";
            this.Size = new System.Drawing.Size(1337, 900);
            this.Load += new System.EventHandler(this.frmSchedule_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSchedule)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2DataGridView dataGridViewSchedule;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private Guna.UI2.WinForms.Guna2ComboBox cboActivity;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpScheduleDate;
        private Guna.UI2.WinForms.Guna2TextBox txtActivityStartTime;
        private Guna.UI2.WinForms.Guna2TextBox txtActivityDuration;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2Button btnMakeSchedule;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridViewTextBoxColumn colScheduledId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNO;
        private System.Windows.Forms.DataGridViewTextBoxColumn colActivity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colScheduleDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colScheduleStartTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDuration;
        private System.Windows.Forms.DataGridViewButtonColumn colDelete;
    }
}
