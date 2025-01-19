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
    public partial class frmRowing : UserControl
    {
        private ConnectionDB db;
        public frmRowing()
        {
            InitializeComponent();
        }

        private void btnRowingRecord_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                var (isValid, totalStrokes, distance, timeTaken, intensity) = ValidateInputs();
                if (!isValid) return;

                // Prepare metrics
                var metrics = new Dictionary<int, double>
                {
                    { 16, totalStrokes },   // Metric ID: Total Strokes
                    { 17, distance },       // Metric ID: Distance
                    { 18, timeTaken }       // Metric ID: Time Taken
                };
                // Clear inputs after recording
                txtRowingStrokes.Clear();
                txtRowingDistance.Clear();
                txtRowingTimeTaken.Clear();
                cboIntensity.SelectedIndex = -1;

                // Update User Summary
                lblUserSummary.Text = $"You completed {totalStrokes} strokes, covered {distance} km in {timeTaken} minutes.";

                HandleActivityRecord(6, metrics, intensity); // Rowing Activity ID = 6
                // Call methods to update graphs and metrics
                LoadRowingGraph();
                LoadRowingMetrics();
                LoadRowingSummary();
                LoadRecentRowingActivity();
                LoadHistoricalComparisonGraph();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private (bool isValid, int totalStrokes, double distance, double timeTaken, string intensity) ValidateInputs()
        {
            int totalStrokes = 0;
            double distance = 0;
            double timeTaken = 0;
            string intensity = string.Empty;

            // Validate Total Strokes
            if (!int.TryParse(txtRowingStrokes.Text, out totalStrokes) || totalStrokes < 0)
            {
                MessageBox.Show("Please enter a valid non-negative number for Total Strokes.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, totalStrokes, distance, timeTaken, intensity);
            }

            // Validate Distance
            if (!double.TryParse(txtRowingDistance.Text, out distance) || distance <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for Distance.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, totalStrokes, distance, timeTaken, intensity);
            }

            // Validate Time Taken
            if (!double.TryParse(txtRowingTimeTaken.Text, out timeTaken) || timeTaken <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for Time Taken.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, totalStrokes, distance, timeTaken, intensity);
            }

            // Validate Intensity
            if (cboIntensity.SelectedItem == null)
            {
                MessageBox.Show("Please select an intensity level.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, totalStrokes, distance, timeTaken, intensity);
            }

            intensity = cboIntensity.SelectedItem.ToString();
            return (true, totalStrokes, distance, timeTaken, intensity);
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
                db = new ConnectionDB();
                db.OpenConnection();

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
                double userWeight = frmLogin.person.Weight;
                double durationHours = metrics[18] / 60; // Convert Time Taken (minutes) to hours

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
                    MessageBox.Show("Rowing record successfully inserted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            finally
            {
                db?.CloseConnection();
            }
        }
        private void LoadRowingGraph()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                DataTable rowingGraphData = db.GetActivityGraphData(frmLogin.person.PersonID, 6); // 6 is Rowing Activity ID

                if (rowingGraphData != null && rowingGraphData.Rows.Count > 0)
                {
                    gunaLineDataset1.DataPoints.Clear();

                    foreach (DataRow row in rowingGraphData.Rows)
                    {
                        string date = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd");
                        double calories = Convert.ToDouble(row["CaloriesBurned"]);

                        gunaLineDataset1.DataPoints.Add(date, calories);
                    }

                    gunaLineDataset1.Label = "Calories Burned Over Time";
                    gunaLineDataset1.BorderWidth = 2;
                    gunaLineDataset1.PointRadius = 4;

                    if (!chartRowingProgress.Datasets.Contains(gunaLineDataset1))
                    {
                        chartRowingProgress.Datasets.Add(gunaLineDataset1);
                    }

                    chartRowingProgress.Title.Text = "Calories Burned from Rowing";
                    chartRowingProgress.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading rowing graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }

        private void LoadRowingMetrics()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                DataTable metricData = db.GetRowingMetricsOverTime(frmLogin.person.PersonID);

                chartRowingMetrics.Datasets.Clear();

                var strokesDataset = new Guna.Charts.WinForms.GunaLineDataset
                {
                    Label = "Total Strokes",
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
                    BorderColor = Color.Red
                };

                if (metricData != null && metricData.Rows.Count > 0)
                {
                    foreach (DataRow row in metricData.Rows)
                    {
                        string date = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd");

                        double totalStrokes = Convert.ToDouble(row["TotalStrokes"]);
                        double distance = Convert.ToDouble(row["Distance"]);
                        double timeTaken = Convert.ToDouble(row["TimeTaken"]);

                        strokesDataset.DataPoints.Add(date, totalStrokes);
                        distanceDataset.DataPoints.Add(date, distance);
                        timeDataset.DataPoints.Add(date, timeTaken);
                    }

                    chartRowingMetrics.Datasets.Add(strokesDataset);
                    chartRowingMetrics.Datasets.Add(distanceDataset);
                    chartRowingMetrics.Datasets.Add(timeDataset);

                    chartRowingMetrics.Title.Text = "Rowing Metrics Over Time";
                    chartRowingMetrics.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading rowing metrics chart: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        private void LoadRowingSummary()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                DataTable summaryData = db.GetRowingSummary(frmLogin.person.PersonID);

                if (summaryData != null && summaryData.Rows.Count > 0)
                {
                    double totalStrokes = summaryData.Rows[0]["TotalStrokes"] != DBNull.Value ? Convert.ToDouble(summaryData.Rows[0]["TotalStrokes"]) : 0.0;
                    double totalDistance = summaryData.Rows[0]["TotalDistance"] != DBNull.Value ? Convert.ToDouble(summaryData.Rows[0]["TotalDistance"]) : 0.0;
                    double totalTime = summaryData.Rows[0]["TotalTime"] != DBNull.Value ? Convert.ToDouble(summaryData.Rows[0]["TotalTime"]) : 0.0;

                    lblTotalStrokes.Text = $"Total Strokes: {totalStrokes}";
                    lblTotalDistance.Text = $"Total Distance: {Math.Round(totalDistance, 2)} km";
                    lblTotalTime.Text = $"Total Time: {Math.Round(totalTime, 2)} minutes";
                }
                else
                {
                    lblTotalStrokes.Text = "Total Strokes: 0";
                    lblTotalDistance.Text = "Total Distance: 0 km";
                    lblTotalTime.Text = "Total Time: 0 minutes";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading rowing summary: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db?.CloseConnection();
            }
        }
        private void LoadRecentRowingActivity()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                DataRow recentActivity = db.GetRecentRowingActivity(frmLogin.person.PersonID);

                if (recentActivity != null)
                {
                    string date = Convert.ToDateTime(recentActivity["Date"]).ToString("yyyy-MM-dd");
                    double totalStrokes = Convert.ToDouble(recentActivity["TotalStrokes"]);
                    double distance = Convert.ToDouble(recentActivity["Distance"]);
                    double timeTaken = Convert.ToDouble(recentActivity["TimeTaken"]);
                    double caloriesBurned = Convert.ToDouble(recentActivity["CaloriesBurned"]);

                    lblRecentDate.Text = $"Date: {date}";
                    lblRecentStrokes.Text = $"Total Strokes: {totalStrokes}";
                    lblRecentDistance.Text = $"Distance: {distance} km";
                    lblRecentTime.Text = $"Time Taken: {timeTaken} minutes";
                    lblRecentCalories.Text = $"Calories Burned: {caloriesBurned} kcal";
                }
                else
                {
                    lblRecentDate.Text = "Date: N/A";
                    lblRecentStrokes.Text = "Total Strokes: N/A";
                    lblRecentDistance.Text = "Distance: N/A";
                    lblRecentTime.Text = "Time Taken: N/A";
                    lblRecentCalories.Text = "Calories Burned: N/A";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recent rowing activity: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db?.CloseConnection();
            }
        }
        private void LoadRowingTips()
        {
            string intensity = cboIntensity.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(intensity))
            {
                lblTips.Text = "Tip: Select intensity for personalized suggestions!";
            }
            else
            {
                switch (intensity.ToLower())
                {
                    case "light":
                        lblTips.Text = "Tip: Light rowing is excellent for beginners! Focus on smooth strokes.";
                        break;
                    case "moderate":
                        lblTips.Text = "Tip: Moderate rowing builds endurance. Keep a steady rhythm.";
                        break;
                    case "vigorous":
                        lblTips.Text = "Tip: Vigorous rowing burns more calories! Ensure proper form and pacing.";
                        break;
                    default:
                        lblTips.Text = "Tip: Rowing is great for full-body strength and cardio!";
                        break;
                }
            }
        }
        private void LoadRowingInsights()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                double maxCaloriesForRowing = db.GetMaxCaloriesForActivity(frmLogin.person.PersonID, 6); // 6 is Rowing Activity ID

                if (maxCaloriesForRowing > 0)
                {
                    lblMaxCalories.Text = $"Maximum Calories Burned: {maxCaloriesForRowing} kcal";
                }
                else
                {
                    lblMaxCalories.Text = "Maximum Calories Burned: N/A";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading rowing insights: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db?.CloseConnection();
            }
        }
        private void LoadHistoricalComparisonGraph()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                // Fetch historical data for calories burned across all activities
                DataTable comparisonData = db.GetHistoricalComparison(frmLogin.person.PersonID);

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
                db = new ConnectionDB();
                db.OpenConnection();

                // Ensure the query fetches schedules for today or later
                DataTable scheduleData = db.GetUpcomingActivitySchedules(frmLogin.person.PersonID, activityId);

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
            finally
            {
                db?.CloseConnection();
            }
        }
        private void frmRowing_Load(object sender, EventArgs e)
        {
            DisplayActivityScheduleReminder(6, "Rowing"); // 1 = Rowing Activity ID
            LoadRowingGraph();
            LoadRowingMetrics();
            LoadRowingSummary();
            LoadRecentRowingActivity();
            LoadRowingTips();
            LoadRowingInsights();
            LoadHistoricalComparisonGraph();
        }

        private void cboIntensity_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRowingTips();
        }
    }
}
