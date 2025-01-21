using Fitness_Tracker.dao;
using PdfSharp.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
            cboDateRange.SelectedIndex = 0;
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
                    int recordId = Convert.ToInt32(row["Record Id"]);
                    string activityDetails = db?.GetActivityDetails(recordId) ?? "N/A";

                    dataGridViewRecord.Rows.Add(
                        row["Record Id"],          // Hidden Record ID
                        rowIndex++,                // Row number
                        row["Activity"].ToString(),
                        Convert.ToDateTime(row["Record Date"]).ToString("yyyy-MM-dd"),
                        row["Burned Calories"],
                        activityDetails,
                        row["Activity Type"],
                        "Delete"
                    );

                    totalCalories += Convert.ToDouble(row["Burned Calories"]);
                }

                lblTotalCalories.Text = $"Total Calories: {Math.Round(totalCalories, 3)}";
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
                        int recordId = Convert.ToInt32(dataGridViewRecord.Rows[e.RowIndex].Cells["colRecordId"].Value);

                      

                        if (db.DeleteRecord(recordId))
                        {
                            MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        string activityName = row["ActivityName"].ToString();
                        double caloriesBurned = Convert.ToDouble(row["CaloriesBurned"]);

                        comparisonDataset.DataPoints.Add(activityName, caloriesBurned);
                    }

                    // Add the dataset to the chart
                    chartActivityCalories.Datasets.Add(comparisonDataset);

                    // Customize the chart
                    chartActivityCalories.Title.Text = "Historical Calories Burned Comparison";
                    chartActivityCalories.Update();
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
                        int recordId = Convert.ToInt32(row["Record Id"]);
                        string activityDetails = db.GetActivityDetails(recordId); // Fetch details for this record

                        // Add data to DataGridView
                        dataGridViewRecord.Rows.Add(
                            row["Record Id"],          // Hidden Record ID
                            rowIndex++,                // Row number
                            row["Activity"].ToString(),
                            Convert.ToDateTime(row["Record Date"]).ToString("yyyy-MM-dd"),
                            row["Burned Calories"],    // Burned Calories
                            activityDetails,           // Activity Details
                            row["Activity Type"],      // Intensity Level (colActivityType)
                            "Delete"                   // Delete button text
                        );

                        // Add to total calories
                        totalCalories += Convert.ToDouble(row["Burned Calories"]);
                    }

                    // Display total calories
                    lblTotalCalories.Text = $"Total Calories: {Math.Round(totalCalories, 3)}";
                }
                else
                {
                    // No records found
                    dataGridViewRecord.Rows.Clear();
                    lblTotalCalories.Text = "Total Calories: 0";
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
                        dataGridViewRecord.Rows.Clear();
                        int rowIndex = 1;
                        double totalCalories = 0;

                        foreach (DataRow row in dt.Rows)
                        {
                            int recordId = Convert.ToInt32(row["Record Id"]);
                            string activityDetails = db.GetActivityDetails(recordId);

                            dataGridViewRecord.Rows.Add(
                                row["Record Id"],
                                rowIndex++,
                                row["Activity"].ToString(),
                                Convert.ToDateTime(row["Record Date"]).ToString("yyyy-MM-dd"),
                                row["Burned Calories"],
                                activityDetails,
                                row["Activity Type"],
                                "Delete"
                            );

                            totalCalories += Convert.ToDouble(row["Burned Calories"]);
                        }

                        lblTotalCalories.Text = $"Total Calories: {Math.Round(totalCalories, 3)}";
                    }
                    else
                    {
                        dataGridViewRecord.Rows.Clear();
                        lblTotalCalories.Text = "Total Calories: 0";
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

                Dictionary<int, string> activities = db.GetActivities();

                cboActivitySort.Items.Clear();
                cboActivitySort.Items.Add("All"); // Add "All" as the default option

                foreach (var activity in activities)
                {
                    cboActivitySort.Items.Add(activity.Value);
                }

                cboActivitySort.SelectedIndex = 0; // Default to "All"
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error populating activities: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    // Populate the dataset with data
                    foreach (DataRow row in dt.Rows)
                    {
                        string recordDate = Convert.ToDateTime(row["record_date"]).ToString("yyyy-MM-dd");
                        double totalCalories = Convert.ToDouble(row["total_calories"]);

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
