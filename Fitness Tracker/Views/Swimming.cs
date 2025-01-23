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
                    IntesityLevel = intensity
                };

                // Step 2: Retrieve calculation factors and MET value
                var calculationFactors = db.GetCalculationFactors(activityId);
                double metValue = db.GetMetValue(activityId, intensity);

                if (metValue <= 0)
                {
                    MessageBox.Show("Invalid MET value. Please check the selected intensity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var metValues = new MetValues(0, activity, intensity, metValue);

                // Step 3: Retrieve user's weight and calculate calories burned
                double userWeight = user.Weight;
                if (!metrics.ContainsKey(2) || metrics[2] <= 0) // Ensure "Time Taken" exists and is valid
                {
                    MessageBox.Show("Time Taken must be greater than zero.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                double durationHours = metrics[2] / 60.0; // Convert Time Taken (minutes) to hours
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

                // Step 5: Insert metric values into the database and create MetricValues objects
                foreach (var metric in metrics)
                {
                    if (calculationFactors.TryGetValue(metric.Key, out double factor))
                    {
                        var metricObj = new Metric
                        {
                            MetricId = metric.Key,
                            Activity = activity,
                            CalculationFactor = factor
                        };

                        var metricValueObj = new MetricValues
                        {
                            Activity = activity,
                            Record = record,
                            Metric = metricObj,
                            Value = metric.Value
                        };

                        // Insert the metric value into the database
                        if (!db.InsertMetricValues(activityId, recordId, new Dictionary<int, double> { { metric.Key, metric.Value } }))
                        {
                            MessageBox.Show($"Failed to insert metric values for {metricObj.MetricName}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                MessageBox.Show($"{activity.ActivityName} record successfully inserted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Optional: Create and associate a UserRecord object
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
                else
                {
                    MessageBox.Show($"No data available for {activity.ActivityName} graph.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading {activityName} graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSwimmingGraph()
        {
            LoadActivityGraph(1, "Swimming", chartSwimmingProgress); // Activity ID 1 is for Swimming
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
                else
                {
                    MessageBox.Show($"No data available for {activity.ActivityName} metrics chart.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading {activityName} metrics chart: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSwimmingMetrics()
        {
            LoadActivityMetrics(1, "Swimming", chartSwimmingMetrics); // Activity ID 1 is for Swimming
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
                    double totalLaps = 0.0;
                    double totalTime = 0.0;
                    double avgHeartRate = 0.0;
                    int heartRateCount = 0;

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
                        if (metricValue.Metric.MetricName == "Laps")
                        {
                            totalLaps += metricValue.Value;
                        }
                        else if (metricValue.Metric.MetricName == "Time Taken")
                        {
                            totalTime += metricValue.Value;
                        }
                        else if (metricValue.Metric.MetricName == "Average Heart Rate")
                        {
                            avgHeartRate += metricValue.Value;
                            heartRateCount++;
                        }
                    }

                    // Calculate averages for heart rate
                    avgHeartRate = heartRateCount > 0 ? avgHeartRate / heartRateCount : 0.0;

                    // Update UI labels
                    lblTotalLaps.Text = $"Total Laps: {Math.Round(totalLaps, 2)}";
                    lblTotalTime.Text = $"Total Time: {Math.Round(totalTime, 2)} minutes";
                    lblAvgHeartRate.Text = $"Average Heart Rate: {Math.Round(avgHeartRate, 2)} bpm";
                }
                else
                {
                    // No data available
                    lblTotalLaps.Text = "Total Laps: 0";
                    lblTotalTime.Text = "Total Time: 0 minutes";
                    lblAvgHeartRate.Text = "Average Heart Rate: N/A";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading {activityName} summary: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSwimmingSummary()
        {
            LoadActivitySummary(
                1, // Activity ID for Swimming
                "Swimming"
            );
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
                // Create objects
                var user = User.GetInstance();

                var activity = new Activity
                {
                    ActivityId = 1, // Swimming Activity ID
                    ActivityName = "Swimming",
                };

                // Retrieve maximum calories burned for the activity
                double maxCaloriesForSwimming = db.GetMaxCaloriesForActivity(user.PersonID, activity.ActivityId);

                // Update UI
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


        private void frmSwimming_Load(object sender, EventArgs e)
        {
            DisplayActivityScheduleReminder(1, "Swimming"); // 1 = Swimming Activity ID
            LoadSwimmingInsights();
            LoadSwimmingTips();
            LoadSwimmingGraph();
            LoadSwimmingMetrics();
            LoadSwimmingSummary();
            LoadHistoricalComparisonGraph();
        }

        private void cboIntensity_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSwimmingTips();
        }
    }
} 