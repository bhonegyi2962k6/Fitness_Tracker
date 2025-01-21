using Fitness_Tracker.dao;
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

namespace Fitness_Tracker.Views
{
    public partial class frmHiking : UserControl
    {
        private readonly ConnectionDB db;
        public frmHiking()
        {
            InitializeComponent();
            db = ConnectionDB.GetInstance(); // Use the Singleton instance
        }

        private void btnHikingRecord_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                var (isValid, elevationGained, distance, timeTaken, intensity) = ValidateInputs();
                if (!isValid) return;

                // Prepare metrics
                var metrics = new Dictionary<int, double>
                {
                    { 10, elevationGained },  // Metric ID: Elevation Gained
                    { 11, distance },         // Metric ID: Distance
                    { 12, timeTaken }         // Metric ID: Time Taken
                };
                lblUserSummary.Text = $"You hiked {distance} km with an elevation gain of {elevationGained} meters in {timeTaken} minutes.";

                HandleActivityRecord(4, metrics, intensity); // Hiking Activity ID = 4
                                                             // Update graphs and metrics
                LoadHikingGraph();
                LoadHikingMetrics();
                LoadHikingSummary();
                LoadRecentHikingActivity();
                LoadHistoricalComparisonGraph();
                LoadHikingInsights();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private (bool isValid, double elevationGained, double distance, double timeTaken, string intensity) ValidateInputs()
        {
            double elevationGained = 0;
            double distance = 0;
            double timeTaken = 0;
            string intensity = string.Empty;

            // Validate Elevation Gained
            if (!double.TryParse(txtHikingElevation.Text, out elevationGained) || elevationGained < 0)
            {
                MessageBox.Show("Please enter a valid non-negative number for Elevation Gained (meters).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, elevationGained, distance, timeTaken, intensity);
            }

            // Validate Distance
            if (!double.TryParse(txtHikingDistance.Text, out distance) || distance <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for Distance (kilometers).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, elevationGained, distance, timeTaken, intensity);
            }

            // Validate Time Taken
            if (!double.TryParse(txtHikingTimeTaken.Text, out timeTaken) || timeTaken <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for Time Taken (minutes).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, elevationGained, distance, timeTaken, intensity);
            }

