using Fitness_Tracker.dao;
using Fitness_Tracker.Entities;
using Guna.Charts.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

namespace Fitness_Tracker.Views
{
    public partial class frmSwimming : UserControl
    {
        private readonly ConnectionDB db;
        public frmSwimming()
        {
            InitializeComponent();
            db = ConnectionDB.GetInstance(); // Use Singleton for ConnectionDB
        }

        private void btnSwimmingRecord_Click(object sender, EventArgs e)
        {
            try
            {
                var user = User.GetInstance();


                var activity = new Activity
                {
                    ActivityId = 1, // Swimming Activity ID
                    ActivityName = "Swimming",
                    Descriptions = "Swimming activity description."
                };

                var record = new Record
                {
                    RecordDate = DateTime.Now,
                    Person = user,
                    Activity = activity,
                    BurnedCalories = 0, // Placeholder, calculated later
                    IntesityLevel = cboIntensity.SelectedItem?.ToString()
                };

                // Validate inputs
                var (isValid, laps, timeTaken, averageHeartRate, selectedIntensity) = ValidateInputs();
                if (!isValid) return;

                // Prepare metrics
                var metrics = new Dictionary<int, double>
                {
                    { 1, laps },             // Metric ID 1: Laps
                    { 2, timeTaken },        // Metric ID 2: Time Taken (minutes)
                    { 3, averageHeartRate }  // Metric ID 3: Average Heart Rate
                };

                // Clear input fields after recording
                txtSwimmingLaps.Clear();
                txtSwimmingTime.Clear();
                txtSwimmingHeartRate.Clear();
                cboIntensity.SelectedIndex = -1;

                // Display summary
                lblUserSummary.Text = $"You swam {laps} laps in {timeTaken} minutes with an average heart rate of {averageHeartRate} bpm.";

                // Update the circular progress bar and handle record
                LoadSwimmingGraph();
                LoadSwimmingMetrics();
                LoadSwimmingSummary();
                LoadRecentSwimmingActivity();
                LoadHistoricalComparisonGraph();
                HandleActivityRecord(1, metrics, selectedIntensity); // Swimming Activity ID = 1
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private (bool isValid, int laps, double timeTaken, double averageHeartRate, string intensity) ValidateInputs()
        {
            int laps = 0;
            double timeTaken = 0;
            double averageHeartRate = 0;
            string selectedIntensity = string.Empty;

            // Validate Laps
            if (!int.TryParse(txtSwimmingLaps.Text, out laps) || laps < 0)
            {
                MessageBox.Show("Please enter a valid non-negative number for Laps.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, laps, timeTaken, averageHeartRate, selectedIntensity);
            }

            // Validate Time Taken
            if (!double.TryParse(txtSwimmingTime.Text, out timeTaken) || timeTaken <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for Time Taken (in minutes).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, laps, timeTaken, averageHeartRate, selectedIntensity);
            }

            // Validate Average Heart Rate
            if (!double.TryParse(txtSwimmingHeartRate.Text, out averageHeartRate) || averageHeartRate <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for Average Heart Rate.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, laps, timeTaken, averageHeartRate, selectedIntensity);
            }

            // Validate Intensity
            if (cboIntensity.SelectedItem == null)
            {
                MessageBox.Show("Please select an intensity level.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, laps, timeTaken, averageHeartRate, selectedIntensity);
            }

            selectedIntensity = cboIntensity.SelectedItem.ToString();

            // Intensity-based validation for laps
            if (selectedIntensity == "Light" && (laps < 0 || laps > 14))
            {
                MessageBox.Show("For Light intensity, laps should be between 0 and 14. Please adjust your laps or activity type.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, laps, timeTaken, averageHeartRate, selectedIntensity);
            }
            else if (selectedIntensity == "Moderate" && (laps < 15 || laps > 29))
            {
                MessageBox.Show("For Moderate intensity, laps should be between 15 and 29. Please adjust your laps or activity type.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, laps, timeTaken, averageHeartRate, selectedIntensity);
            }
            else if (selectedIntensity == "Vigorous" && laps < 30)
            {
                MessageBox.Show("For Vigorous intensity, laps should be 30 or more. Please adjust your laps or activity type.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, laps, timeTaken, averageHeartRate, selectedIntensity);
            }

            return (true, laps, timeTaken, averageHeartRate, selectedIntensity);
        }
        private double CalculateBurnedCalories(Dictionary<int, double> metrics, Dictionary<int, double> calculationFactors, double metValue, double userWeight, double durationHours)
        {
            double caloriesFromMet = metValue * userWeight * durationHours;
            double caloriesFromFactors = 0;

            foreach (var metric in metrics)
            {
                if (calculationFactors.ContainsKey(metric.Key))
                {
                    caloriesFromFactors += metric.Value * calculationFactors[metric.Key];
                }
            }

            return Math.Round(caloriesFromMet + caloriesFromFactors, 2); // Round to 2 decimal places
        }
        private void HandleActivityRecord(int activityId, Dictionary<int, double> metrics, string intensity)
        {
            try
            {
                // Create and link objects
                var user = User.GetInstance();

                var activity = new Activity
                {
                    ActivityId = activityId,
                    ActivityName = db.GetActivityNameById(activityId), // Assuming this method exists in your DB class
                };

                var record = new Record
                {
                    Person = user,
                    Activity = activity,
                    RecordDate = DateTime.Now,
                    IntesityLevel = intensity
                };

                // Calculate burned calories and update record
                var calculationFactors = db.GetCalculationFactors(activityId);
                double metValue = db.GetMetValue(activityId, intensity);
                double userWeight = frmLogin.user.Weight;
                double durationHours = metrics[2] / 60; // Convert Time Taken (minutes) to hours
                record.BurnedCalories = CalculateBurnedCalories(metrics, calculationFactors, metValue, userWeight, durationHours);

                // Insert record and metrics into the database
                int recordId = db.InsertRecords(
                    record.BurnedCalories,
                    record.Activity.ActivityId,
                    record.IntesityLevel
                );

                if (recordId <= 0)
                {
                    MessageBox.Show("Failed to insert the record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (db.InsertMetricValues(activityId, recordId, metrics))
                {
                    MessageBox.Show("Swimming record successfully inserted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to insert metric values.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadSwimmingGraph()
        {
            try
            {
                // Create or fetch objects
                var user = User.GetInstance();
                var activity = new Activity
                {
                    ActivityId = 1, // Swimming Activity ID
                    ActivityName = db.GetActivityNameById(1)
                };

                // Fetch graph data
                DataTable swimmingGraphData = db.GetActivityGraphData(user.PersonID, activity.ActivityId);

                // Clear and update chart
                if (swimmingGraphData != null && swimmingGraphData.Rows.Count > 0)
                {
                    gunaLineDataset1.DataPoints.Clear();
                    foreach (DataRow row in swimmingGraphData.Rows)
                    {
                        string date = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd");
                        double calories = Convert.ToDouble(row["CaloriesBurned"]);
                        gunaLineDataset1.DataPoints.Add(date, calories);
                    }

                    gunaLineDataset1.Label = "Calories Burned Over Time";
                    chartSwimmingProgress.Datasets.Add(gunaLineDataset1);
                    chartSwimmingProgress.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading swimming graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSwimmingMetrics()
        {
            try
            {
                // Create or fetch objects
                var user = User.GetInstance();
                var activity = new Activity
                {
                    ActivityId = 1, // Swimming Activity ID
                    ActivityName = db.GetActivityNameById(1)
                };

                // Fetch metrics data
                DataTable metricData = db.GetSwimmingMetricsOverTime(user.PersonID);

                // Clear existing datasets
                chartSwimmingMetrics.Datasets.Clear();

                // Create individual datasets for each metric
                var lapsDataset = new Guna.Charts.WinForms.GunaLineDataset
                {
                    Label = "Laps",
                    BorderWidth = 2,
                    PointRadius = 4,
                    BorderColor = Color.Blue 
                };

                var timeDataset = new Guna.Charts.WinForms.GunaLineDataset
                {
                    Label = "Time Taken (minutes)",
                    BorderWidth = 2,
                    PointRadius = 4,
                    BorderColor = Color.Green
                };

                var heartRateDataset = new Guna.Charts.WinForms.GunaLineDataset
                {
                    Label = "Average Heart Rate (bpm)",
                    BorderWidth = 2,
                    PointRadius = 4,
                    BorderColor = Color.Red // Line color
                    
                };

                // Populate datasets with data from the DataTable
                if (metricData != null && metricData.Rows.Count > 0)
                {
                    foreach (DataRow row in metricData.Rows)
                    {
                        string date = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd");

                        // Parse metric values
                        double laps = Convert.ToDouble(row["Laps"]);
                        double timeTaken = Convert.ToDouble(row["TimeTaken"]);
                        double avgHeartRate = Convert.ToDouble(row["AvgHeartRate"]);

                        // Add data points to respective datasets
                        lapsDataset.DataPoints.Add(date, laps);
                        timeDataset.DataPoints.Add(date, timeTaken);
                        heartRateDataset.DataPoints.Add(date, avgHeartRate);
                    }

                    // Add datasets to the chart
                    chartSwimmingMetrics.Datasets.Add(lapsDataset);
                    chartSwimmingMetrics.Datasets.Add(timeDataset);
                    chartSwimmingMetrics.Datasets.Add(heartRateDataset);

                    // Customize the chart title
                    chartSwimmingMetrics.Title.Text = "Swimming Metrics Over Time (Line Chart)";
                    chartSwimmingMetrics.Update();
                }
                else
                {
                    MessageBox.Show("No data available for swimming metrics chart.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading swimming metrics chart: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadSwimmingSummary()
        {
            try
            {
                // Create or fetch objects
                var user = User.GetInstance();
                var activity = new Activity
                {
                    ActivityId = 1,
                    ActivityName = db.GetActivityNameById(1)
                };

                // Fetch summary data
                DataTable summaryData = db.GetSwimmingSummary(user.PersonID);

                // Update UI
                if (summaryData != null && summaryData.Rows.Count > 0)
                {
                    lblTotalLaps.Text = $"Total Laps: {summaryData.Rows[0]["TotalLaps"]}";
                    lblTotalTime.Text = $"Total Time: {summaryData.Rows[0]["TotalTime"]} minutes";
                    lblAvgHeartRate.Text = $"Average Heart Rate: {summaryData.Rows[0]["AvgHeartRate"]} bpm";
                }
                else
                {
                    lblTotalLaps.Text = "Total Laps: 0";
                    lblTotalTime.Text = "Total Time: 0 minutes";
                    lblAvgHeartRate.Text = "Average Heart Rate: N/A";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading swimming summary: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadRecentSwimmingActivity()
        {
            try
            {
                // Fetch the most recent swimming activity
                DataRow recentActivity = db.GetRecentSwimmingActivity(frmLogin.user.PersonID);


                if (recentActivity != null)
                {
                    // Extract values from the DataRow
                    string date = Convert.ToDateTime(recentActivity["Date"]).ToString("yyyy-MM-dd");
                    double laps = Convert.ToDouble(recentActivity["Laps"]);
                    double timeTaken = Convert.ToDouble(recentActivity["TimeTaken"]);
                    double avgHeartRate = Convert.ToDouble(recentActivity["HeartRate"]);
                    double caloriesBurned = Convert.ToDouble(recentActivity["CaloriesBurned"]);

                    // Update labels with the data
                    lblRecentDate.Text = $"Date: {date}";
                    lblRecentLaps.Text = $"Laps: {laps}";
                    lblRecentTime.Text = $"Time Taken: {timeTaken} minutes";
                    lblRecentHeartRate.Text = $"Average Heart Rate: {Math.Round(avgHeartRate, 2)} bpm";
                    lblRecentCalories.Text = $"Calories Burned: {caloriesBurned} kcal";
                }
                else
                {
                    // No recent activity found
                    lblRecentDate.Text = "Date: N/A";
                    lblRecentLaps.Text = "Laps: N/A";
                    lblRecentTime.Text = "Time Taken: N/A";
                    lblRecentHeartRate.Text = "Average Heart Rate: N/A";
                    lblRecentCalories.Text = "Calories Burned: N/A";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recent swimming activity: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadSwimmingTips()
        {
            string intensity = cboIntensity.SelectedItem?.ToString(); // Get selected intensity

            if (string.IsNullOrEmpty(intensity))
            {
                lblTips.Text = "Tip: Select your activity intensity to get personalized suggestions!";
            }
            else
            {
                switch (intensity.ToLower())
                {
                    case "light":
                        lblTips.Text = "Tip: Light swimming is great for beginners! Remember to focus on your technique.";
                        break;
                    case "moderate":
                        lblTips.Text = "Tip: Moderate swimming burns more calories. Stay hydrated!";
                        break;
                    case "vigorous":
                        lblTips.Text = "Tip: Vigorous swimming is intense! Take breaks if you feel fatigued.";
                        break;
                    default:
                        lblTips.Text = "Tip: Swimming is great for overall fitness. Keep it up!";
                        break;
                }
            }
        }
        private void LoadSwimmingInsights()
        {
            try
            {
                // Create or fetch objects
                var user = User.GetInstance();
                var activity = new Activity
                {
                    ActivityId = 1, // Swimming Activity ID
                    ActivityName = db.GetActivityNameById(1) // Get activity name dynamically
                };

                // Fetch max calories burned for swimming
                double maxCaloriesForSwimming = db.GetMaxCaloriesForActivity(user.PersonID, activity.ActivityId);

                // Update the UI
                if (maxCaloriesForSwimming > 0)
                {
                    lblMaxCalories.Text = $"Maximum Calories Burned: {maxCaloriesForSwimming} kcal";
                }
                else
                {
                    lblMaxCalories.Text = "Maximum Calories Burned: N/A";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading swimming insights: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadHistoricalComparisonGraph()
        {
            try
            {
                // Create or fetch objects
                var user = User.GetInstance();

                // Fetch historical data
                DataTable comparisonData = db.GetHistoricalComparison(user.PersonID);

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
                        string activityName = row["ActivityName"].ToString();
                        double caloriesBurned = Convert.ToDouble(row["CaloriesBurned"]);

                        comparisonDataset.DataPoints.Add(activityName, caloriesBurned);
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
        private void DisplayActivityScheduleReminder(int activityId, string activityName)
        {
            try
            {
                // Create objects
                var user = User.GetInstance(); // Fetch the singleton instance of the user
                var activity = new Activity
                {
                    ActivityId = activityId,
                    ActivityName = activityName
                };

                // Fetch schedules from the database for the user and activity
                DataTable scheduleData = db.GetUpcomingActivitySchedules(user.PersonID, activity.ActivityId);

                if (scheduleData != null && scheduleData.Rows.Count > 0)
                {
                    foreach (DataRow row in scheduleData.Rows)
                    {
                        DateTime date = Convert.ToDateTime(row["Date"]);
                        TimeSpan startTime = TimeSpan.Parse(row["StartTime"].ToString());
                        int duration = Convert.ToInt32(row["Duration"]);

                        // Display today's schedule first
                        if (date.Date == DateTime.Today)
                        {
                            lblScheduleReminder.Text = $"Today's {activity.ActivityName} Schedule: {startTime:hh\\:mm} for {duration} minutes.";
                            lblScheduleReminder.ForeColor = Color.Green;
                            return;
                        }
                    }

                    // Display the next upcoming schedule
                    DataRow upcomingRow = scheduleData.Rows[0];
                    DateTime upcomingDate = Convert.ToDateTime(upcomingRow["Date"]);
                    TimeSpan upcomingStartTime = TimeSpan.Parse(upcomingRow["StartTime"].ToString());
                    int upcomingDuration = Convert.ToInt32(upcomingRow["Duration"]);

                    lblScheduleReminder.Text = $"Next {activity.ActivityName} Schedule: {upcomingDate:yyyy-MM-dd} at {upcomingStartTime:hh\\:mm} for {upcomingDuration} minutes.";
                    lblScheduleReminder.ForeColor = Color.Blue;
                }
                else
                {
                    lblScheduleReminder.Text = $"No upcoming {activity.ActivityName} schedules.";
                    lblScheduleReminder.ForeColor = Color.DarkRed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading {activityName} schedule reminder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void frmSwimming_Load(object sender, EventArgs e)
        {
            DisplayActivityScheduleReminder(1, "Swimming"); // 1 = Swimming Activity ID
            LoadSwimmingInsights();
            LoadSwimmingTips();
            LoadSwimmingGraph();
            LoadSwimmingMetrics();
            LoadSwimmingSummary();
            LoadRecentSwimmingActivity();
            LoadHistoricalComparisonGraph();
        }

        private void cboIntensity_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSwimmingTips();
        }
    }
} 