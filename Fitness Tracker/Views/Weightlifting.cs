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
    public partial class frmWeightlifting : UserControl
    {
        private ConnectionDB db;
        public frmWeightlifting()
        {
            InitializeComponent();
        }

        private void btnWeightliftingRecord_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                var (isValid, weightLifted, repetitions, setsCompleted, intensity) = ValidateInputs();
                if (!isValid) return;

                // Prepare metrics
                var metrics = new Dictionary<int, double>
                {
                    { 13, weightLifted },   // Metric ID: Weight Lifted
                    { 14, repetitions },    // Metric ID: Repetitions
                    { 15, setsCompleted }   // Metric ID: Sets Completed
                };
                // Clear input fields
                txtWeightliftingWeight.Clear();
                txtWeightliftingReps.Clear();
                txtWeightliftingSets.Clear();

                // Display summary
                lblUserSummary.Text = $"You lifted {weightLifted} kg for {setsCompleted} sets with {repetitions} reps per set.";

                HandleActivityRecord(5, metrics, intensity); // Weightlifting Activity ID = 5
                LoadWeightliftingGraph();
                LoadWeightliftingMetrics();
                LoadWeightliftingSummary();
                LoadRecentWeightliftingActivity();
                LoadHistoricalComparisonGraph();
                LoadWeightliftingInsights();
                LoadWeightliftingTips();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private (bool isValid, double weightLifted, int repetitions, int setsCompleted, string intensity) ValidateInputs()
        {
            double weightLifted = 0;
            int repetitions = 0;
            int setsCompleted = 0;
            string intensity = string.Empty;

            // Validate Weight Lifted
            if (!double.TryParse(txtWeightliftingWeight.Text, out weightLifted) || weightLifted <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for Weight Lifted.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, weightLifted, repetitions, setsCompleted, intensity);
            }

            // Validate Repetitions
            if (!int.TryParse(txtWeightliftingReps.Text, out repetitions) || repetitions < 0)
            {
                MessageBox.Show("Please enter a valid non-negative number for Repetitions.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, weightLifted, repetitions, setsCompleted, intensity);
            }

            // Validate Sets Completed
            if (!int.TryParse(txtWeightliftingSets.Text, out setsCompleted) || setsCompleted < 0)
            {
                MessageBox.Show("Please enter a valid non-negative number for Sets Completed.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, weightLifted, repetitions, setsCompleted, intensity);
            }

            // Validate Intensity
            if (cboIntensity.SelectedItem == null)
            {
                MessageBox.Show("Please select an intensity level.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, weightLifted, repetitions, setsCompleted, intensity);
            }

            intensity = cboIntensity.SelectedItem.ToString();
            return (true, weightLifted, repetitions, setsCompleted, intensity);
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
                double durationHours = metrics[14] / 60; // Repetitions (assume each repetition takes 1 minutes)

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
                    MessageBox.Show("Weightlifting record successfully inserted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        private void LoadWeightliftingGraph()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                DataTable graphData = db.GetActivityGraphData(frmLogin.person.PersonID, 5); // 5 is Weightlifting Activity ID

                if (graphData != null && graphData.Rows.Count > 0)
                {
                    gunaLineDataset1.DataPoints.Clear();

                    foreach (DataRow row in graphData.Rows)
                    {
                        string date = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd");
                        double calories = Convert.ToDouble(row["CaloriesBurned"]);

                        gunaLineDataset1.DataPoints.Add(date, calories);
                    }

                    gunaLineDataset1.Label = "Calories Burned Over Time";
                    gunaLineDataset1.BorderWidth = 2;
                    gunaLineDataset1.PointRadius = 4;

                    if (!chartWeightliftingProgress.Datasets.Contains(gunaLineDataset1))
                    {
                        chartWeightliftingProgress.Datasets.Add(gunaLineDataset1);
                    }

                    chartWeightliftingProgress.Title.Text = "Calories Burned from Weightlifting";
                    chartWeightliftingProgress.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading weightlifting graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }

        private void LoadWeightliftingMetrics()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                DataTable metricData = db.GetWeightliftingMetricsOverTime(frmLogin.person.PersonID);

                chartWeightliftingMetrics.Datasets.Clear();

                var weightDataset = new GunaLineDataset
                {
                    Label = "Weight Lifted (kg)",
                    BorderWidth = 2,
                    PointRadius = 4,
                    BorderColor = Color.Blue
                };

                var repetitionsDataset = new GunaLineDataset
                {
                    Label = "Repetitions",
                    BorderWidth = 2,
                    PointRadius = 4,
                    BorderColor = Color.Green
                };

                var setsDataset = new GunaLineDataset
                {
                    Label = "Sets Completed",
                    BorderWidth = 2,
                    PointRadius = 4,
                    BorderColor = Color.Red
                };

                if (metricData != null && metricData.Rows.Count > 0)
                {
                    foreach (DataRow row in metricData.Rows)
                    {
                        string date = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd");

                        double weight = Convert.ToDouble(row["WeightLifted"]);
                        double repetitions = Convert.ToDouble(row["Repetitions"]);
                        double sets = Convert.ToDouble(row["SetsCompleted"]);

                        weightDataset.DataPoints.Add(date, weight);
                        repetitionsDataset.DataPoints.Add(date, repetitions);
                        setsDataset.DataPoints.Add(date, sets);
                    }

                    chartWeightliftingMetrics.Datasets.Add(weightDataset);
                    chartWeightliftingMetrics.Datasets.Add(repetitionsDataset);
                    chartWeightliftingMetrics.Datasets.Add(setsDataset);

                    chartWeightliftingMetrics.Title.Text = "Weightlifting Metrics Over Time";
                    chartWeightliftingMetrics.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading weightlifting metrics: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }

        private void LoadWeightliftingSummary()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                // Fetch the summary data for weightlifting
                DataTable summaryData = db.GetWeightliftingSummary(frmLogin.person.PersonID);

                if (summaryData != null && summaryData.Rows.Count > 0)
                {
                    // Fetch and calculate metrics
                    double avgWeight = summaryData.Rows[0]["AvgWeight"] != DBNull.Value ? Convert.ToDouble(summaryData.Rows[0]["AvgWeight"]) : 0.0;
                    double totalReps = summaryData.Rows[0]["TotalRepetitions"] != DBNull.Value ? Convert.ToDouble(summaryData.Rows[0]["TotalRepetitions"]) : 0.0;
                    double totalSets = summaryData.Rows[0]["TotalSets"] != DBNull.Value ? Convert.ToDouble(summaryData.Rows[0]["TotalSets"]) : 0.0;

                    // Update labels with average and totals
                    lblAvgWeight.Text = $"Average Weight Lifted: {Math.Round(avgWeight, 2)} kg"; // Changed to Average
                    lblTotalReps.Text = $"Total Repetitions: {totalReps} Times";
                    lblTotalSets.Text = $"Total Sets: {totalSets} Times";
                }
                else
                {
                    // Handle case where no data is returned
                    lblAvgWeight.Text = "Average Weight Lifted: 0 kg";
                    lblTotalReps.Text = "Total Repetitions: 0 Times";
                    lblTotalSets.Text = "Total Sets: 0 Times";
                }
            }
            catch (Exception ex)
            {
                // Handle any errors
                MessageBox.Show($"Error loading weightlifting summary: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Close database connection
                db?.CloseConnection();
            }
        }

        private void LoadRecentWeightliftingActivity()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                DataRow recentActivity = db.GetRecentWeightliftingActivity(frmLogin.person.PersonID);

                if (recentActivity != null)
                {
                    string date = Convert.ToDateTime(recentActivity["Date"]).ToString("yyyy-MM-dd");
                    double weight = Convert.ToDouble(recentActivity["WeightLifted"]);
                    double reps = Convert.ToDouble(recentActivity["Repetitions"]);
                    double sets = Convert.ToDouble(recentActivity["SetsCompleted"]);
                    double calories = Convert.ToDouble(recentActivity["CaloriesBurned"]);

                    lblRecentDate.Text = $"Date: {date}";
                    lblRecentWeight.Text = $"Weight Lifted: {weight} kg";
                    lblRecentReps.Text = $"Repetitions: {reps}";
                    lblRecentSets.Text = $"Sets: {sets}";
                    lblRecentCalories.Text = $"Calories Burned: {calories} kcal";
                }
                else
                {
                    lblRecentDate.Text = "Date: N/A";
                    lblRecentWeight.Text = "Weight Lifted: N/A";
                    lblRecentReps.Text = "Repetitions: N/A";
                    lblRecentSets.Text = "Sets: N/A";
                    lblRecentCalories.Text = "Calories Burned: N/A";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recent weightlifting activity: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }

        private void LoadWeightliftingTips()
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
                        lblTips.Text = "Tip: Light weightlifting is great for beginners! Focus on proper form.";
                        break;
                    case "moderate":
                        lblTips.Text = "Tip: Moderate weightlifting builds strength and endurance. Stay consistent!";
                        break;
                    case "vigorous":
                        lblTips.Text = "Tip: Vigorous weightlifting is intense! Ensure proper recovery and hydration.";
                        break;
                    default:
                        lblTips.Text = "Tip: Weightlifting is excellent for building muscle and boosting metabolism!";
                        break;
                }
            }
        }
        private void LoadWeightliftingInsights()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                // Get max calories burned for weightlifting
                double maxCaloriesForWeightlifting = db.GetMaxCaloriesForActivity(frmLogin.person.PersonID, 5); // 5 is Weightlifting activity ID

                if (maxCaloriesForWeightlifting > 0)
                {
                    lblMaxCalories.Text = $"Maximum Calories Burned: {maxCaloriesForWeightlifting} kcal";
                }
                else
                {
                    lblMaxCalories.Text = "Maximum Calories Burned: N/A";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Weightlifting insights: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void frmWeightlifting_Load(object sender, EventArgs e)
        {
            DisplayActivityScheduleReminder(5, "Weightlifting"); // 5 = Weightlifting Activity ID
            LoadWeightliftingGraph();
            LoadWeightliftingMetrics();
            LoadWeightliftingSummary();
            LoadRecentWeightliftingActivity();
            LoadHistoricalComparisonGraph();
            LoadWeightliftingInsights();
            LoadWeightliftingTips();
        }

        private void cboIntensity_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadWeightliftingTips();
        }
    }
}
