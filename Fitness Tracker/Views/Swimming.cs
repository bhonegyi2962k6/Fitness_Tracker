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
using System.Windows.Media.Media3D;

namespace Fitness_Tracker.Views
{
    public partial class frmSwimming : UserControl
    {
        private ConnectionDB db;
        public frmSwimming()
        {
            InitializeComponent();
        }

        private void btnSwimmingRecord_Click(object sender, EventArgs e)
        {
            try
            {
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

                // Update the circular progress bar
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
                if (metrics[2] <= 0)
                {
                    MessageBox.Show("Time Taken must be greater than zero.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                double durationHours = metrics[2] / 60.0; // Convert minutes to hours

                // Step 3: Calculate calories burned
                double burnedCalories = CalculateBurnedCalories(metrics, calculationFactors, metValue, userWeight, durationHours);

                // Step 4: Insert record into the database
                int recordId = db.InsertRecords(burnedCalories, activityId, intensity);
                if (recordId <= 0)
                {
                    MessageBox.Show("Failed to insert the record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Step 5: Insert metrics into the database
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
            finally
            {
                db?.CloseConnection();
            }
        }
        
        private void LoadSwimmingGraph()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                DataTable swimmingGraphData = db.GetActivityGraphData(frmLogin.person.PersonID, 1); // 1 is Swimming Activity ID

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
                    gunaLineDataset1.BorderWidth = 2;
                    gunaLineDataset1.PointRadius = 4;

                    if (!chartSwimmingProgress.Datasets.Contains(gunaLineDataset1))
                    {
                        chartSwimmingProgress.Datasets.Add(gunaLineDataset1);
                    }

                    chartSwimmingProgress.Title.Text = "Calories Burn From Swimming";
                    chartSwimmingProgress.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading swimming graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        private void LoadSwimmingMetrics()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                // Fetch the swimming metrics data
                DataTable metricData = db.GetSwimmingMetricsOverTime(frmLogin.person.PersonID);

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
            finally
            {
                db.CloseConnection();
            }
        }
        private void LoadSwimmingSummary()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                // Fetch additional metrics like total laps, average heart rate, and total time
                DataTable summaryData = db.GetSwimmingSummary(frmLogin.person.PersonID);

                if (summaryData != null && summaryData.Rows.Count > 0)
                {
                    double totalLaps = summaryData.Rows[0]["TotalLaps"] != DBNull.Value ? Convert.ToDouble(summaryData.Rows[0]["TotalLaps"]) : 0.0;
                    double avgHeartRate = summaryData.Rows[0]["AvgHeartRate"] != DBNull.Value ? Convert.ToDouble(summaryData.Rows[0]["AvgHeartRate"]) : 0.0;
                    double totalTime = summaryData.Rows[0]["TotalTime"] != DBNull.Value ? Convert.ToDouble(summaryData.Rows[0]["TotalTime"]) : 0.0;

                    lblTotalLaps.Text = $"Total Laps: {totalLaps}";
                    lblAvgHeartRate.Text = $"Average Heart Rate: {Math.Round(avgHeartRate, 2)} bpm";
                    lblTotalTime.Text = $"Total Time: {Math.Round(totalTime, 2)} minutes";
                }
                else
                {
                    lblTotalLaps.Text = "Total Laps: 0";
                    lblAvgHeartRate.Text = "Average Heart Rate: N/A";
                    lblTotalTime.Text = "Total Time: 0 minutes";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading swimming summary: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db?.CloseConnection();
            }
        }
        private void LoadRecentSwimmingActivity()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                // Fetch the most recent swimming activity
                DataRow recentActivity = db.GetRecentSwimmingActivity(frmLogin.person.PersonID);


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
            finally
            {
                db?.CloseConnection();
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
                db = new ConnectionDB();
                db.OpenConnection();

                // Get max calories burned for swimming
                double maxCaloriesForSwimming = db.GetMaxCaloriesForActivity(frmLogin.person.PersonID, 1); // 1 is Swimming activity ID
                lblMaxCalories.Text = $"Maximum Calories Burned: {maxCaloriesForSwimming} kcal";


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