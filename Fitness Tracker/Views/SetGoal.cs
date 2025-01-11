using Fitness_Tracker.dao;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fitness_Tracker.Views
{
    public partial class frmSetGoal : UserControl
    {
        private ConnectionDB db;
        public frmSetGoal()
        {
            InitializeComponent();
            LoadCurrentGoal();
            LoadGoalsToGrid();
            LoadAchievedGoals(); // Initialize achieved goals label
            ClearGoalDisplay();
            dgvGoals.RowTemplate.Height = 45; // Adjust this value to your desired height
            dgvGoals.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }

        private void LoadGoalsToGrid()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                // Fetch goals from the database
                DataTable dt = db.GetGoals(frmLogin.person.PersonID);

                if (dt != null && dt.Rows.Count > 0)
                {
                    dgvGoals.Rows.Clear(); // Clear existing rows

                    int rowNumber = 1; // Start numbering from 1

                    foreach (DataRow row in dt.Rows)
                    {
                        dgvGoals.Rows.Add(
                            $"{rowNumber++}",           // colNo
                            row["goal_id"],               // colGoalId
                            row["goal_type"],             // colGoalType
                            row["target_weight"],         // colTargetWeight
                            row["daily_calories_target"], // colDailyCaloriesTarget
                            Convert.ToDateTime(row["created_at"]).ToString("yyyy-MM-dd"), // colCreatedAt
                            Convert.ToBoolean(row["is_achieved"]) ? "Yes" : "No" // colIsAchieved
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading goals: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db?.CloseConnection();
            }
        }
        private void LoadCurrentGoal()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                lblUserWeight.Text = $"Your Current Weight: {frmLogin.person.Weight} kg";

                DataTable dt = db.GetGoals(frmLogin.person.PersonID);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow goal = dt.Rows[0];
                    lblGoalType.Text = $"{goal["goal_type"]}";
                    lblTargetWeight.Text = $"{goal["target_weight"]} kg";
                    lblCaloriesTarget.Text = $"{goal["daily_calories_target"]} cal";
                    lblCreatedDate.Text = $"{Convert.ToDateTime(goal["created_at"]).ToString("yyyy-MM-dd")}";
                    lblIsAchieved.Text = $"{(Convert.ToBoolean(goal["is_achieved"]) ? "Yes" : "No")}";

                    btnAchieved.Enabled = !Convert.ToBoolean(goal["is_achieved"]);
                }
                else
                {
                    ClearGoalDisplay();
                }

                LoadAchievedGoals(); // Ensure achieved goals count is refreshed
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading current goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db?.CloseConnection();
            }
        }


        private void LoadAchievedGoals()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                // Fetch counts for each goal type
                int weightLossAchieved = db.GetAchievedGoalsCountByType(frmLogin.person.PersonID, "Weight Loss");
                int weightGainAchieved = db.GetAchievedGoalsCountByType(frmLogin.person.PersonID, "Weight Gain");
                int maintainWeightAchieved = db.GetAchievedGoalsCountByType(frmLogin.person.PersonID, "Maintain Weight");

                // Update labels
                lblWeightLossAchieved.Text = $"Weight Loss Achieved: {weightLossAchieved}";
                lblWeightGainedAchieved.Text = $"Weight Gain Achieved: {weightGainAchieved}";
                lblMaintainWeightAchieved.Text = $"Maintain Weight Achieved: {maintainWeightAchieved}";

                int totalAchieved = weightLossAchieved + weightGainAchieved + maintainWeightAchieved;
                lblAchievedGoal.Text = $"Achieved Goals: {totalAchieved}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading achieved goals: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db?.CloseConnection();
            }
        }

        private void ClearGoalDisplay()
        {
            lblGoalType.Text = " N/A";
            lblTargetWeight.Text = " N/A";
            lblCaloriesTarget.Text = " N/A";
            lblCreatedDate.Text = " N/A";
            lblIsAchieved.Text = " N/A";
            lblGoalAdvice.Text = "No goal set. Please set a goal.";


            btnAchieved.Enabled = false;
        }

        private void btnSetGoal_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure a goal type is selected
                if (cboGoalType.SelectedItem == null)
                {
                    MessageBox.Show("Please select a goal type.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string goalType = cboGoalType.SelectedItem.ToString();

                // Ensure valid target weight
                if (!double.TryParse(txtTargetWeight.Text, out double targetWeight) || targetWeight <= 0)
                {
                    MessageBox.Show("Please enter a valid target weight.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Ensure valid daily calorie target
                if (!double.TryParse(txtCaloriesTarget.Text, out double dailyCaloriesTarget) || dailyCaloriesTarget <= 0)
                {
                    MessageBox.Show("Please enter a valid daily calorie target.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate based on goal type
                double currentWeight = frmLogin.person.Weight;

                if (goalType == "Weight Loss" && targetWeight >= currentWeight)
                {
                    MessageBox.Show("Target weight must be less than your current weight for Weight Loss.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (goalType == "Weight Gain" && targetWeight <= currentWeight)
                {
                    MessageBox.Show("Target weight must be greater than your current weight for Weight Gain.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Insert the goal into the database
                db = new ConnectionDB();
                db.OpenConnection();

                if (db.InsertGoal(frmLogin.person.PersonID, goalType, targetWeight, dailyCaloriesTarget))
                {
                    MessageBox.Show("Goal saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGoalsToGrid(); // Refresh the goals list
                    LoadCurrentGoal(); // Refresh the current goal display
                }
                else
                {
                    MessageBox.Show("Failed to save the goal.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db?.CloseConnection();
            }
        }

        private void btnEditGoal_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure a goal type is selected
                if (cboGoalType.SelectedItem == null)
                {
                    MessageBox.Show("Please select a goal type.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string goalType = cboGoalType.SelectedItem.ToString().Trim();

                // Ensure valid target weight
                if (!double.TryParse(txtTargetWeight.Text, out double targetWeight) || targetWeight <= 0)
                {
                    MessageBox.Show("Please enter a valid target weight.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Ensure valid daily calorie target
                if (!double.TryParse(txtCaloriesTarget.Text, out double dailyCaloriesTarget) || dailyCaloriesTarget <= 0)
                {
                    MessageBox.Show("Please enter a valid daily calorie target.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate based on goal type
                double currentWeight = frmLogin.person.Weight;

                if (goalType == "Weight Loss" && targetWeight >= currentWeight)
                {
                    MessageBox.Show("Target weight must be less than your current weight for Weight Loss.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (goalType == "Weight Gain" && targetWeight <= currentWeight)
                {
                    MessageBox.Show("Target weight must be greater than your current weight for Weight Gain.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Update the goal in the database
                if (dgvGoals.SelectedRows.Count > 0)
                {
                    int goalId = Convert.ToInt32(dgvGoals.SelectedRows[0].Cells["colGoalId"].Value);

                    db = new ConnectionDB();
                    db.OpenConnection();

                    if (db.UpdateGoal(goalId, goalType, targetWeight, dailyCaloriesTarget))
                    {
                        MessageBox.Show("Goal updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadGoalsToGrid(); // Refresh the goals list
                        LoadCurrentGoal(); // Refresh the current goal display
                    }
                    else
                    {
                        MessageBox.Show("Failed to update the goal.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a goal to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db?.CloseConnection();
            }
        }

        private void bntDeleteGoal_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvGoals.SelectedRows.Count > 0)
                {
                    int goalId = Convert.ToInt32(dgvGoals.SelectedRows[0].Cells["colGoalId"].Value);

                    db = new ConnectionDB();
                    db.OpenConnection();

                    if (db.DeleteGoal(goalId))
                    {
                        MessageBox.Show("Goal deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Refresh the current goal display and DataGridView
                        LoadCurrentGoal();
                        LoadGoalsToGrid();
                        LoadAchievedGoals();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete the goal.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a goal to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db?.CloseConnection();
            }
        }

        private void btnAchieved_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvGoals.SelectedRows.Count > 0)
                {
                    int goalID = Convert.ToInt32(dgvGoals.SelectedRows[0].Cells["colGoalId"].Value);

                    db = new ConnectionDB();
                    db.OpenConnection();

                    if (db.MarkGoalAsAchieved(goalID))
                    {
                        MessageBox.Show("Goal marked as achieved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadCurrentGoal();
                        LoadGoalsToGrid();
                        LoadAchievedGoals(); // Refresh the achieved goals count
                    }
                    else
                    {
                        MessageBox.Show("Failed to mark the goal as achieved.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a goal to mark as achieved.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error marking goal as achieved: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db?.CloseConnection();
            }
        }

        private void dgvGoals_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvGoals.SelectedRows.Count > 0)
            {
                var selectedRow = dgvGoals.SelectedRows[0];

                // Update labels based on the selected row
                lblGoalType.Text = $"Goal Type: {selectedRow.Cells["colGoalType"].Value}";
                lblTargetWeight.Text = $"Target Weight: {selectedRow.Cells["colTargetWeight"].Value} kg";
                lblCaloriesTarget.Text = $"Calories Target: {selectedRow.Cells["colDailyCaloriesTarget"].Value} cal";
                lblCreatedDate.Text = $"Created At: {selectedRow.Cells["colCreatedAt"].Value}";
                lblIsAchieved.Text = $"Achieved: {selectedRow.Cells["colIsAchieved"].Value}";

                // Enable Achieve button only if the goal is not already achieved
                btnAchieved.Enabled = selectedRow.Cells["colIsAchieved"].Value.ToString() != "Yes";
            }
            else
            {
                ClearGoalDisplay();
            }
        }

        private void frmSetGoal_Load(object sender, EventArgs e)
        {

        }
    }
}
