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
using SkiaSharp;
using Guna.Charts.WinForms;



namespace Fitness_Tracker.Views
{
    public partial class frmWalking : UserControl
    {
        private readonly ConnectionDB db;
        public frmWalking()
        {
            InitializeComponent();
            db = ConnectionDB.GetInstance(); // Use the Singleton instance
        }

        private void btnWalkingRecord_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                var (isValid, steps, distance, timeTaken, intensity) = ValidateInputs();
                if (!isValid) return;

                // Create metrics dictionary
                var metrics = new Dictionary<int, double>
                {
                    { 4, steps },      // Metric ID: Steps
                    { 5, distance },   // Metric ID: Distance
                    { 6, timeTaken }   // Metric ID: Time Taken
                };
                // Clear inputs after recording
                txtWalkingSteps.Clear();
                txtWalkingDistance.Clear();
                txtWalkingTimeTaken.Clear();
                cboIntensity.SelectedIndex = -1;

                // Display summary
                lblUserSummary.Text = $"You walked {steps} steps, covered {distance} km in {timeTaken} minutes.";

                // Update graphs and metrics
                LoadWalkingGraph();
                LoadWalkingMetrics();
                LoadWalkingSummary();
                LoadRecentWalkingActivity();
                LoadHistoricalComparisonGraph();

                HandleActivityRecord(2, metrics, intensity); // Walking Activity ID = 2
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private (bool isValid, int steps, double distance, double timeTaken, string intensity) ValidateInputs()
        {
            int steps = 0;
            double distance = 0;
            double timeTaken = 0;
            string intensity = string.Empty;

            // Validate Steps
            if (!int.TryParse(txtWalkingSteps.Text, out steps) || steps < 0)
            {
                MessageBox.Show("Please enter a valid non-negative number for Steps.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, steps, distance, timeTaken, intensity);
            }

            // Validate Distance
            if (!double.TryParse(txtWalkingDistance.Text, out distance) || distance <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for Distance.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, steps, distance, timeTaken, intensity);
            }

            // Validate Time Taken
            if (!double.TryParse(txtWalkingTimeTaken.Text, out timeTaken) || timeTaken <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for Time Taken.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, steps, distance, timeTaken, intensity);
            }

