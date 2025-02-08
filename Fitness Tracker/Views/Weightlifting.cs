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

namespace Fitness_Tracker.Views
{
    public partial class frmWeightlifting : UserControl
    {
        private readonly ConnectionDB db;
        public frmWeightlifting()
        {
            InitializeComponent();
            db = ConnectionDB.GetInstance(); // Use the Singleton instance
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
                LoadHistoricalComparisonGraph();
                LoadWeightliftingSummary();
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
                MessageBox.Show("Please select an Activity type.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                // Step 1: Create objects
                var user = User.GetInstance();

                var activity = new Activity
                {
                    ActivityId = activityId
                };

                var record = new Record
                {
                    Person = user,
                    Activity = activity,
                    RecordDate = DateTime.Now,
                    IntensityLevel = intensity
                };

                // Step 2: Retrieve calculation factors and MET value
                var calculationFactors = db.GetCalculationFactors(activityId);
                double metValue = db.GetMetValue(activityId, intensity);

                // Create MetValues object
                var metValues = new MetValues(0, activity, intensity, metValue);

                // Validate MET value
                if (metValues.MetValue <= 0)
                {
                    MessageBox.Show("Invalid MET value. Please check the selected intensity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Step 3: Retrieve user's weight and calculate calories burned
                double userWeight = user.Weight;

                // Check if "Repetitions" (Metric ID = 14) is valid
                if (!metrics.ContainsKey(14) || metrics[14] <= 0)
                {
                    MessageBox.Show("Repetitions must be greater than zero.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                double durationHours = metrics[14] / 60.0; // Assuming 1 minute per repetition
                record.BurnedCalories = CalculateBurnedCalories(metrics, calculationFactors, metValues.MetValue, userWeight, durationHours);

                // Step 4: Insert the record into the database
                int recordId = db.InsertRecords(record.BurnedCalories, activityId, intensity);
                if (recordId <= 0)
                {
                    MessageBox.Show("Failed to insert the record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update the record with the generated recordId
                record.RecordId = recordId;

                // Step 5: Insert metric values into the database
                foreach (var metric in metrics)
                {
                    if (calculationFactors.TryGetValue(metric.Key, out double factor))
                    {
                        // Create Metric and MetricValues objects
                        var metricObj = new Metric(metric.Key, activity, string.Empty, factor);
                        var metricValueObj = new MetricValues(0, activity, record, metricObj, metric.Value);

                        if (!db.InsertMetricValues(activityId, recordId, new Dictionary<int, double> { { metric.Key, metric.Value } }))
                        {
                            MessageBox.Show($"Failed to insert metric values for Metric ID {metric.Key}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                MessageBox.Show($"Weightlifting performance was recorded successfully! You burned {record.BurnedCalories} calories.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                var userRecord = new UserRecord
                {
                    Person = user,
                    Record = record
                };

                // Further processing of UserRecord (if needed)
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadActivityGraph(int activityId, string activityName, Guna.Charts.WinForms.GunaChart chart)
        {
            try
            {
                // Step 1: Create objects
                var user = User.GetInstance();
                var activity = new Activity
                {
                    ActivityId = activityId,
                    ActivityName = activityName
                };

                // Step 2: Retrieve data from the database
                DataTable activityGraphData = db.GetActivityGraphData(user.PersonID, activity.ActivityId);

                // Step 3: Process data and populate the chart
                if (activityGraphData != null && activityGraphData.Rows.Count > 0)
                {
                    // Create a dataset for the activity
                    var activityDataset = new GunaLineDataset
                    {
                        Label = $"{activity.ActivityName} - Calories Burned Over Time",
                        BorderWidth = 2,
                        PointRadius = 4,
                    };

                    foreach (DataRow row in activityGraphData.Rows)
                    {
                        string date = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd");
                        double calories = Convert.ToDouble(row["CaloriesBurned"]);

                        // Create a Record object to represent the data
                        var record = new Record
                        {
                            RecordDate = Convert.ToDateTime(row["Date"]),
                            BurnedCalories = calories,
                            Activity = activity,
                            Person = user
                        };

                        // Add data to the dataset
                        activityDataset.DataPoints.Add(record.RecordDate.ToString("yyyy-MM-dd"), record.BurnedCalories);
                    }

                    // Clear existing datasets and add the new dataset
                    chart.Datasets.Clear();
                    chart.Datasets.Add(activityDataset);

                    // Customize the chart title
                    chart.Title.Text = $"Calories Burned from {activity.ActivityName}";
                    chart.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading {activityName} graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadWeightliftingGraph()
        {
            LoadActivityGraph(5, "Weightlifting", chartWeightliftingProgress); // Activity ID 5 is for Weightlifting
        }
        private void LoadActivityMetrics(int activityId, string activityName, Guna.Charts.WinForms.GunaChart chart)
        {
            try
            {
                // Step 1: Create objects
                var user = User.GetInstance();
                var activity = new Activity
                {
                    ActivityId = activityId,
                    ActivityName = activityName
                };

                // Step 2: Fetch metrics data from the database
                DataTable metricData = db.GetActivityMetricsOverTime(user.PersonID, activity.ActivityId);

                // Step 3: Clear existing datasets from the chart
                chart.Datasets.Clear();

                // Dictionary to hold datasets for each metric
                var datasets = new Dictionary<string, GunaLineDataset>();

                // Step 4: Process the data and populate the chart
                if (metricData != null && metricData.Rows.Count > 0)
                {
                    foreach (DataRow row in metricData.Rows)
                    {
                        string date = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd");
                        string metricName = row["MetricName"].ToString();
                        double value = Convert.ToDouble(row["Value"]);

                        // Create a Metric and MetricValue object for better representation
                        var metric = new Metric
                        {
                            MetricName = metricName,
                            Activity = activity
                        };

                        var metricValue = new MetricValues
                        {
                            Metric = metric,
                            Value = value,
                            Record = new Record
                            {
                                RecordDate = Convert.ToDateTime(row["Date"]),
                                Person = user,
                                Activity = activity
                            }
                        };

                        // If the dataset for this metric doesn't exist, create it
                        if (!datasets.ContainsKey(metricValue.Metric.MetricName))
                        {
                            datasets[metricValue.Metric.MetricName] = new GunaLineDataset
                            {
                                Label = metricValue.Metric.MetricName,
                                BorderWidth = 2,
                                PointRadius = 4,
                                BorderColor = Color.FromArgb((datasets.Count * 40) % 255, (datasets.Count * 80) % 255, (datasets.Count * 120) % 255)
                            };
                        }

                        // Add the data point to the dataset
                        datasets[metricValue.Metric.MetricName].DataPoints.Add(metricValue.Record.RecordDate.ToString("yyyy-MM-dd"), metricValue.Value);
                    }

                    // Add all datasets to the chart
                    foreach (var dataset in datasets.Values)
                    {
                        chart.Datasets.Add(dataset);
                    }

                    // Step 5: Customize the chart title and update it
                    chart.Title.Text = $"{activity.ActivityName} Metrics Over Time";
                    chart.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading {activityName} metrics chart: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadWeightliftingMetrics()
        {
            LoadActivityMetrics(5, "Weightlifting", chartWeightliftingMetrics); // Activity ID 5 is for Weightlifting
        }
        private void LoadActivitySummary(int activityId, string activityName)
        {
            try
            {
                // Fetch raw metrics data
                DataTable metricsData = db.GetActivityMetrics(frmLogin.user.PersonID, activityId);

                if (metricsData != null && metricsData.Rows.Count > 0)
                {
                    // Create an activity object
                    var activity = new Activity
                    {
                        ActivityId = activityId,
                        ActivityName = activityName
                    };

                    // Initialize metric-related variables
                    double totalWeightLifted = 0.0; // For "Weight Lifted"
                    double totalRepetitions = 0.0;  // For "Repetitions"
                    double totalSets = 0.0;         // For "Sets"
                    int weightLiftedCount = 0;

                    // Process each row and create objects for each metric
                    foreach (DataRow row in metricsData.Rows)
                    {
                        var metric = new Metric
                        {
                            MetricName = row["metric_name"].ToString()
                        };

                        double value = Convert.ToDouble(row["value"]);
                        var metricValue = new MetricValues
                        {
                            Metric = metric,
                            Value = value,
                            Activity = activity
                        };

                        // Sum up or calculate metrics based on the metric name
                        if (metricValue.Metric.MetricName == "Weight Lifted")
                        {
                            totalWeightLifted += metricValue.Value;
                            weightLiftedCount++;
                        }
                        else if (metricValue.Metric.MetricName == "Repetitions")
                        {
                            totalRepetitions += metricValue.Value;
                        }
                        else if (metricValue.Metric.MetricName == "Sets Completed")
                        {
                            totalSets += metricValue.Value;
                        }
                    }

                    // Calculate averages for Weight Lifted
                    double avgWeightLifted = weightLiftedCount > 0 ? totalWeightLifted / weightLiftedCount : 0.0;

                    // Update UI labels
                    lblAvgWeightLifted.Text = $"Average Weight Lifted: {Math.Round(avgWeightLifted, 2)} kg";
                    lblTotalReps.Text = $"Total Repetitions: {Math.Round(totalRepetitions, 2)}";
                    lblTotalSets.Text = $"Total Sets: {Math.Round(totalSets, 2)}";
                }
                else
                {
                    // No data available
                    lblAvgWeightLifted.Text = "Average Weight Lifted: 0 kg";
                    lblTotalReps.Text = "Total Repetitions: 0";
                    lblTotalSets.Text = "Total Sets: 0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading {activityName} summary: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadWeightliftingSummary()
        {
            LoadActivitySummary(
                5, // Activity ID for Weightlifting
                "Weightlifting"
            );
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
                // Create objects
                var user = User.GetInstance();

                var activity = new Activity
                {
                    ActivityId = 5, // Weightlifting Activity ID
                    ActivityName = "Weightlifting",
                };

                // Retrieve maximum calories burned for the activity
                double maxCaloriesForWeightlifting = db.GetMaxCaloriesForActivity(user.PersonID, activity.ActivityId);

                // Update UI
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
                MessageBox.Show($"Error loading weightlifting insights: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                // Create user and activity objects
                var user = User.GetInstance();

                var activity = new Activity
                {
                    ActivityId = activityId,
                    ActivityName = activityName
                };

                // Fetch schedules from the database
                DataTable scheduleData = db.GetUpcomingActivitySchedules(user.PersonID, activity.ActivityId);

                if (scheduleData != null && scheduleData.Rows.Count > 0)
                {
                    foreach (DataRow row in scheduleData.Rows)
                    {
                        // Create schedule object for each row
                        var schedule = new Schedule
                        {
                            Person = user,
                            ScheduledDate = Convert.ToDateTime(row["Date"])
                        };

                        var scheduleActivity = new ScheduleActivity
                        {
                            Schedule = schedule,
                            Activity = activity,
                            StartTime = TimeSpan.Parse(row["StartTime"].ToString()),
                            DurationMinutes = Convert.ToInt32(row["Duration"])
                        };

                        // Display today's schedule first
                        if (schedule.ScheduledDate.Date == DateTime.Today)
                        {
                            lblScheduleReminder.Text = $"Today's {activity.ActivityName} Schedule: {scheduleActivity.StartTime:hh\\:mm} for {scheduleActivity.DurationMinutes} minutes.";
                            lblScheduleReminder.ForeColor = Color.Green;
                            return;
                        }
                    }

                    // Display the next upcoming schedule
                    DataRow upcomingRow = scheduleData.Rows[0];
                    var nextSchedule = new Schedule
                    {
                        Person = user,
                        ScheduledDate = Convert.ToDateTime(upcomingRow["Date"])
                    };

                    var nextScheduleActivity = new ScheduleActivity
                    {
                        Schedule = nextSchedule,
                        Activity = activity,
                        StartTime = TimeSpan.Parse(upcomingRow["StartTime"].ToString()),
                        DurationMinutes = Convert.ToInt32(upcomingRow["Duration"])
                    };

                    lblScheduleReminder.Text = $"Next {activity.ActivityName} Schedule: {nextSchedule.ScheduledDate:yyyy-MM-dd} at {nextScheduleActivity.StartTime:hh\\:mm} for {nextScheduleActivity.DurationMinutes} minutes.";
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
        private void frmWeightlifting_Load(object sender, EventArgs e)
        {
            DisplayActivityScheduleReminder(5, "Weightlifting"); // 5 = Weightlifting Activity ID
            LoadWeightliftingGraph();
            LoadWeightliftingMetrics();
            LoadHistoricalComparisonGraph();
            LoadWeightliftingInsights();
            LoadWeightliftingSummary();
            LoadWeightliftingTips();
        }

        private void cboIntensity_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            LoadWeightliftingTips();
        }
    }
}
