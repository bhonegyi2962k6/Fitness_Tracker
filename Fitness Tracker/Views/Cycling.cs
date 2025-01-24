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
    public partial class frmCycling : UserControl
    {
        private readonly ConnectionDB db;
        public frmCycling()
        {
            InitializeComponent();
            db = ConnectionDB.GetInstance(); // Use the Singleton instance
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
                MessageBox.Show("Please enter a valid positive number for Speed (km/h).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, speed, distance, rideDuration, intensity);
            }

            // Validate Distance
            if (!double.TryParse(txtCyclingDistance.Text, out distance) || distance <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for Distance (km).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, speed, distance, rideDuration, intensity);
            }

            // Validate Ride Duration
            if (!double.TryParse(txtCyclingDuration.Text, out rideDuration) || rideDuration <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for Ride Duration (minutes).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, speed, distance, rideDuration, intensity);
            }

            // Validate Intensity
            if (cboIntensity.SelectedItem == null)
            {
                MessageBox.Show("Please select an intensity level.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, speed, distance, rideDuration, intensity);
            }

            intensity = cboIntensity.SelectedItem.ToString();

            // Intensity-based validation
            if (intensity == "Light" && (speed > 15 || distance > 10 || rideDuration > 30))
            {
                MessageBox.Show("For Light intensity, speed should be ≤ 15 km/h, distance ≤ 10 km, and duration ≤ 30 minutes. Please adjust your inputs or intensity level.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, speed, distance, rideDuration, intensity);
            }
            else if (intensity == "Moderate" && (speed < 16 || speed > 25 || distance > 20 || rideDuration > 60))
            {
                MessageBox.Show("For Moderate intensity, speed should be between 16 and 25 km/h, distance ≤ 20 km, and duration ≤ 60 minutes. Please adjust your inputs or intensity level.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, speed, distance, rideDuration, intensity);
            }
            else if (intensity == "Vigorous" && (speed < 26 || distance > 50 || rideDuration > 120))
            {
                MessageBox.Show("For Vigorous intensity, speed should be ≥ 26 km/h, distance ≤ 50 km, and duration ≤ 120 minutes. Please adjust your inputs or intensity level.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, speed, distance, rideDuration, intensity);
            }

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

                if (metValue <= 0)
                {
                    MessageBox.Show("Invalid MET value. Please check the selected intensity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var metValues = new MetValues(0, activity, intensity, metValue);

                // Step 3: Retrieve user's weight and calculate duration
                double userWeight = user.Weight;

                // Adjusted metric ID for "Ride Duration" to match Cycling's database definition (Metric ID = 9 for "Ride Duration")
                if (!metrics.ContainsKey(9) || metrics[9] <= 0)
                {
                    MessageBox.Show("Ride Duration must be greater than zero.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                double durationHours = metrics[9] / 60.0; // Convert Ride Duration (minutes) to hours
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

                        if (!db.InsertMetricValues(activityId, recordId, new Dictionary<int, double> { { metric.Key, metric.Value } }))
                        {
                            MessageBox.Show($"Failed to insert metric values for {metricObj.MetricName}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                MessageBox.Show("Cycling record successfully inserted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
        private void LoadCyclingGraph()
        {
            LoadActivityGraph(3, "Cycling", chartCyclingProgress); // Activity ID 1 is for Cycling
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

        private void LoadCyclingMetrics()
        {
            LoadActivityMetrics(3, "Cycling", chartCyclingMetrics); // Activity ID 3 is for Cycling
        }

        private void LoadCyclingSummary()
        {
            try
            {
                // Fetch raw metrics data for Cycling activity
                DataTable metricsData = db.GetActivityMetrics(frmLogin.user.PersonID, 3); // Assuming 3 is the Activity ID for Cycling

                if (metricsData != null && metricsData.Rows.Count > 0)
                {
                    double avgSpeed = 0.0;
                    double totalDistance = 0.0;
                    double totalTime = 0.0;
                    int speedCount = 0;

                    // Create an activity object for Cycling
                    var cyclingActivity = new Activity
                    {
                        ActivityId = 3, // Cycling Activity ID
                        ActivityName = "Cycling"
                    };

                    // Process each row to calculate sums and averages
                    foreach (DataRow row in metricsData.Rows)
                    {
                        string metricName = row["metric_name"].ToString();
                        double value = Convert.ToDouble(row["value"]);

                        var metric = new Metric
                        {
                            MetricName = metricName,
                            Activity = cyclingActivity
                        };

                        var metricValue = new MetricValues
                        {
                            Metric = metric,
                            Value = value,
                            Activity = cyclingActivity
                        };

                        // Assign values based on metric names
                        if (metricValue.Metric.MetricName == "Speed")
                        {
                            avgSpeed += metricValue.Value;
                            speedCount++;
                        }
                        else if (metricValue.Metric.MetricName == "Distance")
                        {
                            totalDistance += metricValue.Value;
                        }
                        else if (metricValue.Metric.MetricName == "Ride Duration")
                        {
                            totalTime += metricValue.Value;
                        }
                    }

                    // Calculate average speed if data exists
                    avgSpeed = speedCount > 0 ? avgSpeed / speedCount : 0.0;

                    // Update UI labels
                    lblAvgSpeed.Text = $"Average Speed: {Math.Round(avgSpeed, 2)} Km/h";
                    lblTotalDistance.Text = $"Total Distance: {Math.Round(totalDistance, 2)} km";
                    lblTotalTime.Text = $"Total Duration: {Math.Round(totalTime, 2)} minutes";
                }
                else
                {
                    // Set default values if no data exists
                    lblAvgSpeed.Text = "Average Speed: N/A";
                    lblTotalDistance.Text = "Total Distance: 0 km";
                    lblTotalTime.Text = "Total Duration: 0 minutes";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading cycling summary: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                // Create objects
                var user = User.GetInstance();

                var activity = new Activity
                {
                    ActivityId = 3, // Swimmin=Activity ID
                    ActivityName = "Cycling",
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

        private void frmCycling_Load(object sender, EventArgs e)
        {
            DisplayActivityScheduleReminder(3, "Cycling"); // 3 = Cycling Activity ID
            LoadCyclingGraph();
            LoadCyclingMetrics();
            LoadCyclingSummary();
            LoadCyclingTips();
            LoadHistoricalComparisonGraph();
            LoadCyclingInsights();
        }

        private void cboIntensity_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCyclingTips();
        }

        private void chartCyclingProgress_Load(object sender, EventArgs e)
        {

        }
    }
}
