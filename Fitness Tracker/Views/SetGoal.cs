using Fitness_Tracker.dao;
using Guna.Charts.WinForms;
using Guna.UI2.WinForms;
using LiveCharts.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Fitness_Tracker.Views
{
    public partial class frmSetGoal : System.Windows.Forms.UserControl
    {
        private ConnectionDB db;
        public frmSetGoal()
        {
            InitializeComponent();
            LoadCurrentGoal();
            dtpTargetDate.MinDate = DateTime.Today;
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
                            $"{rowNumber++}",
                            row["goal_id"],
                            row["goal_type"],
                            row["target_weight"],
                            row["daily_calories_target"],
                            Convert.ToDateTime(row["created_at"]).ToString("yyyy-MM-dd"),
                            Convert.ToDateTime(row["target_date"]).ToString("yyyy-MM-dd"),
                            Convert.ToBoolean(row["is_achieved"]) ? "Yes" : "No",
                            row["activity_name"] // Display the activity name instead of ID
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

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No current goal found for this user.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearGoalDisplay();
                    return;
                }

                // Assuming the first row is the current goal
                DataRow goal = dt.Rows[0];

                lblGoalType.Text = $"Goal Type: {goal["goal_type"]}";
                lblTargetWeight.Text = $"Target Weight: {goal["target_weight"]} kg";
                lblCaloriesTarget.Text = $"Calories Target: {goal["daily_calories_target"]} cal";
                lblCreatedDate.Text = $"Created Date: {Convert.ToDateTime(goal["created_at"]).ToString("yyyy-MM-dd")}";
                lblIsAchieved.Text = $"Is Achieved: {(Convert.ToBoolean(goal["is_achieved"]) ? "Yes" : "No")}";
                lblTargetDate.Text = $"Target Date: {(goal["target_date"] != DBNull.Value ? Convert.ToDateTime(goal["target_date"]).ToString("yyyy-MM-dd") : "N/A")}";
                lblActivity.Text = $"Activity: {goal["activity_name"]}";

                btnAchieved.Enabled = !Convert.ToBoolean(goal["is_achieved"]);
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

        private void UpdateProgressBar()
        {
            double progress = 0;
            double weightProgress = 0;
            double timeProgress = 0;

            // Baseline weight when the goal was created
            double baselineWeight = 100; // Example value, replace with actual baseline
            double currentWeight = frmLogin.person.Weight;
            double targetWeight;

            DateTime createdAt;
            DateTime targetDate;

            // Parse CreatedAt and TargetDate
            if (DateTime.TryParse(lblCreatedDate.Text.Replace("Created At: ", ""), out createdAt) &&
                DateTime.TryParse(lblTargetDate.Text.Replace("Target Date: ", ""), out targetDate))
            {
                // Calculate time-based progress
                double totalDays = (targetDate - createdAt).TotalDays;
                double elapsedDays = (DateTime.Now - createdAt).TotalDays;
                timeProgress = (elapsedDays / totalDays) * 100;
            }

            // Ensure time progress is within bounds
            timeProgress = Math.Max(0, Math.Min(100, timeProgress));

            // Parse target weight from the label
            if (double.TryParse(lblTargetWeight.Text.Replace("Target Weight: ", "").Replace(" kg", ""), out targetWeight))
            {
                if (lblIsAchieved.Text.Contains("Yes"))
                {
                    // If the goal is already achieved, set weight progress to 100%
                    weightProgress = 100;
                }
                else if (lblGoalType.Text.Contains("Weight Loss") && baselineWeight > targetWeight)
                {
                    // Weight Loss progress
                    double weightLost = baselineWeight - currentWeight;
                    double totalToLose = baselineWeight - targetWeight;
                    weightProgress = (weightLost / totalToLose) * 100;
                }
                else if (lblGoalType.Text.Contains("Weight Gain") && baselineWeight < targetWeight)
                {
                    // Weight Gain progress
                    double weightGained = currentWeight - baselineWeight;
                    double totalToGain = targetWeight - baselineWeight;
                    weightProgress = (weightGained / totalToGain) * 100;
                }
                else if (lblGoalType.Text.Contains("Maintain Weight"))
                {
                    // Maintain Weight always at 100% progress
                    weightProgress = 100;
                }
            }

            // Ensure weight progress is within bounds
            weightProgress = Math.Max(0, Math.Min(100, weightProgress));

            // Combine weight progress and time progress (optional: take an average or weight them differently)
            progress = (weightProgress + timeProgress) / 2;

            // Update the Progress Bar and Label
            progressBar.Value = (int)progress;
            lblProgress.Text = $"Progress: {progress:F2}%";
            // Set Gradient Colors Based on Achievement/Progress
            if (lblIsAchieved.Text.Contains("Yes"))
            {
                progressBar.ProgressColor = Color.Green;
                progressBar.ProgressColor2 = Color.LimeGreen; // Lighter shade for gradient
                progressBar.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            }
            else if (progress > 0)
            {
                progressBar.ProgressColor = Color.Blue;
                progressBar.ProgressColor2 = Color.LightBlue; // Gradient for ongoing progress
                progressBar.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            }
            else
            {
                progressBar.ProgressColor = Color.Gray;
                progressBar.ProgressColor2 = Color.DarkGray; // Gradient for no progress
                progressBar.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            }

        }

        private void LoadTargetActivityOptions()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                // Fetch activities as a dictionary
                Dictionary<int, string> activities = db.GetActivities();

                if (activities != null && activities.Count > 0)
                {
                    cboTargetActivity.Items.Clear();
                    foreach (var activity in activities)
                    {
                        cboTargetActivity.Items.Add(new ComboBoxItem
                        {
                            Text = activity.Value,
                            Value = activity.Key
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading activities: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db?.CloseConnection();
            }
        }

        private class ComboBoxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }


        private void ClearGoalDisplay()
        {
            lblGoalType.Text = "Goal Type: N/A";
            lblTargetWeight.Text = "Target Weight: N/A";
            lblCaloriesTarget.Text = "Daily Calorie Target: N/A";
            lblCreatedDate.Text = "Created Date: N/A";
            lblIsAchieved.Text = "Achieved: N/A";
            lblGoalAdvice.Text = "No goal set. Please set a goal.";
            lblTargetDate.Text = "Target Date: N/A";
            lblActivity.Text = "Activity: N/A";

            btnAchieved.Enabled = false;
        }

        private void btnSetGoal_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboGoalType.SelectedItem == null)
                {
                    MessageBox.Show("Please select a goal type.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboTargetActivity.SelectedItem == null)
                {
                    MessageBox.Show("Please select a target activity.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string goalType = cboGoalType.SelectedItem.ToString();
                string activityName = cboTargetActivity.SelectedItem.ToString();

                if (!double.TryParse(txtTargetWeight.Text, out double targetWeight) || targetWeight <= 0)
                {
                    MessageBox.Show("Please enter a valid target weight.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtCaloriesTarget.Text, out double dailyCaloriesTarget) || dailyCaloriesTarget <= 0)
                {
                    MessageBox.Show("Please enter a valid daily calorie target.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dtpTargetDate.Value.Date <= DateTime.Now.Date)
                {
                    MessageBox.Show("Target date must be a future date.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

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

                db = new ConnectionDB();
                db.OpenConnection();

                // Get activity_id from activity_name
                int activityId = db.GetActivityIdByName(activityName);

                if (activityId == -1)
                {
                    MessageBox.Show("Invalid target activity.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (db.InsertGoal(frmLogin.person.PersonID, goalType, targetWeight, dailyCaloriesTarget, dtpTargetDate.Value, activityId))
                {
                    MessageBox.Show("Goal saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGoalsToGrid();
                    LoadCurrentGoal();
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
                if (cboGoalType.SelectedItem == null)
                {
                    MessageBox.Show("Please select a goal type.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboTargetActivity.SelectedItem == null)
                {
                    MessageBox.Show("Please select a target activity.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dgvGoals.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a goal to edit.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string goalType = cboGoalType.SelectedItem.ToString();
                string activityName = cboTargetActivity.SelectedItem.ToString();

                if (!double.TryParse(txtTargetWeight.Text, out double targetWeight) || targetWeight <= 0)
                {
                    MessageBox.Show("Please enter a valid target weight.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtCaloriesTarget.Text, out double dailyCaloriesTarget) || dailyCaloriesTarget <= 0)
                {
                    MessageBox.Show("Please enter a valid daily calorie target.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dtpTargetDate.Value.Date <= DateTime.Now.Date)
                {
                    MessageBox.Show("Target date must be a future date.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                db = new ConnectionDB();
                db.OpenConnection();

                // Get activity_id from activity_name
                int activityId = db.GetActivityIdByName(activityName);

                if (activityId == -1)
                {
                    MessageBox.Show("Invalid target activity.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int goalId = Convert.ToInt32(dgvGoals.SelectedRows[0].Cells["colGoalId"].Value);

                if (db.UpdateGoal(goalId, goalType, targetWeight, dailyCaloriesTarget, dtpTargetDate.Value, activityId))
                {
                    MessageBox.Show("Goal updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGoalsToGrid();
                    LoadCurrentGoal();
                }
                else
                {
                    MessageBox.Show("Failed to update the goal.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    // Get the selected goal ID
                    int goalId = Convert.ToInt32(dgvGoals.SelectedRows[0].Cells["colGoalId"].Value);

                    db = new ConnectionDB();
                    db.OpenConnection();

                    // Delete the goal from the database
                    if (db.DeleteGoal(goalId))
                    {
                        MessageBox.Show("Goal deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Remove the selected row from the DataGridView
                        dgvGoals.Rows.RemoveAt(dgvGoals.SelectedRows[0].Index);

                        // Update the current goal display and progress
                        LoadCurrentGoal();
                        UpdateProgressBar();
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
                    double targetWeight = Convert.ToDouble(dgvGoals.SelectedRows[0].Cells["colTargetWeight"].Value);

                    db = new ConnectionDB();
                    db.OpenConnection();

                    // Update the goal as achieved and set the achieved date
                    if (db.MarkGoalAsAchieved(goalID))
                    {
                        // Update the user's weight after marking the goal as achieved
                        if (db.UpdateUserWeight(frmLogin.person.PersonID, targetWeight))
                        {
                            frmLogin.person.Weight = targetWeight; // Update the in-memory weight
                            MessageBox.Show("Goal marked as achieved and weight updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadCurrentGoal();
                            LoadGoalsToGrid();
                            LoadAchievedGoals();
                            UpdateProgressBar(); // Refresh the progress bar
                        }
                        else
                        {
                            MessageBox.Show("Failed to update user's weight.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
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
                lblTargetDate.Text =$"Target Date: {selectedRow.Cells["colTargetDate"].Value}"; 

                // Determine goal advice based on the user's current weight
                double currentWeight = frmLogin.person.Weight;
                double targetWeight = Convert.ToDouble(selectedRow.Cells["colTargetWeight"].Value);

                // Enable Achieve button only if the goal is not already achieved
                btnAchieved.Enabled = selectedRow.Cells["colIsAchieved"].Value.ToString() != "Yes";
                // Update lblGoalAdvice dynamically
                UpdateGoalAdvice(selectedRow);
                UpdateProgressBar();    
            }
            else
            {
                // Reset to default state if no row is selected
                ClearGoalDisplay();
            }
        }
        private void UpdateGoalAdvice(DataGridViewRow selectedRow)
        {
            try
            {
                string goalType = selectedRow.Cells["colGoalType"].Value.ToString();
                double currentWeight = frmLogin.person.Weight;
                double targetWeight = Convert.ToDouble(selectedRow.Cells["colTargetWeight"].Value);

                if (goalType == "Weight Loss")
                {
                    double weightToLose = currentWeight - targetWeight;
                    lblGoalAdvice.Text = $"You need to lose {weightToLose} kg to reach your goal.";
                }
                else if (goalType == "Weight Gain")
                {
                    double weightToGain = targetWeight - currentWeight;
                    lblGoalAdvice.Text = $"You need to gain {weightToGain} kg to reach your goal.";
                }
                else if (goalType == "Maintain Weight")
                {
                    lblGoalAdvice.Text = "Your goal is to maintain your current weight.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error calculating goal advice: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadGoalAchievementByMonthGraph()
        {
            try
            {
                db.OpenConnection(); // Ensure connection is opened
                DataTable dt = db.GetGoalAchievementByMonth();

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Clear existing data points
                    gunaBarDataset1.DataPoints.Clear();

                    // Populate the dataset with data
                    foreach (DataRow row in dt.Rows)
                    {
                        string month = row["achievement_month"].ToString(); // Example: "January 2025"
                        int count = Convert.ToInt32(row["achieved_count"]);

                        // Add data point to the dataset
                        gunaBarDataset1.DataPoints.Add(month, count);
                    }

                    // Optionally customize the dataset's appearance
                    gunaBarDataset1.Label = "Goals Achieved";
                }

                // Add the dataset to the chart (if not already added)
                if (!chartGoalAchievementByMonth.Datasets.Contains(gunaBarDataset1))
                {
                    chartGoalAchievementByMonth.Datasets.Add(gunaBarDataset1);
                }

                // Optional: Customize the chart's title
                chartGoalAchievementByMonth.Title.Text = "Goal Achievement by Month";
                chartGoalAchievementByMonth.Update(); // Refresh the chart
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading goal achievement by month: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection(); // Ensure connection is closed
            }
        }

        private void LoadAchievedVsPendingGraph()
        {
            try
            {
                db.OpenConnection(); // Open connection
                DataTable dt = db.GetAchievedAndPendingGoals(frmLogin.person.PersonID);

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Fetch counts from the DataTable
                    int achievedCount = Convert.ToInt32(dt.Rows[0]["achieved_count"]);
                    int pendingCount = Convert.ToInt32(dt.Rows[0]["pending_count"]);

                    // Clear existing data points
                    gunaPieDataset1.DataPoints.Clear();

                    // Add data points
                    gunaPieDataset1.DataPoints.Add("Achieved", achievedCount);
                    gunaPieDataset1.DataPoints.Add("Pending", pendingCount);

                    // Customize dataset
                    gunaPieDataset1.Label = "Goals";

                    // Optional: Customize chart title
                    chartAchievedVsPending.Title.Text = "Achieved vs Pending Goals";
                }
                else
                {
                    MessageBox.Show("No data found for Achieved vs Pending Goals.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Achieved vs Pending Goals chart: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection(); // Close connection
            }
        }



        private void frmSetGoal_Load(object sender, EventArgs e)
        {
            LoadGoalsToGrid();
            LoadTargetActivityOptions();
            LoadGoalAchievementByMonthGraph();
            LoadAchievedVsPendingGraph();
            dgvGoals.ClearSelection();
            UpdateProgressBar(); // Ensure progress bar is updated on load
            ClearGoalDisplay();
            // Populate Target Activity if a Goal Type is pre-selected
            if (cboGoalType.SelectedItem != null)
            {
                cboGoalType_SelectedIndexChanged(null, null);
            }
        }

        // Define a dictionary for recommended activities based on goal types
        private readonly Dictionary<string, List<string>> activityRecommendations = new Dictionary<string, List<string>>
        {
                { "Weight Loss", new List<string> { "Walking", "Running", "Cycling" ,"Rowing"} },
                { "Weight Gain", new List<string> { "Weightlifting" } },
                { "Maintain Weight", new List<string> { "Rowing", "Hiking" } }
        };

        private void cboGoalType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected goal type
            string selectedGoalType = cboGoalType.SelectedItem?.ToString();

            // Clear the existing items in Target Activity
            cboTargetActivity.Items.Clear();

            if (!string.IsNullOrEmpty(selectedGoalType) && activityRecommendations.ContainsKey(selectedGoalType))
            {
                // Populate Target Activity dropdown with the recommended activities
                foreach (string activity in activityRecommendations[selectedGoalType])
                {
                    cboTargetActivity.Items.Add(activity);
                }
            }
            else
            {
                // Optional: Provide a default message or disable the dropdown
                cboTargetActivity.Items.Add("No activities available for this goal type");
            }

            cboTargetActivity.SelectedIndex = 0; // Select the first item by default
        }
    }
}
