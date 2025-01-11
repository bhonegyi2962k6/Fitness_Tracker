namespace Fitness_Tracker.Views
{
    partial class frmSetGoal
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
            this.cboGoalType = new Guna.UI2.WinForms.Guna2ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTargetWeight = new Guna.UI2.WinForms.Guna2TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCaloriesTarget = new Guna.UI2.WinForms.Guna2TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSetGoal = new Guna.UI2.WinForms.Guna2Button();
            this.btnEditGoal = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.bntDeleteGoal = new Guna.UI2.WinForms.Guna2Button();
            this.btnAchieved = new Guna.UI2.WinForms.Guna2Button();
            this.lblGoalAdvice = new System.Windows.Forms.Label();
            this.lblIsAchieved = new System.Windows.Forms.Label();
            this.lblCreatedDate = new System.Windows.Forms.Label();
            this.lblCaloriesTarget = new System.Windows.Forms.Label();
            this.lblTargetWeight = new System.Windows.Forms.Label();
            this.lblGoalType = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dgvGoals = new Guna.UI2.WinForms.Guna2DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGoalId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGoalType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTargetWeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDailyCaloriesTarget = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCreatedAt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIsAchieved = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblAchievedGoal = new System.Windows.Forms.Label();
            this.lblUserWeight = new System.Windows.Forms.Label();
            this.guna2Panel2 = new Guna.UI2.WinForms.Guna2Panel();
            this.lblMaintainWeightAchieved = new System.Windows.Forms.Label();
            this.lblWeightGainedAchieved = new System.Windows.Forms.Label();
            this.lblWeightLossAchieved = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.guna2Panel3 = new Guna.UI2.WinForms.Guna2Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.guna2Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGoals)).BeginInit();
            this.guna2Panel2.SuspendLayout();
            this.guna2Panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboGoalType
            // 
            this.cboGoalType.BackColor = System.Drawing.Color.Transparent;
            this.cboGoalType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.cboGoalType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboGoalType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGoalType.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cboGoalType.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cboGoalType.Font = new System.Drawing.Font("Century Gothic", 10.2F);
            this.cboGoalType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cboGoalType.ItemHeight = 30;
            this.cboGoalType.Items.AddRange(new object[] {
            "Weight Loss",
            "Weight Gain",
            "Maintain Weight"});
            this.cboGoalType.Location = new System.Drawing.Point(18, 87);
            this.cboGoalType.Name = "cboGoalType";
            this.cboGoalType.Size = new System.Drawing.Size(253, 36);
            this.cboGoalType.TabIndex = 56;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(17, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 21);
            this.label2.TabIndex = 57;
            this.label2.Text = "Goal Type:";
            // 
            // txtTargetWeight
            // 
            this.txtTargetWeight.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.txtTargetWeight.BorderThickness = 2;
            this.txtTargetWeight.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtTargetWeight.DefaultText = "";
            this.txtTargetWeight.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtTargetWeight.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtTargetWeight.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtTargetWeight.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtTargetWeight.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtTargetWeight.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTargetWeight.ForeColor = System.Drawing.Color.Black;
            this.txtTargetWeight.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtTargetWeight.IconLeft = global::Fitness_Tracker.Properties.Resources.calendar;
            this.txtTargetWeight.IconLeftSize = new System.Drawing.Size(25, 25);
            this.txtTargetWeight.Location = new System.Drawing.Point(18, 185);
            this.txtTargetWeight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtTargetWeight.Name = "txtTargetWeight";
            this.txtTargetWeight.PasswordChar = '\0';
            this.txtTargetWeight.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtTargetWeight.PlaceholderText = "Enter Your Target Weight";
            this.txtTargetWeight.SelectedText = "";
            this.txtTargetWeight.Size = new System.Drawing.Size(256, 48);
            this.txtTargetWeight.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtTargetWeight.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(17, 160);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 21);
            this.label1.TabIndex = 59;
            this.label1.Text = "Target Weight:";
            // 
            // txtCaloriesTarget
            // 
            this.txtCaloriesTarget.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.txtCaloriesTarget.BorderThickness = 2;
            this.txtCaloriesTarget.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCaloriesTarget.DefaultText = "";
            this.txtCaloriesTarget.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtCaloriesTarget.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtCaloriesTarget.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCaloriesTarget.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCaloriesTarget.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCaloriesTarget.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCaloriesTarget.ForeColor = System.Drawing.Color.Black;
            this.txtCaloriesTarget.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCaloriesTarget.IconLeft = global::Fitness_Tracker.Properties.Resources.calendar;
            this.txtCaloriesTarget.IconLeftSize = new System.Drawing.Size(25, 25);
            this.txtCaloriesTarget.Location = new System.Drawing.Point(18, 290);
            this.txtCaloriesTarget.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtCaloriesTarget.Name = "txtCaloriesTarget";
            this.txtCaloriesTarget.PasswordChar = '\0';
            this.txtCaloriesTarget.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtCaloriesTarget.PlaceholderText = "Enter Your Calories Target";
            this.txtCaloriesTarget.SelectedText = "";
            this.txtCaloriesTarget.Size = new System.Drawing.Size(256, 48);
            this.txtCaloriesTarget.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtCaloriesTarget.TabIndex = 60;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(17, 265);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 21);
            this.label3.TabIndex = 61;
            this.label3.Text = "Calories Target:";
            // 
            // btnSetGoal
            // 
            this.btnSetGoal.BorderRadius = 5;
            this.btnSetGoal.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnSetGoal.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnSetGoal.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnSetGoal.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnSetGoal.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.btnSetGoal.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSetGoal.ForeColor = System.Drawing.Color.White;
            this.btnSetGoal.Location = new System.Drawing.Point(21, 418);
            this.btnSetGoal.Name = "btnSetGoal";
            this.btnSetGoal.Size = new System.Drawing.Size(122, 46);
            this.btnSetGoal.TabIndex = 62;
            this.btnSetGoal.Text = "Set Goal";
            this.btnSetGoal.Click += new System.EventHandler(this.btnSetGoal_Click);
            // 
            // btnEditGoal
            // 
            this.btnEditGoal.BorderRadius = 5;
            this.btnEditGoal.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnEditGoal.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnEditGoal.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnEditGoal.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnEditGoal.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.btnEditGoal.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditGoal.ForeColor = System.Drawing.Color.White;
            this.btnEditGoal.Location = new System.Drawing.Point(152, 418);
            this.btnEditGoal.Name = "btnEditGoal";
            this.btnEditGoal.Size = new System.Drawing.Size(122, 46);
            this.btnEditGoal.TabIndex = 63;
            this.btnEditGoal.Text = "Edit";
            this.btnEditGoal.Click += new System.EventHandler(this.btnEditGoal_Click);
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.guna2Panel1.BorderRadius = 10;
            this.guna2Panel1.BorderThickness = 2;
            this.guna2Panel1.Controls.Add(this.bntDeleteGoal);
            this.guna2Panel1.Controls.Add(this.btnAchieved);
            this.guna2Panel1.Controls.Add(this.lblGoalAdvice);
            this.guna2Panel1.Controls.Add(this.lblIsAchieved);
            this.guna2Panel1.Controls.Add(this.lblCreatedDate);
            this.guna2Panel1.Controls.Add(this.lblCaloriesTarget);
            this.guna2Panel1.Controls.Add(this.lblTargetWeight);
            this.guna2Panel1.Controls.Add(this.lblGoalType);
            this.guna2Panel1.Controls.Add(this.label4);
            this.guna2Panel1.Location = new System.Drawing.Point(338, 433);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Size = new System.Drawing.Size(389, 391);
            this.guna2Panel1.TabIndex = 64;
            // 
            // bntDeleteGoal
            // 
            this.bntDeleteGoal.BorderRadius = 5;
            this.bntDeleteGoal.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.bntDeleteGoal.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.bntDeleteGoal.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.bntDeleteGoal.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.bntDeleteGoal.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.bntDeleteGoal.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bntDeleteGoal.ForeColor = System.Drawing.Color.White;
            this.bntDeleteGoal.Location = new System.Drawing.Point(204, 323);
            this.bntDeleteGoal.Name = "bntDeleteGoal";
            this.bntDeleteGoal.Size = new System.Drawing.Size(167, 46);
            this.bntDeleteGoal.TabIndex = 65;
            this.bntDeleteGoal.Text = "Delete Goal";
            this.bntDeleteGoal.Click += new System.EventHandler(this.bntDeleteGoal_Click);
            // 
            // btnAchieved
            // 
            this.btnAchieved.BorderRadius = 5;
            this.btnAchieved.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnAchieved.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnAchieved.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnAchieved.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnAchieved.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.btnAchieved.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAchieved.ForeColor = System.Drawing.Color.White;
            this.btnAchieved.Location = new System.Drawing.Point(21, 323);
            this.btnAchieved.Name = "btnAchieved";
            this.btnAchieved.Size = new System.Drawing.Size(167, 46);
            this.btnAchieved.TabIndex = 65;
            this.btnAchieved.Text = "Achieved Goal";
            this.btnAchieved.Click += new System.EventHandler(this.btnAchieved_Click);
            // 
            // lblGoalAdvice
            // 
            this.lblGoalAdvice.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGoalAdvice.Location = new System.Drawing.Point(29, 268);
            this.lblGoalAdvice.Name = "lblGoalAdvice";
            this.lblGoalAdvice.Size = new System.Drawing.Size(318, 21);
            this.lblGoalAdvice.TabIndex = 70;
            // 
            // lblIsAchieved
            // 
            this.lblIsAchieved.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIsAchieved.Location = new System.Drawing.Point(29, 230);
            this.lblIsAchieved.Name = "lblIsAchieved";
            this.lblIsAchieved.Size = new System.Drawing.Size(318, 21);
            this.lblIsAchieved.TabIndex = 69;
            this.lblIsAchieved.Text = "Is Achieved:";
            // 
            // lblCreatedDate
            // 
            this.lblCreatedDate.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCreatedDate.Location = new System.Drawing.Point(29, 188);
            this.lblCreatedDate.Name = "lblCreatedDate";
            this.lblCreatedDate.Size = new System.Drawing.Size(318, 21);
            this.lblCreatedDate.TabIndex = 68;
            this.lblCreatedDate.Text = "Created Date:";
            // 
            // lblCaloriesTarget
            // 
            this.lblCaloriesTarget.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCaloriesTarget.Location = new System.Drawing.Point(29, 146);
            this.lblCaloriesTarget.Name = "lblCaloriesTarget";
            this.lblCaloriesTarget.Size = new System.Drawing.Size(318, 21);
            this.lblCaloriesTarget.TabIndex = 67;
            this.lblCaloriesTarget.Text = "Calories Target:";
            // 
            // lblTargetWeight
            // 
            this.lblTargetWeight.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTargetWeight.Location = new System.Drawing.Point(29, 103);
            this.lblTargetWeight.Name = "lblTargetWeight";
            this.lblTargetWeight.Size = new System.Drawing.Size(318, 21);
            this.lblTargetWeight.TabIndex = 66;
            this.lblTargetWeight.Text = "Target Weight:";
            // 
            // lblGoalType
            // 
            this.lblGoalType.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGoalType.Location = new System.Drawing.Point(29, 62);
            this.lblGoalType.Name = "lblGoalType";
            this.lblGoalType.Size = new System.Drawing.Size(318, 21);
            this.lblGoalType.TabIndex = 65;
            this.lblGoalType.Text = "Goal Type:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(29, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 19);
            this.label4.TabIndex = 65;
            this.label4.Text = "Current Goal";
            // 
            // dgvGoals
            // 
            this.dgvGoals.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(200)))), ((int)(((byte)(207)))));
            this.dgvGoals.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvGoals.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvGoals.BackgroundColor = System.Drawing.Color.Silver;
            this.dgvGoals.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvGoals.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvGoals.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgvGoals.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colGoalId,
            this.colGoalType,
            this.colTargetWeight,
            this.colDailyCaloriesTarget,
            this.colCreatedAt,
            this.colIsAchieved});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(133)))), ((int)(((byte)(147)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvGoals.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvGoals.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(199)))), ((int)(((byte)(206)))));
            this.dgvGoals.Location = new System.Drawing.Point(745, 178);
            this.dgvGoals.Name = "dgvGoals";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.RoyalBlue;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.MidnightBlue;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvGoals.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvGoals.RowHeadersVisible = false;
            this.dgvGoals.RowHeadersWidth = 51;
            this.dgvGoals.RowTemplate.Height = 50;
            this.dgvGoals.Size = new System.Drawing.Size(867, 646);
            this.dgvGoals.TabIndex = 65;
            this.dgvGoals.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.WetAsphalt;
            this.dgvGoals.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(200)))), ((int)(((byte)(207)))));
            this.dgvGoals.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dgvGoals.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dgvGoals.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dgvGoals.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dgvGoals.ThemeStyle.BackColor = System.Drawing.Color.Silver;
            this.dgvGoals.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(199)))), ((int)(((byte)(206)))));
            this.dgvGoals.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.dgvGoals.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvGoals.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvGoals.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dgvGoals.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgvGoals.ThemeStyle.HeaderStyle.Height = 23;
            this.dgvGoals.ThemeStyle.ReadOnly = false;
            this.dgvGoals.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.dgvGoals.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvGoals.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvGoals.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.Black;
            this.dgvGoals.ThemeStyle.RowsStyle.Height = 50;
            this.dgvGoals.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(133)))), ((int)(((byte)(147)))));
            this.dgvGoals.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvGoals.SelectionChanged += new System.EventHandler(this.dgvGoals_SelectionChanged);
            // 
            // colNo
            // 
            this.colNo.FillWeight = 33.22845F;
            this.colNo.HeaderText = "No";
            this.colNo.MinimumWidth = 6;
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            // 
            // colGoalId
            // 
            this.colGoalId.HeaderText = "Goal Id";
            this.colGoalId.MinimumWidth = 6;
            this.colGoalId.Name = "colGoalId";
            this.colGoalId.ReadOnly = true;
            this.colGoalId.Visible = false;
            // 
            // colGoalType
            // 
            this.colGoalType.FillWeight = 117.6287F;
            this.colGoalType.HeaderText = "Goal Type";
            this.colGoalType.MinimumWidth = 6;
            this.colGoalType.Name = "colGoalType";
            this.colGoalType.ReadOnly = true;
            // 
            // colTargetWeight
            // 
            this.colTargetWeight.FillWeight = 117.6287F;
            this.colTargetWeight.HeaderText = "Target Weight";
            this.colTargetWeight.MinimumWidth = 6;
            this.colTargetWeight.Name = "colTargetWeight";
            this.colTargetWeight.ReadOnly = true;
            // 
            // colDailyCaloriesTarget
            // 
            this.colDailyCaloriesTarget.FillWeight = 117.6287F;
            this.colDailyCaloriesTarget.HeaderText = "Daily Calories Target";
            this.colDailyCaloriesTarget.MinimumWidth = 6;
            this.colDailyCaloriesTarget.Name = "colDailyCaloriesTarget";
            this.colDailyCaloriesTarget.ReadOnly = true;
            // 
            // colCreatedAt
            // 
            this.colCreatedAt.FillWeight = 117.6287F;
            this.colCreatedAt.HeaderText = "Created At";
            this.colCreatedAt.MinimumWidth = 6;
            this.colCreatedAt.Name = "colCreatedAt";
            this.colCreatedAt.ReadOnly = true;
            // 
            // colIsAchieved
            // 
            this.colIsAchieved.FillWeight = 96.25668F;
            this.colIsAchieved.HeaderText = "Is Achieved";
            this.colIsAchieved.MinimumWidth = 6;
            this.colIsAchieved.Name = "colIsAchieved";
            this.colIsAchieved.ReadOnly = true;
            // 
            // lblAchievedGoal
            // 
            this.lblAchievedGoal.AutoSize = true;
            this.lblAchievedGoal.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAchievedGoal.Location = new System.Drawing.Point(17, 168);
            this.lblAchievedGoal.Name = "lblAchievedGoal";
            this.lblAchievedGoal.Size = new System.Drawing.Size(196, 21);
            this.lblAchievedGoal.TabIndex = 66;
            this.lblAchievedGoal.Text = "Achieved Total Goals:";
            // 
            // lblUserWeight
            // 
            this.lblUserWeight.AutoSize = true;
            this.lblUserWeight.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserWeight.Location = new System.Drawing.Point(334, 107);
            this.lblUserWeight.Name = "lblUserWeight";
            this.lblUserWeight.Size = new System.Drawing.Size(116, 21);
            this.lblUserWeight.TabIndex = 67;
            this.lblUserWeight.Text = "Your Weight:";
            // 
            // guna2Panel2
            // 
            this.guna2Panel2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.guna2Panel2.BorderRadius = 10;
            this.guna2Panel2.BorderThickness = 2;
            this.guna2Panel2.Controls.Add(this.lblMaintainWeightAchieved);
            this.guna2Panel2.Controls.Add(this.lblWeightGainedAchieved);
            this.guna2Panel2.Controls.Add(this.lblWeightLossAchieved);
            this.guna2Panel2.Controls.Add(this.label5);
            this.guna2Panel2.Controls.Add(this.lblAchievedGoal);
            this.guna2Panel2.Location = new System.Drawing.Point(338, 178);
            this.guna2Panel2.Name = "guna2Panel2";
            this.guna2Panel2.Size = new System.Drawing.Size(389, 219);
            this.guna2Panel2.TabIndex = 71;
            // 
            // lblMaintainWeightAchieved
            // 
            this.lblMaintainWeightAchieved.AutoSize = true;
            this.lblMaintainWeightAchieved.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaintainWeightAchieved.Location = new System.Drawing.Point(17, 136);
            this.lblMaintainWeightAchieved.Name = "lblMaintainWeightAchieved";
            this.lblMaintainWeightAchieved.Size = new System.Drawing.Size(240, 21);
            this.lblMaintainWeightAchieved.TabIndex = 74;
            this.lblMaintainWeightAchieved.Text = "Maintain Weight Achieved:";
            // 
            // lblWeightGainedAchieved
            // 
            this.lblWeightGainedAchieved.AutoSize = true;
            this.lblWeightGainedAchieved.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWeightGainedAchieved.Location = new System.Drawing.Point(17, 102);
            this.lblWeightGainedAchieved.Name = "lblWeightGainedAchieved";
            this.lblWeightGainedAchieved.Size = new System.Drawing.Size(229, 21);
            this.lblWeightGainedAchieved.TabIndex = 73;
            this.lblWeightGainedAchieved.Text = "Weight Gained Achieved:";
            // 
            // lblWeightLossAchieved
            // 
            this.lblWeightLossAchieved.AutoSize = true;
            this.lblWeightLossAchieved.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWeightLossAchieved.Location = new System.Drawing.Point(17, 71);
            this.lblWeightLossAchieved.Name = "lblWeightLossAchieved";
            this.lblWeightLossAchieved.Size = new System.Drawing.Size(199, 21);
            this.lblWeightLossAchieved.TabIndex = 72;
            this.lblWeightLossAchieved.Text = "Weight Loss Achieved:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(17, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(135, 19);
            this.label5.TabIndex = 71;
            this.label5.Text = "Achieved Goal";
            // 
            // guna2Panel3
            // 
            this.guna2Panel3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(53)))), ((int)(((byte)(85)))));
            this.guna2Panel3.BorderRadius = 10;
            this.guna2Panel3.BorderThickness = 2;
            this.guna2Panel3.Controls.Add(this.label9);
            this.guna2Panel3.Controls.Add(this.btnEditGoal);
            this.guna2Panel3.Controls.Add(this.cboGoalType);
            this.guna2Panel3.Controls.Add(this.label2);
            this.guna2Panel3.Controls.Add(this.txtTargetWeight);
            this.guna2Panel3.Controls.Add(this.label1);
            this.guna2Panel3.Controls.Add(this.btnSetGoal);
            this.guna2Panel3.Controls.Add(this.txtCaloriesTarget);
            this.guna2Panel3.Controls.Add(this.label3);
            this.guna2Panel3.Location = new System.Drawing.Point(19, 178);
            this.guna2Panel3.Name = "guna2Panel3";
            this.guna2Panel3.Size = new System.Drawing.Size(301, 646);
            this.guna2Panel3.TabIndex = 75;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(14, 13);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(94, 19);
            this.label9.TabIndex = 71;
            this.label9.Text = "Set a Goal";
            // 
            // frmSetGoal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.guna2Panel3);
            this.Controls.Add(this.guna2Panel2);
            this.Controls.Add(this.lblUserWeight);
            this.Controls.Add(this.dgvGoals);
            this.Controls.Add(this.guna2Panel1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "frmSetGoal";
            this.Size = new System.Drawing.Size(1637, 900);
            this.Load += new System.EventHandler(this.frmSetGoal_Load);
            this.guna2Panel1.ResumeLayout(false);
            this.guna2Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGoals)).EndInit();
            this.guna2Panel2.ResumeLayout(false);
            this.guna2Panel2.PerformLayout();
            this.guna2Panel3.ResumeLayout(false);
            this.guna2Panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2ComboBox cboGoalType;
        private System.Windows.Forms.Label label2;
        private Guna.UI2.WinForms.Guna2TextBox txtTargetWeight;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2TextBox txtCaloriesTarget;
        private System.Windows.Forms.Label label3;
        private Guna.UI2.WinForms.Guna2Button btnSetGoal;
        private Guna.UI2.WinForms.Guna2Button btnEditGoal;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private System.Windows.Forms.Label lblGoalType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblTargetWeight;
        private System.Windows.Forms.Label lblIsAchieved;
        private System.Windows.Forms.Label lblCreatedDate;
        private System.Windows.Forms.Label lblCaloriesTarget;
        private Guna.UI2.WinForms.Guna2Button bntDeleteGoal;
        private Guna.UI2.WinForms.Guna2Button btnAchieved;
        private System.Windows.Forms.Label lblGoalAdvice;
        private Guna.UI2.WinForms.Guna2DataGridView dgvGoals;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGoalId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGoalType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTargetWeight;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDailyCaloriesTarget;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCreatedAt;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIsAchieved;
        private System.Windows.Forms.Label lblAchievedGoal;
        private System.Windows.Forms.Label lblUserWeight;
        private System.Windows.Forms.Label label5;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel2;
        private System.Windows.Forms.Label lblMaintainWeightAchieved;
        private System.Windows.Forms.Label lblWeightGainedAchieved;
        private System.Windows.Forms.Label lblWeightLossAchieved;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel3;
        private System.Windows.Forms.Label label9;
    }
}