            // Validate Intensity
            if (cboIntensity.SelectedItem == null)
            {
                MessageBox.Show("Please select an intensity level.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, steps, distance, timeTaken, intensity);
            }

            intensity = cboIntensity.SelectedItem.ToString();

            // Intensity-based validation
            if (intensity == "Light" && (steps < 0 || steps > 5000 || distance > 3 || timeTaken > 30))
            {
                MessageBox.Show("For Light intensity, steps should be between 0 and 5000, distance ≤ 3 km, and time ≤ 30 minutes. Please adjust your inputs or activity type.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, steps, distance, timeTaken, intensity);
            }
            else if (intensity == "Moderate" && (steps < 5001 || steps > 10000 || distance > 7 || timeTaken > 60))
            {
                MessageBox.Show("For Moderate intensity, steps should be between 5001 and 10000, distance ≤ 7 km, and time ≤ 60 minutes. Please adjust your inputs or activity type.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, steps, distance, timeTaken, intensity);
            }
            else if (intensity == "Vigorous" && (steps < 10001 || distance > 15 || timeTaken > 120))
            {
                MessageBox.Show("For Vigorous intensity, steps should be > 10000, distance ≤ 15 km, and time ≤ 120 minutes. Please adjust your inputs or activity type.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, steps, distance, timeTaken, intensity);
            }

            return (true, steps, distance, timeTaken, intensity);
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
                // Step 1: Retrieve calculation factors and MET value
                var calculationFactors = db.GetCalculationFactors(activityId);
                double metValue = db.GetMetValue(activityId, intensity);

                // Validate MET value
                if (metValue <= 0)
                {
                    MessageBox.Show("Invalid MET value. Please check the selected intensity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Step 2: Retrieve user's weight and calculate duration
                double userWeight = frmLogin.user.Weight;
                double durationHours = metrics[6] / 60; // Convert Time Taken (minutes) to hours

                // Step 3: Calculate calories burned
                double burnedCalories = CalculateBurnedCalories(metrics, calculationFactors, metValue, userWeight, durationHours);

                // Step 4: Insert record into the database
                int recordId = db.InsertRecords(burnedCalories, activityId, intensity);
                if (recordId <= 0)
                {
                    MessageBox.Show("Failed to insert the record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Step 5: Insert metrics
                if (db.InsertMetricValues(activityId, recordId, metrics))
                {
                    MessageBox.Show("Walking record successfully inserted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void LoadWalkingGraph()
        {
            try
            {

                DataTable walkingGraphData = db.GetActivityGraphData(frmLogin.user.PersonID, 2); // 2 is Walking Activity ID

                if (walkingGraphData != null && walkingGraphData.Rows.Count > 0)
                {
                    gunaLineDataset1.DataPoints.Clear();

                    foreach (DataRow row in walkingGraphData.Rows)
                    {
                        string date = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd");
                        double calories = Convert.ToDouble(row["CaloriesBurned"]);

                        gunaLineDataset1.DataPoints.Add(date, calories);
                    }

                    gunaLineDataset1.Label = "Calories Burned Over Time";
                    gunaLineDataset1.BorderWidth = 2;
                    gunaLineDataset1.PointRadius = 4;

                    if (!chartWalkingProgress.Datasets.Contains(gunaLineDataset1))
                    {
                        chartWalkingProgress.Datasets.Add(gunaLineDataset1);
                    }

                    chartWalkingProgress.Title.Text = "Calories Burn From Walking";
                    chartWalkingProgress.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading walking graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadWalkingMetrics()
        {
            try
            {
                // Fetch the walking metrics data
                DataTable metricData = db.GetWalkingMetricsOverTime(frmLogin.user.PersonID);

                // Clear existing datasets
                chartWalkingMetrics.Datasets.Clear();

                // Create individual datasets for each metric
                var stepsDataset = new Guna.Charts.WinForms.GunaLineDataset
                {
                    Label = "Steps",
                    BorderWidth = 2,
                    PointRadius = 4,
                    BorderColor = Color.Blue
                };

                var distanceDataset = new Guna.Charts.WinForms.GunaLineDataset
                {
                    Label = "Distance (km)",
                    BorderWidth = 2,
                    PointRadius = 4,
                    BorderColor = Color.Green
                };

                var timeDataset = new Guna.Charts.WinForms.GunaLineDataset
                {
                    Label = "Time Taken (minutes)",
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
                        double steps = Convert.ToDouble(row["TotalSteps"]);
                        double distance = Convert.ToDouble(row["TotalDistance"]);
                        double timeTaken = Convert.ToDouble(row["TotalTime"]);

                        // Add data points to respective datasets
                        stepsDataset.DataPoints.Add(date, steps);
                        distanceDataset.DataPoints.Add(date, distance);
                        timeDataset.DataPoints.Add(date, timeTaken);
                    }

                    // Add datasets to the chart
                    chartWalkingMetrics.Datasets.Add(stepsDataset);
                    chartWalkingMetrics.Datasets.Add(distanceDataset);
                    chartWalkingMetrics.Datasets.Add(timeDataset);

                    // Customize the chart title
                    chartWalkingMetrics.Title.Text = "Walking Metrics Over Time (Line Chart)";
                    chartWalkingMetrics.Update();
                }
                else
                {
                    MessageBox.Show("No data available for walking metrics chart.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading walking metrics chart: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadWalkingSummary()
        {
            try
            {

                DataTable summaryData = db.GetWalkingSummary(frmLogin.user.PersonID);

                if (summaryData != null && summaryData.Rows.Count > 0)
                {
                    double totalSteps = summaryData.Rows[0]["TotalSteps"] != DBNull.Value ? Convert.ToDouble(summaryData.Rows[0]["TotalSteps"]) : 0.0;
                    double totalDistance = summaryData.Rows[0]["TotalDistance"] != DBNull.Value ? Convert.ToDouble(summaryData.Rows[0]["TotalDistance"]) : 0.0;
                    double totalTime = summaryData.Rows[0]["TotalTime"] != DBNull.Value ? Convert.ToDouble(summaryData.Rows[0]["TotalTime"]) : 0.0;

                    lblTotalSteps.Text = $"Total Steps: {totalSteps}";
                    lblTotalDistance.Text = $"Total Distance: {Math.Round(totalDistance, 2)} km";
                    lblTotalTime.Text = $"Total Time: {Math.Round(totalTime, 2)} minutes";
                }
                else
                {
                    lblTotalSteps.Text = "Total Steps: 0";
                    lblTotalDistance.Text = "Total Distance: 0 km";
                    lblTotalTime.Text = "Total Time: 0 minutes";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading walking summary: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db?.CloseConnection();
            }
        }

        private void LoadRecentWalkingActivity()
        {
            try
            {
                DataRow recentActivity = db.GetRecentWalkingActivity(frmLogin.user.PersonID);

                if (recentActivity != null)
                {
                    string date = Convert.ToDateTime(recentActivity["Date"]).ToString("yyyy-MM-dd");
                    double steps = Convert.ToDouble(recentActivity["Steps"]);
                    double distance = Convert.ToDouble(recentActivity["Distance"]);
                    double timeTaken = Convert.ToDouble(recentActivity["TimeTaken"]);
                    double caloriesBurned = Convert.ToDouble(recentActivity["CaloriesBurned"]);

                    lblRecentDate.Text = $"Date: {date}";
                    lblRecentSteps.Text = $"Steps: {steps} steps";
                    lblRecentDistance.Text = $"Distance: {distance} km";
                    lblRecentTime.Text = $"Time Taken: {timeTaken} minutes";
                    lblRecentCalories.Text = $"Calories Burned: {caloriesBurned} kcal";
                }
                else
                {
                    lblRecentDate.Text = "Date: N/A";
                    lblRecentSteps.Text = "Steps: N/A";
                    lblRecentDistance.Text = "Distance: N/A";
                    lblRecentTime.Text = "Time Taken: N/A";
                    lblRecentCalories.Text = "Calories Burned: N/A";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recent walking activity: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }
        private void LoadWalkingTips()
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
                        lblTips.Text = "Tip: Light walking is great for beginners! Keep a steady pace and enjoy the journey.";
                        break;
                    case "moderate":
                        lblTips.Text = "Tip: Moderate walking boosts cardiovascular health. Keep hydrated and wear comfortable shoes!";
                        break;
                    case "vigorous":
                        lblTips.Text = "Tip: Vigorous walking helps burn more calories! Focus on your posture and breathing.";
                        break;
                    default:
                        lblTips.Text = "Tip: Walking is excellent for staying active and improving your mood. Keep going!";
                        break;
                }
            }
        }

        private void LoadSwimmingInsights()
        {
            try
            {
              

                // Get max calories burned for swimming
                double maxCaloriesForWalking = db.GetMaxCaloriesForActivity(frmLogin.user.PersonID, 2); // 2 is Walking activity ID
                lblMaxCalories.Text = $"Maximum Calories Burned: {maxCaloriesForWalking} kcal";


                if (maxCaloriesForWalking > 0)
                {
                    lblMaxCalories.Text = $"Maximum Calories Burned: {maxCaloriesForWalking} kcal";
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
            finally
            {
                db.CloseConnection();
            }
        }
        private void DisplayActivityScheduleReminder(int activityId, string activityName)
        {
            try
            {
                // Ensure the query fetches schedules for today or later
                DataTable scheduleData = db.GetUpcomingActivitySchedules(frmLogin.user.PersonID, activityId);

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
                            lblScheduleReminder.Text = $"Today's {activityName} Schedule: {startTime:hh\\:mm} for {duration} minutes.";
                            lblScheduleReminder.ForeColor = Color.Green;
                            return;
                        }
                    }

                    // Display the next upcoming schedule
                    DataRow upcomingRow = scheduleData.Rows[0];
                    DateTime upcomingDate = Convert.ToDateTime(upcomingRow["Date"]);
                    TimeSpan upcomingStartTime = TimeSpan.Parse(upcomingRow["StartTime"].ToString());
                    int upcomingDuration = Convert.ToInt32(upcomingRow["Duration"]);

                    lblScheduleReminder.Text = $"Next {activityName} Schedule: {upcomingDate:yyyy-MM-dd} at {upcomingStartTime:hh\\:mm} for {upcomingDuration} minutes.";
                    lblScheduleReminder.ForeColor = Color.Blue;
                }
                else
                {
                    lblScheduleReminder.Text = $"No upcoming {activityName} schedules.";
                    lblScheduleReminder.ForeColor = Color.DarkRed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading {activityName} schedule reminder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void frmWalking_Load(object sender, EventArgs e)
        {
            DisplayActivityScheduleReminder(2, "Walking"); // 2 = Walking Activity ID
            LoadWalkingGraph();
            LoadWalkingMetrics();
            LoadRecentWalkingActivity();
            LoadWalkingSummary();
            LoadSwimmingInsights();
            LoadWalkingTips();
            LoadHistoricalComparisonGraph();
        }

        private void cboIntensity_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadWalkingTips();
        }
    }
}
