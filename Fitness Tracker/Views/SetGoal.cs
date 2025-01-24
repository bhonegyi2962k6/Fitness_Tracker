using Fitness_Tracker.dao;
using Fitness_Tracker.Entities;
using Guna.Charts.WinForms;
using Guna.UI2.WinForms;
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
        private readonly ConnectionDB db;
        public frmSetGoal()
        {
            InitializeComponent();
            db = ConnectionDB.GetInstance(); // Use the Singleton instance
            LoadCurrentGoal();
            dtpTargetDate.MinDate = DateTime.Today;
            dgvGoals.RowTemplate.Height = 45; // Adjust this value to your desired height
            dgvGoals.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }
        private void LoadGoalsToGrid()
        {
            try
            {
                // Fetch goals from the database
                DataTable dt = db.GetGoals(frmLogin.user.PersonID);

                if (dt != null && dt.Rows.Count > 0)
                {
                    dgvGoals.Rows.Clear(); // Clear existing rows

                    int rowNumber = 1;

                    foreach (DataRow row in dt.Rows)
                    {
                        // Create an Activity object
                        Activity activity = new Activity
                        {
                            ActivityId = row["activity_name"] != DBNull.Value ? db.GetActivityIdByName(row["activity_name"].ToString()) : -1,
                            ActivityName = row["activity_name"] != DBNull.Value ? row["activity_name"].ToString() : "N/A",
                        };

                        // Create a GoalTracking object
                        GoalTracking goal = new GoalTracking
                        {
                            GoalId = Convert.ToInt32(row["goal_id"]),
                            GoalType = row["goal_type"].ToString(),
                            TargetWeight = Convert.ToDecimal(row["target_weight"]),
                            DailyCaloriesTarget = Convert.ToDouble(row["daily_calories_target"]),
                            IsAchieved = Convert.ToBoolean(row["is_achieved"]),
                            CreatedAt = Convert.ToDateTime(row["created_at"]),
                            TargetDate = Convert.ToDateTime(row["target_date"]),
                            Activity = activity // Link the Activity object
                        };

                        // Add the goal to the DataGridView
                        int rowIndex = dgvGoals.Rows.Add(
                            $"{rowNumber++}",
                            goal.GoalId,
                            goal.GoalType,
                            goal.TargetWeight,
                            goal.DailyCaloriesTarget,
                            goal.CreatedAt.ToString("yyyy-MM-dd"),
                            goal.TargetDate.ToString("yyyy-MM-dd"),
                            goal.IsAchieved ? "Yes" : "No",
                            goal.Activity.ActivityName
                        );

                        // Highlight row if the target date is within 3 days
                        if (!goal.IsAchieved && (goal.TargetDate - DateTime.Now).TotalDays <= 3)
                        {
                            dgvGoals.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Red;
                            dgvGoals.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.White;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading goals: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCurrentGoal()
        {
            try
            {
                if (frmLogin.user == null)
                {
                    MessageBox.Show("User not logged in. Please log in again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                lblUserWeight.Text = $"Your Current Weight: {frmLogin.user.Weight} kg";

                // Fetch the current goal data
                DataTable dt = db?.GetGoals(frmLogin.user.PersonID);
                if (dt == null || dt.Rows.Count == 0)
                {
                    ClearGoalDisplay();
                    return;
                }

                // Get the first goal row
                DataRow goal = dt.Rows[0];

                // Create Activity object from the goal data
                Activity activity = new Activity
                {
                    ActivityName = goal["activity_name"].ToString()
                };

                // Populate UI with data from the query and Activity object
                lblGoalType.Text = $"Goal Type: {goal["goal_type"]}";
                lblTargetWeight.Text = $"Target Weight: {goal["target_weight"]} kg";
                lblCaloriesTarget.Text = $"Calories Target: {goal["daily_calories_target"]} cal";
                lblCreatedDate.Text = $"Created Date: {Convert.ToDateTime(goal["created_at"]).ToString("yyyy-MM-dd")}";
                lblIsAchieved.Text = $"Is Achieved: {(Convert.ToBoolean(goal["is_achieved"]) ? "Yes" : "No")}";
                lblTargetDate.Text = $"Target Date: {(goal["target_date"] != DBNull.Value ? Convert.ToDateTime(goal["target_date"]).ToString("yyyy-MM-dd") : "N/A")}";
                lblActivity.Text = $"Activity: {activity.ActivityName}";

                // Enable or disable the "Achieved" button
                btnAchieved.Enabled = !Convert.ToBoolean(goal["is_achieved"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading current goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadAchievedGoals()
        {
            try
            {
                // Create WeightTracking objects for each goal type
                WeightTracking weightLossTracking = new WeightTracking
                {
                    Person = frmLogin.user, // Assign the logged-in user
                };

                WeightTracking weightGainTracking = new WeightTracking
                {
                    Person = frmLogin.user, // Assign the logged-in user
                };

                WeightTracking maintainWeightTracking = new WeightTracking
                {
                    Person = frmLogin.user, // Assign the logged-in user
                };

                // Fetch counts for each goal type
                weightLossTracking.Weight = db.GetAchievedGoalsCountByType(frmLogin.user.PersonID, "Weight Loss");
                weightGainTracking.Weight = db.GetAchievedGoalsCountByType(frmLogin.user.PersonID, "Weight Gain");
                maintainWeightTracking.Weight = db.GetAchievedGoalsCountByType(frmLogin.user.PersonID, "Maintain Weight");

                // Update labels using the WeightTracking objects
                lblWeightLossAchieved.Text = $"Weight Loss Achieved: {weightLossTracking.Weight}";
                lblWeightGainedAchieved.Text = $"Weight Gain Achieved: {weightGainTracking.Weight}";
                lblMaintainWeightAchieved.Text = $"Maintain Weight Achieved: {maintainWeightTracking.Weight}";

                int totalAchieved = (int)(weightLossTracking.Weight + weightGainTracking.Weight + maintainWeightTracking.Weight);
                lblAchievedGoal.Text = $"Achieved Goals: {totalAchieved}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading achieved goals: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateProgressBar()
        {
            double progress = 0;
            double weightProgress = 0;
            double timeProgress = 0;

            // Baseline weight when the goal was created
            double baselineWeight = 100; // Example value, replace with actual baseline
            double currentWeight = frmLogin.user.Weight;
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
                // Fetch activities as a dictionary
                Dictionary<int, string> activities = db.GetActivities();

                if (activities != null && activities.Count > 0)
                {
                    cboTargetActivity.Items.Clear();

                    // Populate ComboBox using Activity objects
                    foreach (var activityEntry in activities)
                    {
                        // Create an Activity object for each entry
                        Activity activity = new Activity
                        {
                            ActivityId = activityEntry.Key,
                            ActivityName = activityEntry.Value
                        };

                        // Add the activity to the ComboBox as a ComboBoxItem
                        cboTargetActivity.Items.Add(new ComboBoxItem
                        {
                            Text = activity.ActivityName,
                            Value = activity.ActivityId
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading activities: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void ClearGoalInputFields()
        {
            // Clear text boxes
            txtTargetWeight.Clear();
            txtCaloriesTarget.Clear();

            // Reset combo boxes
            cboGoalType.SelectedIndex = -1;
            cboTargetActivity.SelectedIndex = -1;

            // Reset date picker to today's date
            dtpTargetDate.Value = DateTime.Now;
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

                double currentWeight = frmLogin.user.Weight;

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

                // Get activity_id from activity_name
                int activityId = db.GetActivityIdByName(activityName);

                if (activityId == -1)
                {
                    MessageBox.Show("Invalid target activity.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create the Activity object
                Activity selectedActivity = new Activity
                {
                    ActivityId = activityId,
                    ActivityName = activityName
                };

                // Create the GoalTracking object
                GoalTracking goalTracking = new GoalTracking
                {
                    GoalType = goalType,
                    TargetWeight = (decimal)targetWeight,
                    DailyCaloriesTarget = dailyCaloriesTarget,
                    TargetDate = dtpTargetDate.Value,
                    CreatedAt = DateTime.Now,
                    IsAchieved = false, // Default to not achieved when setting a new goal
                    Person = frmLogin.user, // Assign the logged-in user
                    Activity = selectedActivity
                };

                // Insert the goal into the database
                if (db.InsertGoal(
                    goalTracking.Person.PersonID,
                    goalTracking.GoalType,
                    (double)goalTracking.TargetWeight,
                    goalTracking.DailyCaloriesTarget,
                    goalTracking.TargetDate,
                    goalTracking.Activity.ActivityId))
                {
                    MessageBox.Show("Goal saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh UI components
                    LoadGoalsToGrid();
                    LoadCurrentGoal();
                    UpdateProgressBar();
                    LoadGoalAchievementByMonthGraph();
                    LoadAchievedVsPendingGraph();
                    ClearGoalInputFields();
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
        }
        private void btnEditGoal_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
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

                // Create objects
                var user = User.GetInstance();
                string goalType = cboGoalType.SelectedItem.ToString();
                string activityName = cboTargetActivity.SelectedItem.ToString();

                // Get activity ID
                int activityId = db.GetActivityIdByName(activityName);
                if (activityId == -1)
                {
                    MessageBox.Show("Invalid target activity.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Retrieve the selected goal ID from the grid
                int goalId = Convert.ToInt32(dgvGoals.SelectedRows[0].Cells["colGoalId"].Value);

                // Create `Activity` and `GoalTracking` objects
                var activity = new Activity
                {
                    ActivityId = activityId,
                    ActivityName = activityName
                };

                var goal = new GoalTracking
                {
                    GoalId = goalId,
                    Person = user,
                    GoalType = goalType,
                    TargetWeight = (decimal)targetWeight,
                    DailyCaloriesTarget = dailyCaloriesTarget,
                    TargetDate = dtpTargetDate.Value,
                    Activity = activity
                };

                // Update the goal in the database
                if (db.UpdateGoal(goal.GoalId, goal.GoalType, (double)goal.TargetWeight, goal.DailyCaloriesTarget, goal.TargetDate, goal.Activity.ActivityId))
                {
                    MessageBox.Show("Goal updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh UI
                    LoadGoalsToGrid();
                    LoadCurrentGoal();
                    UpdateProgressBar();
                    LoadGoalAchievementByMonthGraph();
                    LoadAchievedVsPendingGraph();
                    LoadWeightTrendGraph();
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
        }
        private void bntDeleteGoal_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvGoals.SelectedRows.Count > 0)
                {
                    // Get the selected goal ID
                    int goalId = Convert.ToInt32(dgvGoals.SelectedRows[0].Cells["colGoalId"].Value);

                    // Get the "IsAchieved" value as a string
                    string isAchievedValue = dgvGoals.SelectedRows[0].Cells["colIsAchieved"].Value.ToString().ToLower();
                    bool isAchieved = false;

                    // Convert string to boolean
                    if (isAchievedValue == "true" || isAchievedValue == "yes" || isAchievedValue == "1")
                    {
                        isAchieved = true;
                    }
                    else if (isAchievedValue == "false" || isAchievedValue == "no" || isAchievedValue == "0")
                    {
                        isAchieved = false;
                    }
                    else
                    {
                        throw new InvalidCastException($"Invalid value for IsAchieved: {isAchievedValue}");
                    }

                    // Create the Activity object for the selected goal
                    Activity selectedActivity = new Activity
                    {
                        ActivityName = dgvGoals.SelectedRows[0].Cells["colActivity"].Value.ToString()
                    };

                    // Create the GoalTracking object
                    GoalTracking selectedGoal = new GoalTracking
                    {
                        GoalId = goalId,
                        Person = frmLogin.user, // Assuming the logged-in user is the person
                        GoalType = dgvGoals.SelectedRows[0].Cells["colGoalType"].Value.ToString(),
                        TargetWeight = Convert.ToDecimal(dgvGoals.SelectedRows[0].Cells["colTargetWeight"].Value),
                        DailyCaloriesTarget = Convert.ToDouble(dgvGoals.SelectedRows[0].Cells["colDailyCaloriesTarget"].Value),
                        IsAchieved = isAchieved,
                        CreatedAt = Convert.ToDateTime(dgvGoals.SelectedRows[0].Cells["colCreatedAt"].Value),
                        TargetDate = Convert.ToDateTime(dgvGoals.SelectedRows[0].Cells["colTargetDate"].Value),
                        Activity = selectedActivity
                    };

                    // Confirm deletion
                    DialogResult confirmResult = MessageBox.Show(
                        "Are you sure you want to delete this goal?",
                        "Confirm Delete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (confirmResult == DialogResult.Yes)
                    {
                        // Delete the goal from the database
                        if (db.DeleteGoal(goalId))
                        {
                            MessageBox.Show("Goal deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Remove the selected row from the DataGridView
                            dgvGoals.Rows.RemoveAt(dgvGoals.SelectedRows[0].Index);

                            // Update the current goal display and progress
                            LoadGoalsToGrid(); // Refresh the table to update numbering
                            LoadCurrentGoal();
                            LoadAchievedGoals();
                            UpdateProgressBar();
                            LoadGoalAchievementByMonthGraph();
                            LoadAchievedVsPendingGraph();
                            LoadWeightTrendGraph();
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete the goal.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
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
        }


        private void btnAchieved_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvGoals.SelectedRows.Count > 0)
                {
                    // Retrieve goal data from the selected row
                    int goalId = Convert.ToInt32(dgvGoals.SelectedRows[0].Cells["colGoalId"].Value);
                    double targetWeight = Convert.ToDouble(dgvGoals.SelectedRows[0].Cells["colTargetWeight"].Value);

                    // Create GoalTracking object
                    var selectedGoal = new GoalTracking
                    {
                        GoalId = goalId,
                        Person = frmLogin.user, // Reference the logged-in user
                        GoalType = dgvGoals.SelectedRows[0].Cells["colGoalType"].Value.ToString(),
                        TargetWeight = (decimal)targetWeight,
                        Activity = new Activity
                        {
                            ActivityName = dgvGoals.SelectedRows[0].Cells["colActivity"].Value.ToString()
                        },
                        IsAchieved = false, // Will be updated below
                        TargetDate = Convert.ToDateTime(dgvGoals.SelectedRows[0].Cells["colTargetDate"].Value)
                    };

                    // 1. Update the goal as achieved
                    if (db.MarkGoalAsAchieved(goalId))
                    {
                        selectedGoal.IsAchieved = true; // Update the status in the object
                        selectedGoal.AchievedDate = DateTime.Now; // Mark achievement date

                        // 2. Create WeightTracking object
                        var weightTracking = new WeightTracking
                        {
                            Person = frmLogin.user,
                            RecorededDate = DateTime.Now,
                            Weight = targetWeight
                        };

                        // Insert into weight_tracking
                        if (db.InsertWeightTracking(frmLogin.user.PersonID, targetWeight))
                        {
                            // 3. Update the person's current weight
                            if (db.UpdateUserWeight(frmLogin.user.PersonID, targetWeight))
                            {
                                frmLogin.user.Weight = targetWeight; // Update in-memory value
                                MessageBox.Show("Goal marked as achieved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Refresh UI components
                                LoadCurrentGoal();
                                LoadGoalsToGrid();
                                LoadAchievedGoals();
                                UpdateProgressBar();
                                LoadGoalAchievementByMonthGraph();
                                LoadAchievedVsPendingGraph();
                                LoadWeightTrendGraph();
                            }
                            else
                            {
                                MessageBox.Show("Failed to update user's weight.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Failed to log weight in weight tracking.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        }

        private void dgvGoals_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvGoals.SelectedRows.Count > 0)
            {
                var selectedRow = dgvGoals.SelectedRows[0];

                // Update labels based on the selected row
                lblGoalType.Text = $"Goal Type: {selectedRow.Cells["colGoalType"].Value}";
                lblTargetWeight.Text = $"Target Weight: {selectedRow.Cells["colTargetWeight"].Value} kg";
                lblCaloriesTarget.Text = $"Daily Calories Target: {selectedRow.Cells["colDailyCaloriesTarget"].Value} cal";
                lblCreatedDate.Text = $"Created At: {selectedRow.Cells["colCreatedAt"].Value}";
                lblIsAchieved.Text = $"Achieved: {selectedRow.Cells["colIsAchieved"].Value}";
                lblTargetDate.Text =$"Target Date: {selectedRow.Cells["colTargetDate"].Value}";
                lblActivity.Text = $"Actvity: {selectedRow.Cells["colActivity"].Value}";
                // Determine goal advice based on the user's current weight
                double currentWeight = frmLogin.user.Weight;
                double targetWeight = Convert.ToDouble(selectedRow.Cells["colTargetWeight"].Value);

                // Enable Achieve button only if the goal is not already achieved
                btnAchieved.Enabled = selectedRow.Cells["colIsAchieved"].Value.ToString() != "Yes";
                btnEditGoal.Enabled = selectedRow.Cells["colIsAchieved"].Value.ToString() != "Yes";
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
                double currentWeight = frmLogin.user.Weight;
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
                // Fetch data from the database
                DataTable dt = db.GetGoalAchievementByMonth();

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Clear existing data points
                    gunaBarDataset1.DataPoints.Clear();

                    // Iterate over the DataTable rows and populate the chart using GoalTracking objects
                    foreach (DataRow row in dt.Rows)
                    {
                        // Create a GoalTracking object
                        GoalTracking goalTracking = new GoalTracking
                        {
                            AchievedDate = DateTime.ParseExact(row["achievement_month"].ToString() + "-01", "yyyy-MM-dd", null),
                            IsAchieved = true
                        };

                        // Extract the achieved count
                        int achievedCount = Convert.ToInt32(row["achieved_count"]);

                        // Use the formatted date string as the label (e.g., "January 2025")
                        string monthLabel = goalTracking.AchievedDate.ToString("MMMM yyyy");

                        // Add data point to the dataset
                        gunaBarDataset1.DataPoints.Add(monthLabel, achievedCount);
                    }

                    // Customize the dataset's appearance
                    gunaBarDataset1.Label = "Goals Achieved";

                    // Add the dataset to the chart (if not already added)
                    if (!chartGoalAchievementByMonth.Datasets.Contains(gunaBarDataset1))
                    {
                        chartGoalAchievementByMonth.Datasets.Add(gunaBarDataset1);
                    }

                    // Customize the chart's title
                    chartGoalAchievementByMonth.Title.Text = "Goal Achievement by Month";
                    chartGoalAchievementByMonth.Update(); // Refresh the chart
                }
                else
                {
                    // Handle the case where no data is returned
                    MessageBox.Show("No data available for goal achievement by month.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading goal achievement by month: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadAchievedVsPendingGraph()
        {
            try
            {
                // Fetch the data from the database
                DataTable dt = db.GetAchievedAndPendingGoals(frmLogin.user.PersonID);

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Create GoalTracking objects to represent achieved and pending goals
                    var achievedGoals = new GoalTracking
                    {
                        IsAchieved = true,
                        GoalType = "Achieved Goals",
                        Person = frmLogin.user,
                        TargetDate = DateTime.Today // Placeholder, if needed
                    };

                    var pendingGoals = new GoalTracking
                    {
                        IsAchieved = false,
                        GoalType = "Pending Goals",
                        Person = frmLogin.user,
                        TargetDate = DateTime.Today // Placeholder, if needed
                    };

                    // Fetch counts from the DataTable
                    achievedGoals.GoalId = Convert.ToInt32(dt.Rows[0]["achieved_count"]);
                    pendingGoals.GoalId = Convert.ToInt32(dt.Rows[0]["pending_count"]);

                    // Clear existing data points
                    gunaPieDataset1.DataPoints.Clear();

                    // Add data points to the chart
                    gunaPieDataset1.DataPoints.Add(achievedGoals.GoalType, achievedGoals.GoalId);
                    gunaPieDataset1.DataPoints.Add(pendingGoals.GoalType, pendingGoals.GoalId);

                    // Customize dataset
                    gunaPieDataset1.Label = "Goals";

                    // Optional: Customize chart title
                    chartAchievedVsPending.Title.Text = "Achieved vs Pending Goals";

                    // Refresh the chart
                    chartAchievedVsPending.Datasets.Clear();
                    chartAchievedVsPending.Datasets.Add(gunaPieDataset1);
                    chartAchievedVsPending.Update();
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
        }
        private void LoadWeightTrendGraph()
        {
            try
            {
                // Fetch the weight tracking data from the database
                DataTable dt = db.GetWeightTrackingData(frmLogin.user.PersonID);

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Clear existing datasets
                    gunaLineDataset1.DataPoints.Clear();

                    // Create a list to hold WeightTracking objects
                    var weightTrackingList = new List<WeightTracking>();

                    // Populate the WeightTracking objects and add data points
                    foreach (DataRow row in dt.Rows)
                    {
                        // Create a WeightTracking object
                        var weightTracking = new WeightTracking
                        {
                            Person = frmLogin.user,
                            RecorededDate = Convert.ToDateTime(row["recorded_date"]),
                            Weight = Convert.ToDouble(row["weight"])
                        };

                        // Add the object to the list
                        weightTrackingList.Add(weightTracking);

                        // Add the data point to the dataset
                        gunaLineDataset1.DataPoints.Add(weightTracking.RecorededDate.ToString("yyyy-MM-dd"), weightTracking.Weight);
                    }

                    // Configure dataset properties
                    gunaLineDataset1.Label = "Weight Trends";
                    gunaLineDataset1.BorderColor = Color.Blue;
                    gunaLineDataset1.BorderWidth = 2;

                    // Add the dataset to the chart
                    chartWeightTrend.Datasets.Clear();
                    chartWeightTrend.Datasets.Add(gunaLineDataset1);

                    // Set chart title
                    chartWeightTrend.Title.Text = "Weight Trends Over Time";
                    chartWeightTrend.Update();
                }
                else
                {
                    MessageBox.Show("No weight tracking data available.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading weight trend graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmSetGoal_Load(object sender, EventArgs e)
        {
            LoadGoalsToGrid();
            LoadTargetActivityOptions();
            LoadGoalAchievementByMonthGraph();
            LoadAchievedVsPendingGraph();
            LoadAchievedGoals();
            LoadWeightTrendGraph();
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
                { "Weight Loss", new List<string> {"Swimming", "Walking", "Cycling" ,"Rowing", "Hiking" } },
                { "Weight Gain", new List<string> {"Weightlifting" } },
                { "Maintain Weight", new List<string> { "Rowing", "Hiking", "Walking", "Cycling" } }
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
