namespace Fitness_Tracker.Views
{
    partial class frmMonitorActivity
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
            this.dataGridViewRecord = new Guna.UI2.WinForms.Guna2DataGridView();
            this.colRecordId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActivity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRecordDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBurnedCalories = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActivityDetails = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActvityType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRecordDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.label5 = new System.Windows.Forms.Label();
            this.lblTotalCalories = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRecord)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewRecord
            // 
            this.dataGridViewRecord.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(200)))), ((int)(((byte)(207)))));
            this.dataGridViewRecord.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewRecord.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewRecord.BackgroundColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewRecord.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewRecord.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dataGridViewRecord.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRecordId,
            this.colNO,
            this.colActivity,
            this.colRecordDate,
            this.colBurnedCalories,
            this.colActivityDetails,
            this.colActvityType,
            this.colRecordDelete});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(133)))), ((int)(((byte)(147)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewRecord.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewRecord.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(199)))), ((int)(((byte)(206)))));
            this.dataGridViewRecord.Location = new System.Drawing.Point(41, 46);
            this.dataGridViewRecord.Name = "dataGridViewRecord";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.RoyalBlue;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.MidnightBlue;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewRecord.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewRecord.RowHeadersVisible = false;
            this.dataGridViewRecord.RowHeadersWidth = 51;
            this.dataGridViewRecord.RowTemplate.Height = 30;
            this.dataGridViewRecord.Size = new System.Drawing.Size(1271, 701);
            this.dataGridViewRecord.TabIndex = 1;
            this.dataGridViewRecord.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.WetAsphalt;
            this.dataGridViewRecord.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(200)))), ((int)(((byte)(207)))));
            this.dataGridViewRecord.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dataGridViewRecord.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dataGridViewRecord.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dataGridViewRecord.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dataGridViewRecord.ThemeStyle.BackColor = System.Drawing.Color.Silver;
            this.dataGridViewRecord.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(199)))), ((int)(((byte)(206)))));
            this.dataGridViewRecord.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.dataGridViewRecord.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridViewRecord.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewRecord.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dataGridViewRecord.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dataGridViewRecord.ThemeStyle.HeaderStyle.Height = 23;
            this.dataGridViewRecord.ThemeStyle.ReadOnly = false;
            this.dataGridViewRecord.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.dataGridViewRecord.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dataGridViewRecord.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewRecord.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.Black;
            this.dataGridViewRecord.ThemeStyle.RowsStyle.Height = 30;
            this.dataGridViewRecord.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(133)))), ((int)(((byte)(147)))));
            this.dataGridViewRecord.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridViewRecord.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewRecord_CellContentClick);
            // 
            // colRecordId
            // 
            this.colRecordId.HeaderText = "Record Id";
            this.colRecordId.MinimumWidth = 6;
            this.colRecordId.Name = "colRecordId";
            this.colRecordId.ReadOnly = true;
            this.colRecordId.Visible = false;
            // 
            // colNO
            // 
            this.colNO.FillWeight = 42.72886F;
            this.colNO.HeaderText = "No";
            this.colNO.MinimumWidth = 6;
            this.colNO.Name = "colNO";
            this.colNO.ReadOnly = true;
            // 
            // colActivity
            // 
            this.colActivity.FillWeight = 64.17112F;
            this.colActivity.HeaderText = "Activity";
            this.colActivity.MinimumWidth = 6;
            this.colActivity.Name = "colActivity";
            this.colActivity.ReadOnly = true;
            // 
            // colRecordDate
            // 
            this.colRecordDate.FillWeight = 80.14235F;
            this.colRecordDate.HeaderText = "Record Date";
            this.colRecordDate.MinimumWidth = 6;
            this.colRecordDate.Name = "colRecordDate";
            this.colRecordDate.ReadOnly = true;
            // 
            // colBurnedCalories
            // 
            this.colBurnedCalories.FillWeight = 103.2042F;
            this.colBurnedCalories.HeaderText = "Burned Calories";
            this.colBurnedCalories.MinimumWidth = 6;
            this.colBurnedCalories.Name = "colBurnedCalories";
            // 
            // colActivityDetails
            // 
            this.colActivityDetails.FillWeight = 236.9363F;
            this.colActivityDetails.HeaderText = "Activity Details";
            this.colActivityDetails.MinimumWidth = 6;
            this.colActivityDetails.Name = "colActivityDetails";
            // 
            // colActvityType
            // 
            this.colActvityType.HeaderText = "Activity Type";
            this.colActvityType.MinimumWidth = 6;
            this.colActvityType.Name = "colActvityType";
            // 
            // colRecordDelete
            // 
            this.colRecordDelete.FillWeight = 72.81707F;
            this.colRecordDelete.HeaderText = "Delete Record";
            this.colRecordDelete.MinimumWidth = 6;
            this.colRecordDelete.Name = "colRecordDelete";
            this.colRecordDelete.Text = "";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Century Gothic", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(660, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 22);
            this.label5.TabIndex = 62;
            this.label5.Text = "Record";
            // 
            // lblTotalCalories
            // 
            this.lblTotalCalories.AutoSize = true;
            this.lblTotalCalories.Font = new System.Drawing.Font("Century Gothic", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalCalories.Location = new System.Drawing.Point(47, 794);
            this.lblTotalCalories.Name = "lblTotalCalories";
            this.lblTotalCalories.Size = new System.Drawing.Size(138, 22);
            this.lblTotalCalories.TabIndex = 63;
            this.lblTotalCalories.Text = "Total Calories: ";
            // 
            // frmMonitorActivity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblTotalCalories);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dataGridViewRecord);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "frmMonitorActivity";
            this.Size = new System.Drawing.Size(1337, 900);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRecord)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2DataGridView dataGridViewRecord;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblTotalCalories;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRecordId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNO;
        private System.Windows.Forms.DataGridViewTextBoxColumn colActivity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRecordDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBurnedCalories;
        private System.Windows.Forms.DataGridViewTextBoxColumn colActivityDetails;
        private System.Windows.Forms.DataGridViewTextBoxColumn colActvityType;
        private System.Windows.Forms.DataGridViewButtonColumn colRecordDelete;
    }
}