            // Validate Intensity
            if (cboIntensity.SelectedItem == null)
            {
                MessageBox.Show("Please select an intensity level (Moderate or Vigorous).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, elevationGained, distance, timeTaken, intensity);
            }

            intensity = cboIntensity.SelectedItem.ToString();

            // Intensity-based Validation
            if (intensity == "Moderate" && (elevationGained < 200 || elevationGained > 800 || distance > 10 || timeTaken > 120))
            {
                MessageBox.Show("For Moderate intensity, elevation should be between 200m and 800m, distance ≤ 10km, and time ≤ 120 minutes. Please adjust your inputs.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, elevationGained, distance, timeTaken, intensity);
            }
            else if (intensity == "Vigorous" && (elevationGained < 801 || distance > 20 || timeTaken > 240))
            {
                MessageBox.Show("For Vigorous intensity, elevation should be ≥ 801m, distance ≤ 20km, and time ≤ 240 minutes. Please adjust your inputs.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, elevationGained, distance, timeTaken, intensity);
            }

            return (true, elevationGained, distance, timeTaken, intensity);
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
                double durationHours = metrics[12] / 60; // Convert Time Taken (minutes) to hours

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
                    MessageBox.Show("Hiking record successfully inserted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        private void LoadHikingGraph()
        {
            try
            {

                DataTable hikingGraphData = db.GetActivityGraphData(frmLogin.user.PersonID, 4); // 4 is Hiking Activity ID

                if (hikingGraphData != null && hikingGraphData.Rows.Count > 0)
                {
                    gunaLineDataset1.DataPoints.Clear();

                    foreach (DataRow row in hikingGraphData.Rows)
                    {
                        string date = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd");
                        double calories = Convert.ToDouble(row["CaloriesBurned"]);

                        gunaLineDataset1.DataPoints.Add(date, calories);
                    }

                    gunaLineDataset1.Label = "Calories Burned Over Time";
                    gunaLineDataset1.BorderWidth = 2;
                    gunaLineDataset1.PointRadius = 4;

                    if (!chartHikingProgress.Datasets.Contains(gunaLineDataset1))
                    {
                        chartHikingProgress.Datasets.Add(gunaLineDataset1);
                    }

                    chartHikingProgress.Title.Text = "Calories Burn From Hiking";
                    chartHikingProgress.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading hiking graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        private void LoadHikingMetrics()
        {
            try
            {
                

                DataTable metricData = db.GetHikingMetricsOverTime(frmLogin.user.PersonID);

                chartHikingMetrics.Datasets.Clear();

                var elevationDataset = new GunaLineDataset
                {
                    Label = "Elevation Gained (m)",
                    BorderWidth = 2,
                    PointRadius = 4,
                    BorderColor = Color.Blue
                };

                var distanceDataset = new GunaLineDataset
                {
                    Label = "Distance (km)",
                    BorderWidth = 2,
                    PointRadius = 4,
                    BorderColor = Color.Green
                };

                var timeDataset = new GunaLineDataset
                {
                    Label = "Time Taken (minutes)",
                    BorderWidth = 2,
                    PointRadius = 4,
                    BorderColor = Color.Red
                };

                if (metricData != null && metricData.Rows.Count > 0)
                {
                    foreach (DataRow row in metricData.Rows)
                    {
                        string date = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd");

                        double elevation = Convert.ToDouble(row["ElevationGained"]);
                        double distance = Convert.ToDouble(row["Distance"]);
                        double time = Convert.ToDouble(row["TimeTaken"]);

                        elevationDataset.DataPoints.Add(date, elevation);
                        distanceDataset.DataPoints.Add(date, distance);
                        timeDataset.DataPoints.Add(date, time);
                    }

                    chartHikingMetrics.Datasets.Add(elevationDataset);
                    chartHikingMetrics.Datasets.Add(distanceDataset);
                    chartHikingMetrics.Datasets.Add(timeDataset);

                    chartHikingMetrics.Title.Text = "Hiking Metrics Over Time";
                    chartHikingMetrics.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading hiking metrics chart: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadHikingSummary()
        {
            try
            {

                DataTable summaryData = db.GetHikingSummary(frmLogin.user.PersonID);

                if (summaryData != null && summaryData.Rows.Count > 0)
                {
                    double totalElevation = summaryData.Rows[0]["TotalElevation"] != DBNull.Value ? Convert.ToDouble(summaryData.Rows[0]["TotalElevation"]) : 0.0;
                    double totalDistance = summaryData.Rows[0]["TotalDistance"] != DBNull.Value ? Convert.ToDouble(summaryData.Rows[0]["TotalDistance"]) : 0.0;
                    double totalTime = summaryData.Rows[0]["TotalTime"] != DBNull.Value ? Convert.ToDouble(summaryData.Rows[0]["TotalTime"]) : 0.0;

                    lblTotalElevation.Text = $"Total Elevation Gained: {totalElevation} m";
                    lblTotalDistance.Text = $"Total Distance: {Math.Round(totalDistance, 2)} km";
                    lblTotalTime.Text = $"Total Time: {Math.Round(totalTime, 2)} minutes";
                }
                else
                {
                    lblTotalElevation.Text = "Total Elevation Gained: 0 m";
                    lblTotalDistance.Text = "Total Distance: 0 km";
                    lblTotalTime.Text = "Total Time: 0 minutes";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading hiking summary: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        private void LoadRecentHikingActivity()
        {
            try
            {

                DataRow recentActivity = db.GetRecentHikingActivity(frmLogin.user.PersonID);

                if (recentActivity != null)
                {
                    string date = Convert.ToDateTime(recentActivity["Date"]).ToString("yyyy-MM-dd");
                    double elevation = Convert.ToDouble(recentActivity["ElevationGained"]);
                    double distance = Convert.ToDouble(recentActivity["Distance"]);
                    double timeTaken = Convert.ToDouble(recentActivity["TimeTaken"]);
                    double caloriesBurned = Convert.ToDouble(recentActivity["CaloriesBurned"]);

                    lblRecentDate.Text = $"Date: {date}";
                    lblRecentElevation.Text = $"Elevation Gained: {elevation} m";
                    lblRecentDistance.Text = $"Distance: {distance} km";
                    lblRecentTime.Text = $"Time Taken: {timeTaken} minutes";
                    lblRecentCalories.Text = $"Calories Burned: {caloriesBurned} kcal";
                }
                else
                {
                    lblRecentDate.Text = "Date: N/A";
                    lblRecentElevation.Text = "Elevation Gained: N/A";
                    lblRecentDistance.Text = "Distance: N/A";
                    lblRecentTime.Text = "Time Taken: N/A";
                    lblRecentCalories.Text = "Calories Burned: N/A";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recent hiking activity: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        private void LoadHikingTips()
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
                    case "moderate":
                        lblTips.Text = "Tip: Moderate hiking is a great cardiovascular workout! Wear proper footwear and stay hydrated.";
                        break;
                    case "vigorous":
                        lblTips.Text = "Tip: Vigorous hiking can be intense! Take short breaks and keep an eye on the trail conditions.";
                        break;
                    default:
                        lblTips.Text = "Tip: Hiking is an excellent way to enjoy nature and stay active. Plan your route and pack essentials!";
                        break;
                }
            }
        }

        private void LoadHikingInsights()
        {
            try
            {
                // Get max calories burned for swimming
                double maxCaloriesForHiking = db.GetMaxCaloriesForActivity(frmLogin.user.PersonID, 4); // 1 is Hiking activity ID
                lblMaxCalories.Text = $"Maximum Calories Burned: {maxCaloriesForHiking} kcal";


                if (maxCaloriesForHiking > 0)
                {
                    lblMaxCalories.Text = $"Maximum Calories Burned: {maxCaloriesForHiking} kcal";
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
        private void frmHiking_Load(object sender, EventArgs e)
        {
            DisplayActivityScheduleReminder(4, "Hiking"); // 4 = Hiking Activity ID
            LoadHikingGraph();
            LoadHikingMetrics();
            LoadHikingSummary();
            LoadRecentHikingActivity();
            LoadHistoricalComparisonGraph();
            LoadHikingTips();
            LoadHikingInsights();
        }

        private void cboIntensity_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadHikingTips();
        }
    }
}
