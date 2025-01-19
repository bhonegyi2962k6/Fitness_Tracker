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
    public partial class frmCycling : UserControl
    {
        private ConnectionDB db;
        public frmCycling()
        {
            InitializeComponent();
        }

        private void btnCyclingRecord_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                var (isValid, speed, distance, rideDuration, intensity) = ValidateInputs();
                if (!isValid) return;

                // Prepare metrics
                var metrics = new Dictionary<int, double>
                {
                    { 7, speed },          // Metric ID: Speed
                    { 8, distance },       // Metric ID: Distance
                    { 9, rideDuration }    // Metric ID: Ride Duration
                };
                // Clear inputs after recording
                txtCyclingSpeed.Clear();
                txtCyclingDistance.Clear();
                txtCyclingDuration.Clear();
                cboIntensity.SelectedIndex = -1;

                // Display summary
                lblUserSummary.Text = $"You cycled at {speed} km/h, covered {distance} km in {rideDuration} minutes.";

                // Update graphs and metrics
                HandleActivityRecord(3, metrics, intensity); // Cycling Activity ID = 3
                LoadCyclingGraph();
                LoadCyclingMetrics();
                LoadCyclingSummary();
                LoadRecentCyclingActivity();
                LoadHistoricalComparisonGraph();
                LoadCyclingInsights();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private (bool isValid, double speed, double distance, double rideDuration, string intensity) ValidateInputs()
        {
            double speed = 0, distance = 0, rideDuration = 0;
            string intensity = string.Empty;

            // Validate Speed
            if (!double.TryParse(txtCyclingSpeed.Text, out speed) || speed <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for Speed.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, speed, distance, rideDuration, intensity);
            }

            // Validate Distance
            if (!double.TryParse(txtCyclingDistance.Text, out distance) || distance <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for Distance.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, speed, distance, rideDuration, intensity);
            }

            // Validate Ride Duration
            if (!double.TryParse(txtCyclingDuration.Text, out rideDuration) || rideDuration <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for Ride Duration.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, speed, distance, rideDuration, intensity);
            }

            // Validate Intensity
            if (cboIntensity.SelectedItem == null)
            {
                MessageBox.Show("Please select an intensity level.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, speed, distance, rideDuration, intensity);
            }

            intensity = cboIntensity.SelectedItem.ToString();
            return (true, speed, distance, rideDuration, intensity);
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
                double durationHours = metrics[9] / 60; // Convert duration from minutes to hours

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
                    MessageBox.Show("Cycling record successfully inserted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        private void LoadCyclingGraph()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                DataTable cyclingGraphData = db.GetActivityGraphData(frmLogin.person.PersonID, 3); 

                if (cyclingGraphData != null && cyclingGraphData.Rows.Count > 0)
                {
                    gunaLineDataset1.DataPoints.Clear();

                    foreach (DataRow row in cyclingGraphData.Rows)
                    {
                        string date = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd");
                        double calories = Convert.ToDouble(row["CaloriesBurned"]);

                        gunaLineDataset1.DataPoints.Add(date, calories);
                    }

                    gunaLineDataset1.Label = "Calories Burned Over Time";
                    gunaLineDataset1.BorderWidth = 2;
                    gunaLineDataset1.PointRadius = 4;

                    if (!chartCyclingProgress.Datasets.Contains(gunaLineDataset1))
                    {
                        chartCyclingProgress.Datasets.Add(gunaLineDataset1);
                    }

                    chartCyclingProgress.Title.Text = "Calories Burn From Cycling";
                    chartCyclingProgress.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading cycling graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        private void LoadCyclingMetrics()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                DataTable metricData = db.GetCyclingMetricsOverTime(frmLogin.person.PersonID);

                chartCyclingMetrics.Datasets.Clear();

                var speedDataset = new GunaLineDataset
                {
                    Label = "Speed (km/h)",
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

                var durationDataset = new GunaLineDataset
                {
                    Label = "Ride Duration (minutes)",
                    BorderWidth = 2,
                    PointRadius = 4,
                    BorderColor = Color.Red
                };

                if (metricData != null && metricData.Rows.Count > 0)
                {
                    foreach (DataRow row in metricData.Rows)
                    {
                        string date = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd");

                        double speed = Convert.ToDouble(row["Speed"]);
                        double distance = Convert.ToDouble(row["Distance"]);
                        double duration = Convert.ToDouble(row["RideDuration"]);

                        speedDataset.DataPoints.Add(date, speed);
                        distanceDataset.DataPoints.Add(date, distance);
                        durationDataset.DataPoints.Add(date, duration);
                    }

                    chartCyclingMetrics.Datasets.Add(speedDataset);
                    chartCyclingMetrics.Datasets.Add(distanceDataset);
                    chartCyclingMetrics.Datasets.Add(durationDataset);

                    chartCyclingMetrics.Title.Text = "Cycling Metrics Over Time";
                    chartCyclingMetrics.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading cycling metrics chart: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        private void LoadCyclingSummary()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                DataTable summaryData = db.GetCyclingSummary(frmLogin.person.PersonID);

                if (summaryData != null && summaryData.Rows.Count > 0)
                {
                    double avgSpeed = summaryData.Rows[0]["Speed"] != DBNull.Value ? Convert.ToDouble(summaryData.Rows[0]["Speed"]) : 0.0;
                    double totalDistance = Convert.ToDouble(summaryData.Rows[0]["TotalDistance"]);
                    double totalTime = Convert.ToDouble(summaryData.Rows[0]["RideDuration"]);

                    lblAvgSpeed.Text = $"Average Speed: {Math.Round(avgSpeed, 2)} Km/h";
                    lblTotalDistance.Text = $"Total Distance: {totalDistance} km";
                    lblTotalTime.Text = $"Total Time: {totalTime} minutes";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading cycling summary: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }

        private void LoadRecentCyclingActivity()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                DataRow recentActivity = db.GetRecentCyclingActivity(frmLogin.person.PersonID);
                if (recentActivity != null)
                {
                    // Extract values from the DataRow
                    string date = Convert.ToDateTime(recentActivity["Date"]).ToString("yyyy-MM-dd");
                    double caloriesBurned = Convert.ToDouble(recentActivity["CaloriesBurned"]);

                    // Update labels with the data
                    lblRecentDate.Text = $"Date: {date}";
                    lblRecentSpeed.Text = $"Speed: {recentActivity["Speed"]} km/h";
                    lblRecentDistance.Text = $"Distance: {recentActivity["Distance"]} km";
                    lblRecentTime.Text = $"Ride Duration: {recentActivity["RideDuration"]} minutes";
                    lblRecentCalories.Text = $"Calories Burned: {caloriesBurned} kcal";
                }
                else
                {
                    // No recent activity found
                    lblRecentDate.Text = "Date: N/A";
                    lblRecentSpeed.Text = $"Speed: {recentActivity["Speed"]} km/h";
                    lblRecentDistance.Text = $"Distance: {recentActivity["Distance"]} km";
                    lblRecentTime.Text = $"Ride Duration: {recentActivity["RideDuration"]} minutes";
                    lblRecentCalories.Text = "Calories Burned: N/A";
                }
            
             
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recent cycling activity: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }

        private void LoadCyclingTips()
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
                        lblTips.Text = "Tip: Light cycling is great for recovery!";
                        break;
                    case "moderate":
                        lblTips.Text = "Tip: Moderate cycling improves stamina!";
                        break;
                    case "vigorous":
                        lblTips.Text = "Tip: Vigorous cycling burns more calories!";
                        break;
                    default:
                        lblTips.Text = "Tip: Cycling is great for cardio!";
                        break;
                }
            }
        }
        private void LoadCyclingInsights()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                // Get max calories burned for swimming
                double maxCaloriesForCycling = db.GetMaxCaloriesForActivity(frmLogin.person.PersonID, 3); // 1 is Cycling activity ID
                lblMaxCalories.Text = $"Maximum Calories Burned: {maxCaloriesForCycling} kcal";


                if (maxCaloriesForCycling > 0)
                {
                    lblMaxCalories.Text = $"Maximum Calories Burned: {maxCaloriesForCycling} kcal";
                }
                else
                {
                    lblMaxCalories.Text = "Maximum Calories Burned: N/A";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Cycling insights: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                DataTable comparisonData = db.GetHistoricalComparison(frmLogin.person.PersonID);

                chartHistoricalComparison.Datasets.Clear();

                var comparisonDataset = new GunaBarDataset { Label = "Calories Burned by Activity" };

                if (comparisonData != null && comparisonData.Rows.Count > 0)
                {
                    foreach (DataRow row in comparisonData.Rows)
                    {
                        comparisonDataset.DataPoints.Add(row["ActivityName"].ToString(), Convert.ToDouble(row["CaloriesBurned"]));
                    }

                    chartHistoricalComparison.Datasets.Add(comparisonDataset);
                    chartHistoricalComparison.Title.Text = "Historical Calories Burned Comparison";
                    chartHistoricalComparison.Update();
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
        private void frmCycling_Load(object sender, EventArgs e)
        {
            DisplayActivityScheduleReminder(3, "Cycling"); // 3 = Cycling Activity ID
            LoadCyclingGraph();
            LoadCyclingMetrics();
            LoadCyclingSummary();
            LoadCyclingTips();
            LoadRecentCyclingActivity();
            LoadHistoricalComparisonGraph();
            LoadCyclingInsights();
        }

        private void cboIntensity_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCyclingTips();
        }
    }
}
