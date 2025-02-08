using Fitness_Tracker.dao;
using Fitness_Tracker.Entities;
using PdfSharp.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fitness_Tracker.Views
{
    public partial class frmMonitorActivity : UserControl
    {
        private readonly ConnectionDB db;
        public frmMonitorActivity()
        {
            InitializeComponent();
            db = ConnectionDB.GetInstance(); // Use the Singleton instance
            LoadRecords();
        }
        private void LoadRecords()
        {
            try
            {
                if (frmLogin.user == null)
                {
                    MessageBox.Show("User is not logged in. Please log in again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Fetch records from the database
                DataTable dt = db?.GetRecords(frmLogin.user.PersonID);

                if (dt == null || dt.Rows.Count == 0)
                {
                    dataGridViewRecord.Rows.Clear();
                    lblTotalCalories.Text = "Total Calories: 0";
                    return;
                }

                dataGridViewRecord.Rows.Clear();
                int rowIndex = 1;
                double totalCalories = 0;

                foreach (DataRow row in dt.Rows)
                {
                    // Create an Activity object
                    Activity activity = new Activity
                    {
                        ActivityName = row["Activity"].ToString(),
                        ActivityId = db.GetActivityIdByName(row["Activity"].ToString()) // Use method to fetch ActivityId
                    };

                    // Create a Record object
                    Record record = new Record
                    {
                        RecordId = Convert.ToInt32(row["Record Id"]),
                        Person = frmLogin.user, // Link the logged-in user
                        Activity = activity,
                        RecordDate = Convert.ToDateTime(row["Record Date"]),
                        BurnedCalories = Convert.ToDouble(row["Burned Calories"]),
                        IntensityLevel = row["Activity Type"].ToString()
                    };

                    // Add record to DataGridView
                    int currentRowIndex = dataGridViewRecord.Rows.Add(
                        record.RecordId,                 // Hidden Record ID
                        rowIndex++,                      // Row number
                        record.Activity.ActivityName,    // Activity name
                        record.RecordDate.ToString("yyyy-MM-dd"), // Record date
                        record.BurnedCalories,           // Burned calories
                        db.GetActivityDetails(record.RecordId) ?? "N/A", // activity details
                        record.IntensityLevel,            // Intensity level
                        "Delete"                         // Action column
                    );

                    // Add burned calories to total
                    totalCalories += record.BurnedCalories;
                }

                // Update total calories label
                lblTotalCalories.Text = $"Total Calories: {Math.Round(totalCalories, 3)} kal";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading records: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDefaultDailyCaloriesGraph()
        {
            DateTime startDate = DateTime.Today.AddDays(-7); // Example: Last 7 days
            DateTime endDate = DateTime.Today;

            // Call the actual method with the calculated range
            LoadDailyCaloriesGraph(startDate, endDate);
        }

        private void dataGridViewRecord_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridViewRecord.Columns["colRecordDelete"].Index && e.RowIndex >= 0)
            {
                DialogResult confirmResult = MessageBox.Show(
                    "Are you sure you want to delete this record?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (confirmResult == DialogResult.Yes)
                {
                    try
                    {
                        // Get record ID and activity name from the selected row
                        int recordId = Convert.ToInt32(dataGridViewRecord.Rows[e.RowIndex].Cells["colRecordId"].Value);
                        string activityName = dataGridViewRecord.Rows[e.RowIndex].Cells["colActivity"].Value.ToString();

                        // Create Activity object
                        var activity = new Activity
                        {
                            ActivityName = activityName
                        };

                        // Create Record object
                        var record = new Record
                        {
                            RecordId = recordId,
                            Activity = activity,
                            Person = User.GetInstance() // Get the current logged-in user
                        };

                        // Create UserRecord object
                        var userRecord = new UserRecord
                        {
                            Person = record.Person,
                            Record = record
                        };

                        // Delete the record from the database
                        if (db.DeleteRecord(record.RecordId))
                        {
                            MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Refresh the UI
                            DateTime startDate = DateTime.Today.AddDays(-7);
                            DateTime endDate = DateTime.Today;
                            LoadDailyCaloriesGraph(startDate, endDate);
                            LoadRecords(); // Refresh the records
                            LoadDefaultDailyCaloriesGraph();
                            LoadHistoricalComparisonGraph();
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete the record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void LoadHistoricalComparisonGraph()
        {
            try
            {
                // Fetch historical data for calories burned across all activities
                DataTable comparisonData = db.GetHistoricalComparison(frmLogin.user.PersonID);

                // Clear existing datasets
                chartActivityCalories.Datasets.Clear();

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
                    chartActivityCalories.Datasets.Add(comparisonDataset);

                    // Customize the chart
                    chartActivityCalories.Title.Text = "Historical Calories Burned Comparison";
                    chartActivityCalories.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading historical comparison graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
 
        private void frmMonitorActivity_Load(object sender, EventArgs e)
        {
            LoadDefaultDailyCaloriesGraph();
            LoadHistoricalComparisonGraph();
            PopulateActivitySort();
            dataGridViewRecord.ClearSelection();
        }

        private void LoadRecordsForDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Query to retrieve records within the date range
                DataTable dt = db.GetRecordsForDateRange(frmLogin.user.PersonID, startDate, endDate);

                if (dt != null && dt.Rows.Count > 0)
                {
                    dataGridViewRecord.Rows.Clear(); // Clear existing rows
                    int rowIndex = 1;
                    double totalCalories = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        // Create Activity object
                        var activity = new Activity
                        {
                            ActivityName = row["Activity"].ToString()
                        };

                        // Create Record object
                        var record = new Record
                        {
                            RecordId = Convert.ToInt32(row["Record Id"]),
                            Activity = activity,
                            RecordDate = Convert.ToDateTime(row["Record Date"]),
                            BurnedCalories = Convert.ToDouble(row["Burned Calories"]),
                            IntensityLevel = row["Activity Type"].ToString(),
                            Person = User.GetInstance()
                        };

                        // Get activity details and create Metric and MetricValues objects
                        string activityDetailsString = db.GetActivityDetails(record.RecordId);
                        var metricValuesList = new List<MetricValues>();

                        if (!string.IsNullOrEmpty(activityDetailsString))
                        {
                            string[] details = activityDetailsString.Split(new string[] { ", " }, StringSplitOptions.None); // Correct way to split by ", "
                            foreach (string detail in details)
                            {
                                string[] metricParts = detail.Split(':'); // Split(':') for char
                                if (metricParts.Length == 2)
                                {
                                    string metricName = metricParts[0].Trim();
                                    double metricValue = Convert.ToDouble(metricParts[1].Trim());

                                    var metric = new Metric
                                    {
                                        MetricName = metricName,
                                        Activity = activity
                                    };

                                    var metricValueObj = new MetricValues
                                    {
                                        Metric = metric,
                                        Record = record,
                                        Value = metricValue
                                    };

                                    metricValuesList.Add(metricValueObj);
                                }
                            }
                        }
                        // Add data to DataGridView
                        dataGridViewRecord.Rows.Add(
                            record.RecordId,              // Hidden Record ID
                            rowIndex++,                   // Row number
                            record.Activity.ActivityName,
                            record.RecordDate.ToString("yyyy-MM-dd"),
                            record.BurnedCalories,        // Burned Calories
                            activityDetailsString,        // Activity Details
                            record.IntensityLevel,         // Intensity Level (colActivityType)
                            "Delete"                      // Delete button text
                        );

                        // Add to total calories
                        totalCalories += record.BurnedCalories;
                    }

                    // Display total calories
                    lblTotalCalories.Text = $"Total Calories: {Math.Round(totalCalories, 3)} kal";
                }
                else
                {
                    // No records found
                    dataGridViewRecord.Rows.Clear();
                    lblTotalCalories.Text = "Total Calories: 0 kal";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading records: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void cboDateRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.Today;
            try
            {
                // Determine the selected date range
                switch (cboDateRange.SelectedItem?.ToString())
                {
                    case "Today":
                        startDate = DateTime.Today;
                        endDate = DateTime.Today;
                        break;
                    case "Yesterday":
                        startDate = DateTime.Today.AddDays(-1);
                        endDate = DateTime.Today.AddDays(-1);
                        break;
                    case "Last 3 Days":
                        startDate = DateTime.Today.AddDays(-2); // Includes today, yesterday, and the day before
                        endDate = DateTime.Today;
                        break;
                    case "Last 5 Days":
                        startDate = DateTime.Today.AddDays(-4); // Includes the last 5 days including today
                        endDate = DateTime.Today;
                        break;
                    case "Last Week":
                        startDate = DateTime.Today.AddDays(-7); // Last 7 days including today
                        endDate = DateTime.Today;
                        break;
                    case "Last Month":
                        startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1); // First day of last month
                        endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);    // Last day of last month
                        break;
                    default:
                        MessageBox.Show("Invalid date range selected. Please choose a valid option.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                }

                // Validate that the startDate is earlier than or equal to endDate
                if (startDate > endDate)
                {
                    MessageBox.Show("The selected date range is invalid. Start date must be before the end date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update the DataGridView and the graph with the selected date range
                LoadRecordsForDateRange(startDate, endDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while processing the date range selection: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboActivitySort_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string selectedActivity = cboActivitySort.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(selectedActivity) || selectedActivity == "All")
                {
                    // Load all records if "All" is selected
                    LoadRecords();
                }
                else
                {
                    // Get records for the selected activity
                    DataTable dt = db.GetRecordsByActivity(frmLogin.user.PersonID, selectedActivity);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        dataGridViewRecord.Rows.Clear(); // Clear the DataGridView
                        int rowIndex = 1;
                        double totalCalories = 0;

                        foreach (DataRow row in dt.Rows)
                        {
                            // Create Activity object
                            var activity = new Activity
                            {
                                ActivityId = db.GetActivityIdByName(selectedActivity), // Assuming this method exists in your DB class
                                ActivityName = row["Activity"].ToString()
                            };

                            // Create Record object
                            var record = new Record
                            {
                                RecordId = Convert.ToInt32(row["Record Id"]),
                                Activity = activity,
                                RecordDate = Convert.ToDateTime(row["Record Date"]),
                                BurnedCalories = Convert.ToDouble(row["Burned Calories"]),
                                IntensityLevel = row["Activity Type"].ToString()
                            };

                            // Fetch activity details
                            string activityDetails = db.GetActivityDetails(record.RecordId);

                            // Add record to DataGridView
                            dataGridViewRecord.Rows.Add(
                                record.RecordId,                   // Hidden Record ID
                                rowIndex++,                        // Row number
                                record.Activity.ActivityName,      // Activity name
                                record.RecordDate.ToString("yyyy-MM-dd"), // Record date
                                record.BurnedCalories,             // Burned calories
                                activityDetails,                   // Activity details
                                record.IntensityLevel,              // Intensity level
                                "Delete"                           // Delete button
                            );

                            // Update total calories
                            totalCalories += record.BurnedCalories;
                        }

                        // Display total calories
                        lblTotalCalories.Text = $"Total Calories: {Math.Round(totalCalories, 3)} kal";
                    }
                    else
                    {
                        // No records found
                        dataGridViewRecord.Rows.Clear();
                        lblTotalCalories.Text = "Total Calories: 0 kal";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sorting by activity: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PopulateActivitySort()
        {
            try
            {
                // Fetch activities from the database
                Dictionary<int, string> activityDictionary = db.GetActivities();

                // Clear the ComboBox
                cboActivitySort.Items.Clear();
                cboActivitySort.Items.Add("All"); // Add "All" as the default option

                // Create and store Activity objects
                List<Activity> activities = new List<Activity>();

                foreach (var entry in activityDictionary)
                {
                    var activity = new Activity
                    {
                        ActivityId = entry.Key,
                        ActivityName = entry.Value
                    };

                    // Add to the ComboBox
                    cboActivitySort.Items.Add(activity.ActivityName);

                    // Store the Activity object (if needed elsewhere)
                    activities.Add(activity);
                }

                // Set the default selection to "All"
                cboActivitySort.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error populating activities: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public class DailyCalories
        {
            public string RecordDate { get; set; }
            public double TotalCalories { get; set; }

            public DailyCalories() { }

            public DailyCalories(string recordDate, double totalCalories)
            {
                RecordDate = recordDate;
                TotalCalories = totalCalories;
            }
        }

        private void LoadDailyCaloriesGraph(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Get daily calories data
                DataTable dt = db.GetDailyCaloriesData(frmLogin.user.PersonID, startDate, endDate);

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Clear existing data points
                    gunaLineDataset1.DataPoints.Clear();

                    // Create a list to store DailyCalories objects
                    List<DailyCalories> dailyCaloriesList = new List<DailyCalories>();

                    // Populate the dataset with data and create objects
                    foreach (DataRow row in dt.Rows)
                    {
                        string recordDate = Convert.ToDateTime(row["record_date"]).ToString("yyyy-MM-dd");
                        double totalCalories = Convert.ToDouble(row["total_calories"]);

                        // Create DailyCalories object
                        var dailyCalories = new DailyCalories
                        {
                            RecordDate = recordDate,
                            TotalCalories = totalCalories
                        };

                        // Add to the list
                        dailyCaloriesList.Add(dailyCalories);

                        // Add data point to the dataset
                        gunaLineDataset1.DataPoints.Add(recordDate, totalCalories);
                    }

                    // Customize dataset appearance
                    gunaLineDataset1.Label = "Calories Burned";
                    gunaLineDataset1.BorderWidth = 2;

                    // Add the dataset to the chart (if not already added)
                    if (!chartDailyCalories.Datasets.Contains(gunaLineDataset1))
                    {
                        chartDailyCalories.Datasets.Add(gunaLineDataset1);
                    }

                    // Customize chart title and axis labels
                    chartDailyCalories.Title.Text = "Calories Burned Per Day";
                    chartDailyCalories.Update(); // Refresh the chart
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading daily calories graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
