using Fitness_Tracker.dao;
using Guna.Charts.WinForms;
using MySql.Data.MySqlClient;
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
    public partial class frmSchedule : UserControl
    {
        private ConnectionDB db;
        public frmSchedule()
        {
            InitializeComponent();
        }

        private void frmSchedule_Load(object sender, EventArgs e)
        {
            
                dtpScheduleDate.MinDate = DateTime.Today;
                dataGridViewSchedule.RowTemplate.Height = 45; // Adjust this value to your desired height
                dataGridViewSchedule.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

                LoadSchedules(); // Load the data into the DataGridView
                LoadSchedulesGraph();
                LoadScheduleDistributionByTime();
                SetupSortByDateComboBox();
                LoadActivities();

                lblTotalSchedules.Text = $"Total Schedules: {dataGridViewSchedule.Rows.Count}";

                // Clear selection after loading data
                dataGridViewSchedule.ClearSelection();
        }
        private void LoadSchedules()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                DataTable dt = db.GetSchedules(frmLogin.person.PersonID);

                if (dt != null && dt.Rows.Count > 0)
                {
                    dataGridViewSchedule.Rows.Clear(); // Clear existing rows
                    int rowIndex = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        dataGridViewSchedule.Rows.Add(row["Schedule Id"],
                            rowIndex++, // Row number (for colNo)
                            row["Activity"].ToString(),
                            Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd"),
                            TimeSpan.Parse(row["Start Time"].ToString()).ToString(@"hh\:mm"),
                            $"{row["Duration"]} minutes",
                            "Delete" // Text for the delete button
                             // Populate the hidden column
                        );
                    }
                }
                else
                {
                    dataGridViewSchedule.Rows.Clear(); // No data, ensure the grid is empty
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading schedules: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadActivities()
        {
            try
            {
                db = new ConnectionDB();
                Dictionary<int, string> activities = db.GetActivities();

                cboActivity.DataSource = new BindingSource(activities, null);
                cboActivity.DisplayMember = "Value";
                cboActivity.ValueMember = "Key";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading activities: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnMakeSchedule_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (cboActivity.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select an activity.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate and process Start Time
                string startTimeInput = txtActivityStartTime.Text.Trim();

                // Check if input is only an hour (like "6")
                if (int.TryParse(startTimeInput, out int hour) && hour >= 0 && hour <= 23)
                {
                    // Convert "6" to "06:00" (6 AM)
                    startTimeInput = $"{hour:D2}:00";
                }

                // Try parsing the final input as TimeSpan
                if (!TimeSpan.TryParse(startTimeInput, out TimeSpan startTime))
                {
                    MessageBox.Show("Invalid Start Time. Please enter a valid time (e.g., 6, 6:30, or 15:45).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtActivityDuration.Text, out int durationMinutes) || durationMinutes <= 0)
                {
                    MessageBox.Show("Please enter a valid duration in minutes.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                db = new ConnectionDB();
                db.OpenConnection();

                // Check the number of schedules the user has for the selected date
                int scheduleCount = db.GetScheduleCountForDate(frmLogin.person.PersonID, dtpScheduleDate.Value);

                if (scheduleCount >= 2)
                {
                    MessageBox.Show("You can only have a maximum of 2 schedules per day.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (db.IsOverlappingSchedule(frmLogin.person.PersonID, dtpScheduleDate.Value, startTime, durationMinutes))
                {
                    MessageBox.Show("This schedule overlaps with an existing one. Please select a different time.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Insert schedule
                int scheduleId = db.InsertSchedule(frmLogin.person.PersonID, dtpScheduleDate.Value);
                if (scheduleId == -1)
                {
                    MessageBox.Show("Failed to insert schedule.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Insert schedule activity
                int activityId = ((KeyValuePair<int, string>)cboActivity.SelectedItem).Key;
                bool success = db.InsertScheduleActivity(scheduleId, activityId, startTime, durationMinutes);

                if (success)
                {
                    MessageBox.Show("Schedule successfully created!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSchedules(); // Refresh grid
                    LoadScheduleDistributionByTime();

                    // Clear input fields
                    txtActivityStartTime.Text = string.Empty;
                    txtActivityDuration.Text = string.Empty;
                    cboActivity.SelectedIndex = -1; // Reset combo box selection
                    dtpScheduleDate.Value = DateTime.Today; // Reset to today's date
                }
                else
                {
                    MessageBox.Show("Failed to insert schedule activity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void dataGridViewSchedule_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.ColumnIndex == dataGridViewSchedule.Columns["colDelete"].Index && e.RowIndex >= 0)
            {
                DialogResult confirmResult = MessageBox.Show(
                    "Are you sure you want to delete this schedule?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );
                if (confirmResult == DialogResult.Yes)
                {
                    try
                    {
                        // Get schedule_id from the hidden column
                        int scheduleId = Convert.ToInt32(dataGridViewSchedule.Rows[e.RowIndex].Cells["colScheduledId"].Value);

                        ConnectionDB db = new ConnectionDB();

                        if (db.DeleteSchedule(scheduleId))
                        {
                            MessageBox.Show("Schedule deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadSchedules(); // Refresh the table after deletion
                            LoadSchedulesGraph();
                            LoadScheduleDistributionByTime();
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete the schedule.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error occurred while deleting the schedule: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadSchedulesGraph()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                // Fetch data for the graph
                DataTable dt = db.GetSchedulesSummary(frmLogin.person.PersonID);

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Clear existing data points
                    gunaBarDataset1.DataPoints.Clear();

                    // Populate the dataset with data
                    foreach (DataRow row in dt.Rows)
                    {
                        string activity = row["Activity"].ToString();
                        int totalSchedules = Convert.ToInt32(row["TotalSchedules"]);

                        // Add data points to the dataset
                        gunaBarDataset1.DataPoints.Add(activity, totalSchedules);
                    }

                    // Optionally customize the dataset's appearance
                    gunaBarDataset1.Label = "Number of Schedules";
                    gunaBarDataset1.BorderWidth = 1;
                }

                // Add the dataset to the chart (if not already added)
                if (!chartSchedules.Datasets.Contains(gunaBarDataset1))
                {
                    chartSchedules.Datasets.Add(gunaBarDataset1);
                }

                // Optional: Customize the chart's title
                chartSchedules.Title.Text = "Schedules by Activity";
                chartSchedules.Update(); // Refresh the chart
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading schedules graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }

        private void SetupSortByDateComboBox()
        {
            cboSortByDate.Items.Clear();

            // Add default options
            cboSortByDate.Items.Add("All Dates"); // Show all schedules
            cboSortByDate.Items.Add("Today");
            cboSortByDate.Items.Add("Yesterday");
            cboSortByDate.Items.Add("Last 7 Days");
            cboSortByDate.Items.Add("Last Month");

            cboSortByDate.SelectedIndex = 0; // Default to "All Dates"
        }

        private void cboSortByDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedDateRange = cboSortByDate.SelectedItem?.ToString();
            DateTime? startDate = null;
            DateTime? endDate = null;

            // Determine the date range based on selection
            switch (selectedDateRange)
            {
                case "Today":
                    startDate = DateTime.Today;
                    endDate = DateTime.Today;
                    break;
                case "Yesterday":
                    startDate = DateTime.Today.AddDays(-1);
                    endDate = DateTime.Today.AddDays(-1);
                    break;
                case "Last 7 Days":
                    startDate = DateTime.Today.AddDays(-7);
                    endDate = DateTime.Today;
                    break;
                case "Last Month":
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                    endDate = startDate.Value.AddMonths(1).AddDays(-1);
                    break;
                case "All Dates":
                default:
                    // No date filtering
                    startDate = null;
                    endDate = null;
                    break;
            }

            // Reload schedules and graph with the selected date range
            LoadSchedules(dateFilter: startDate, dateEnd: endDate);
            LoadSchedulesGraph(dateFilter: startDate, dateEnd: endDate);
        }

        private void LoadSchedules(DateTime? dateFilter, DateTime? dateEnd)
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                DataTable dt = db.GetFilteredSchedules(frmLogin.person.PersonID, dateFilter, dateEnd);

                if (dt != null && dt.Rows.Count > 0)
                {
                    dataGridViewSchedule.Rows.Clear(); // Clear existing rows
                    int rowIndex = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        dataGridViewSchedule.Rows.Add(row["Schedule Id"],
                            rowIndex++,
                            row["Activity"].ToString(),
                            Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd"),
                            TimeSpan.Parse(row["Start Time"].ToString()).ToString(@"hh\:mm"),
                            $"{row["Duration"]} minutes",
                            "Delete"
                        );
                    }
                }
                else
                {
                    dataGridViewSchedule.Rows.Clear(); // No data, ensure the grid is empty
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading schedules: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }

        private void LoadSchedulesGraph(DateTime? dateFilter, DateTime? dateEnd)
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                DataTable dt = db.GetFilteredSchedules(frmLogin.person.PersonID, dateFilter, dateEnd);

                if (dt != null && dt.Rows.Count > 0)
                {
                    dataGridViewSchedule.Rows.Clear(); // Clear existing rows
                    int rowIndex = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        dataGridViewSchedule.Rows.Add(row["Schedule Id"],
                            rowIndex++,
                            row["Activity"].ToString(),
                            Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd"),
                            TimeSpan.Parse(row["Start Time"].ToString()).ToString(@"hh\:mm"),
                            $"{row["Duration"]} minutes",
                            "Delete"
                        );
                    }
                }
                else
                {
                    dataGridViewSchedule.Rows.Clear(); // No data, ensure the grid is empty
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading schedules: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        private void LoadScheduleDistributionByTime()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                DataTable dt = db.GetScheduleDistributionByTime(frmLogin.person.PersonID);

                if (dt != null && dt.Rows.Count > 0)
                {
                    gunaAreaDataset1.DataPoints.Clear();

                    foreach (DataRow row in dt.Rows)
                    {
                        string hour = $"{Convert.ToInt32(row["Hour"]):D2}:00";
                        int count = Convert.ToInt32(row["Count"]);

                        gunaAreaDataset1.DataPoints.Add(hour, count);
                    }

                    gunaAreaDataset1.Label = "Schedules by Time";
                    gunaAreaDataset1.BorderWidth = 1;
                    gunaAreaDataset1.BorderColor = Color.Blue;
                }

                if (!chartScheduleByTime.Datasets.Contains(gunaAreaDataset1))
                {
                    chartScheduleByTime.Datasets.Add(gunaAreaDataset1);
                }

                chartScheduleByTime.Title.Text = "Schedule Distribution by Time";
                chartScheduleByTime.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading schedule distribution graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }

        private void dataGridViewSchedule_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewSchedule.SelectedRows.Count > 0)
            {
                // Get the first selected row
                DataGridViewRow selectedRow = dataGridViewSchedule.SelectedRows[0];

                string activity = selectedRow.Cells["colActivity"].Value?.ToString() ?? "N/A";
                string date = selectedRow.Cells["colScheduleDate"].Value?.ToString() ?? "N/A";
                string startTime = selectedRow.Cells["colScheduleStartTime"].Value?.ToString() ?? "N/A";
                string duration = selectedRow.Cells["colDuration"].Value?.ToString() ?? "N/A";

                lblScheduleSummary.Text = $"{activity} on {date} at {startTime} for {duration}.";
            }
            else
            {
                // Clear the label if no row is selected
                lblScheduleSummary.Text = "No schedule selected.";
            }

        }
    }
}
