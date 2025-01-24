using Fitness_Tracker.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Fitness_Tracker.dao;
using Guna.Charts.WinForms;

namespace Fitness_Tracker.Views
{
    public partial class frmHome : UserControl
    {
        private readonly ConnectionDB db;
        public frmHome()
        {
            InitializeComponent();
            db = ConnectionDB.GetInstance(); // Use the Singleton instance
            LoadAchievedVsPendingGraph();
            ShowLastActivity();
            ShowUpcomingSchedule();
            ShowUpcomingGoal();
            LoadWeightTrendGraph();
            LoadHistoricalComparisonGraph();
            LoadSchedulesGraph();
            UpdateTotalGoalsAchieved();
            UpdateStreakCounter();
        }

        private void frmHome_Load(object sender, EventArgs e)
        {
            try
            {
                // Get the current user instance
                var user = User.GetInstance();

                // Display welcome message with Firstname and Lastname
                lblWelcome.Text = $"Welcome back, {user.Firstname} {user.Lastname}!";

                // Display user profile photo
                if (!string.IsNullOrEmpty(user.PhotoPath) && File.Exists(user.PhotoPath))
                {
                    picProfilePhoto.Image = Image.FromFile(user.PhotoPath);
                }
                else
                {
                    // Load default photo
                    picProfilePhoto.Image = Properties.Resources.user_icon_1024x1024_dtzturco;
                }

                // Retrieve all records for the current user
                List<Record> records = ConnectionDB.GetInstance().GetAllRecordsForUser(user.PersonID);

                // Calculate total calories burned
                double totalCaloriesBurned = records.Sum(record => record.BurnedCalories);

                // Update the label for Total Calories Burned
                lblTotalCaloriesBurned.Text = $"Total Calories Burned: {totalCaloriesBurned} kcal";

                // Display user's weight
                lblUserWeight.Text = $"Your Current Weight: {user.Weight} kg";
                ShowBadgeNotificationFromDatabase();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading home page: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void ShowLastActivity()
        {
            try
            {
                // Get the current user instance
                var user = User.GetInstance();

                // Retrieve the most recent activity record
                Record lastActivity = db.GetLastActivityForUser(user.PersonID);

                if (lastActivity != null)
                {
                    lblLastActivity.Text = $"Your most recent activity was {lastActivity.Activity.ActivityName} on {lastActivity.RecordDate:dd/MM/yyyy}, where you burned {lastActivity.BurnedCalories:N2} kcal.";
                }
                else
                {
                    lblLastActivity.Text = "Last Activity: No activity recorded yet.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving last activity: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ShowUpcomingGoal()
        {
            try
            {
                // Get the current user instance
                var user = User.GetInstance();

                // Retrieve the next upcoming goal
                GoalTracking upcomingGoal = db.GetNextUpcomingGoalForUser(user.PersonID);

                if (upcomingGoal != null)
                {
                    lblUpcomingGoal.Text = $"Your next goal is to {upcomingGoal.GoalType.ToLower()} with {upcomingGoal.Activity?.ActivityName ?? "no specific activity"} by {upcomingGoal.TargetDate:dd/MM/yyyy}. Aim for a target weight of {upcomingGoal.TargetWeight:N2} kg.";
                }
                else
                {
                    lblUpcomingGoal.Text = "You currently have no upcoming goals.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving upcoming goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ShowUpcomingSchedule()
        {
            try
            {
                // Get the current user instance
                var user = User.GetInstance();

                // Retrieve the next schedule
                Schedule nextSchedule = db.GetNextScheduleForUser(user.PersonID);

                if (nextSchedule != null)
                {
                    // Retrieve the first associated activity for the schedule
                    ScheduleActivity activity = db.GetFirstActivityForSchedule(nextSchedule.ScheduleId);

                    if (activity != null)
                    {
                        lblUpcomingSchedule.Text = $"Your next scheduled activity is {activity.Activity.ActivityName} on {nextSchedule.ScheduledDate:dd/MM/yyyy} at {activity.StartTime}. Duration: {activity.DurationMinutes} minutes.";
                    }
                    else
                    {
                        lblUpcomingSchedule.Text = $"Your next schedule is on {nextSchedule.ScheduledDate:dd/MM/yyyy}, but no activities are associated.";
                    }
                }
                else
                {
                    lblUpcomingSchedule.Text = "No upcoming schedules.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving upcoming schedule: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void LoadSchedulesGraph()
        {
            try
            {
                // Fetch data for the graph
                DataTable dt = db.GetSchedulesSummary(frmLogin.user.PersonID);

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Clear existing data points
                    gunaBarDataset1.DataPoints.Clear();

                    // List to hold Schedule and ScheduleActivity objects
                    List<ScheduleActivity> scheduleActivities = new List<ScheduleActivity>();

                    foreach (DataRow row in dt.Rows)
                    {
                        string activityName = row["Activity"].ToString();
                        int totalSchedules = Convert.ToInt32(row["TotalSchedules"]);

                        // Create Activity object
                        var activity = new Activity
                        {
                            ActivityName = activityName
                        };

                        // Create Schedule object (dummy schedule for visualization purposes)
                        var schedule = new Schedule
                        {
                            Person = frmLogin.user,
                            ScheduledDate = DateTime.Now // Set to the current date or a placeholder
                        };

                        // Create ScheduleActivity object
                        var scheduleActivity = new ScheduleActivity
                        {
                            Schedule = schedule,
                            Activity = activity,
                            // StartTime and DurationMinutes are not relevant here, but can be added if needed
                        };

                        // Add to the list
                        scheduleActivities.Add(scheduleActivity);

                        // Add data points to the dataset
                        gunaBarDataset1.DataPoints.Add(activityName, totalSchedules);
                    }

                    // Optionally customize the dataset's appearance
                    gunaBarDataset1.Label = "Number of Schedules";
                    gunaBarDataset1.BorderWidth = 1;

                }

                // Add the dataset to the chart (if not already added)
                if (!chartSchedules.Datasets.Contains(gunaBarDataset1))
                {
                    chartSchedules.Datasets.Add(gunaBarDataset1);
                }

                // Optional: Customize the chart's title
                chartSchedules.Title.Text = "Schedules by Activity";
                chartSchedules.Update(); // Refresh the chart
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading schedules graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadHistoricalComparisonGraph()
        {
            try
            {
                // Fetch historical data for calories burned across all activities
                DataTable comparisonData = db.GetHistoricalComparison(frmLogin.user.PersonID);

                // Clear existing datasets
                chartHistoricalComparison.Datasets.Clear();

                // Create a dataset for the comparison
                var comparisonDataset = new Guna.Charts.WinForms.GunaBarDataset
                {
                    Label = "Calories Burned by Activity",
                    BorderWidth = 1
                };

                // Populate the dataset with data
                if (comparisonData != null && comparisonData.Rows.Count > 0)
                {
                    foreach (DataRow row in comparisonData.Rows)
                    {
                        // Create Activity object
                        var activity = new Activity
                        {
                            ActivityName = row["ActivityName"].ToString()
                        };

                        // Create Record object
                        var record = new Record
                        {
                            Activity = activity,
                            BurnedCalories = Convert.ToDouble(row["CaloriesBurned"]),
                            Person = User.GetInstance() // Link the logged-in user
                        };

                        // Add data point to the dataset
                        comparisonDataset.DataPoints.Add(activity.ActivityName, record.BurnedCalories);
                    }

                    // Add the dataset to the chart
                    chartHistoricalComparison.Datasets.Add(comparisonDataset);

                    // Customize the chart
                    chartHistoricalComparison.Title.Text = "Historical Calories Burned Comparison";
                    chartHistoricalComparison.Update();
                }
                else
                {
                    MessageBox.Show("No historical data available for comparison.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading historical comparison graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private int CalculateStreak()
        {
            // Assuming you have a list of activity records for the user
            var user = User.GetInstance();
            var records = db.GetUserActivityRecords(user.PersonID);

            int streak = 0;
            DateTime currentDate = DateTime.Now.Date;

            foreach (var record in records.OrderByDescending(r => r.RecordDate))
            {
                if (record.RecordDate.Date == currentDate)
                {
                    streak++;
                    currentDate = currentDate.AddDays(-1);
                }
                else
                {
                    break;
                }
            }

            return streak;
        }

        private void UpdateStreakCounter()
        {
            int streak = CalculateStreak();
            lblStreakCounter.Text = $"Streak: {streak} days";
        }
        private int GetTotalGoalsAchieved()
        {
            var user = User.GetInstance();
            var goals = db.GetGoalsForUser(user.PersonID);

            return goals.Count(g => g.IsAchieved);
        }

        private void UpdateTotalGoalsAchieved()
        {
            int totalGoalsAchieved = GetTotalGoalsAchieved();
            lblTotalGoalsAchieved.Text = $"Total Goals Achieved: {totalGoalsAchieved}";
        }
        private void ShowBadgeNotificationFromDatabase()
        {
            try
            {
                // Get the current user instance
                var user = User.GetInstance();

                // Fetch total burned calories from the database
                double burnedCalories = ConnectionDB.GetInstance().GetTotalBurnedCalories(user.PersonID);

                string title = "Achievement Unlocked!";
                string message;
                Color badgeColor;

                if (burnedCalories >= 1000)
                {
                    message = "You earned a Green Badge for burning 1000+ kcal!";
                    badgeColor = Color.Red;
                }
                else if (burnedCalories >= 500)
                {
                    message = "You earned a Yellow Badge for burning 500–999 kcal!";
                    badgeColor = Color.Yellow;
                }
                else
                {
                    message = "Keep going! Burn more than 500 kcal for a badge.";
                    badgeColor = Color.Green;
                }

                // Display the toast notification
                frmToastForm toast = new frmToastForm(title, message, badgeColor);
                toast.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying badge notification: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
