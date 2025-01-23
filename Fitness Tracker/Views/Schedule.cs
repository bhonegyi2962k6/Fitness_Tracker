using Fitness_Tracker.dao;
using Fitness_Tracker.Entities;
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
        private readonly ConnectionDB db;
        public frmSchedule()
        {
            InitializeComponent();
            db = ConnectionDB.GetInstance(); // Use the Singleton instance
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
                DataTable dt = db.GetSchedules(frmLogin.user.PersonID);

                if (dt != null && dt.Rows.Count > 0)
                {
                    dataGridViewSchedule.Rows.Clear(); // Clear existing rows
                    int rowIndex = 1;

                    // List to hold Schedule and ScheduleActivity objects
                    List<ScheduleActivity> scheduleActivityList = new List<ScheduleActivity>();

                    foreach (DataRow row in dt.Rows)
                    {
                        // Create Schedule object
                        var schedule = new Schedule
                        {
                            ScheduleId = Convert.ToInt32(row["Schedule Id"]),
                            Person = User.GetInstance(), // Assuming logged-in user is the person
                            ScheduledDate = Convert.ToDateTime(row["Date"])
                        };

                        // Create Activity object
                        var activity = new Activity
                        {
                            ActivityName = row["Activity"].ToString()
                        };

                        // Create ScheduleActivity object
                        var scheduleActivity = new ScheduleActivity
                        {
                            Schedule = schedule,
                            Activity = activity,
                            StartTime = TimeSpan.Parse(row["Start Time"].ToString()),
                            DurationMinutes = Convert.ToInt32(row["Duration"])
                        };

                        // Add to the list
                        scheduleActivityList.Add(scheduleActivity);

                        // Populate DataGridView
                        dataGridViewSchedule.Rows.Add(
                            row["Schedule Id"],
                            rowIndex++, // Row number (for colNo)
                            scheduleActivity.Activity.ActivityName,
                            scheduleActivity.Schedule.ScheduledDate.ToString("yyyy-MM-dd"),
                            scheduleActivity.StartTime.ToString(@"hh\:mm"),
                            $"{scheduleActivity.DurationMinutes} minutes",
                            "Delete" // Text for the delete button
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
                // Fetch activities as a dictionary
                Dictionary<int, string> activities = db.GetActivities();

                // Clear the combo box and set data source
                cboActivity.Items.Clear();

                foreach (var activity in activities)
                {
                    // Add KeyValuePair objects to the combo box
                    cboActivity.Items.Add(new KeyValuePair<int, string>(activity.Key, activity.Value));
                }

                cboActivity.DisplayMember = "Value"; // Display the activity name
                cboActivity.ValueMember = "Key";    // Use the activity ID as the value
                cboActivity.SelectedIndex = 0;      // Default to the first activity
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
                // Validate combo box selection
                if (!(cboActivity.SelectedItem is KeyValuePair<int, string> selectedActivity))
                {
                    MessageBox.Show("Please select a valid activity.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Extract activity ID and name
                int activityId = selectedActivity.Key;
                string activityName = selectedActivity.Value;

                // Create Activity object
                Activity activity = new Activity
                {
                    ActivityId = activityId,
                    ActivityName = activityName
                };

                // Validate and process Start Time
                string startTimeInput = txtActivityStartTime.Text.Trim();

                // Handle cases like "14" (convert to "14:00")
                if (int.TryParse(startTimeInput, out int hour) && hour >= 0 && hour <= 23)
                {
                    startTimeInput = $"{hour:D2}:00";
                }

                // Try parsing the final input as TimeSpan
                if (!TimeSpan.TryParse(startTimeInput, out TimeSpan startTime))
                {
                    MessageBox.Show("Invalid Start Time. Please enter a valid time (e.g., 14:00 or 6:30).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate duration
                if (!int.TryParse(txtActivityDuration.Text, out int durationMinutes) || durationMinutes <= 0)
                {
                    MessageBox.Show("Please enter a valid duration in minutes.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check schedule limits
                int scheduleCount = db.GetScheduleCountForDate(frmLogin.user.PersonID, dtpScheduleDate.Value);
                if (scheduleCount >= 2)
                {
                    MessageBox.Show("You can only have a maximum of 2 schedules per day.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check for overlapping schedules
                if (db.IsOverlappingSchedule(frmLogin.user.PersonID, dtpScheduleDate.Value, startTime, durationMinutes))
                {
                    MessageBox.Show("This schedule overlaps with an existing one. Please select a different time.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create Schedule object
                Schedule schedule = new Schedule
                {
                    Person = frmLogin.user,
                    ScheduledDate = dtpScheduleDate.Value
                };

                // Insert schedule into database
                int scheduleId = db.InsertSchedule(frmLogin.user.PersonID, schedule.ScheduledDate);
                if (scheduleId == -1)
                {
                    MessageBox.Show("Failed to insert schedule.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                schedule.ScheduleId = scheduleId; // Assign the generated ID to the object

                // Create ScheduleActivity object
                ScheduleActivity scheduleActivity = new ScheduleActivity
                {
                    Schedule = schedule,
                    Activity = activity,
                    StartTime = startTime,
                    DurationMinutes = durationMinutes
                };

                // Insert schedule activity into database
                bool success = db.InsertScheduleActivity(scheduleActivity.Schedule.ScheduleId, scheduleActivity.Activity.ActivityId, scheduleActivity.StartTime, scheduleActivity.DurationMinutes);

                if (success)
                {
                    MessageBox.Show("Schedule successfully created!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSchedules(); // Refresh grid
                    LoadScheduleDistributionByTime();
                    LoadSchedules();

                    // Clear input fields
                    txtActivityStartTime.Text = string.Empty;
                    txtActivityDuration.Text = string.Empty;
                    cboActivity.SelectedIndex = 0; // Reset combo box selection
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

                        // Create a Schedule object for deletion
                        Schedule schedule = new Schedule
                        {
                            ScheduleId = scheduleId,
                            Person = frmLogin.user // Use the current logged-in user
                        };

                        // Delete the schedule from the database
                        if (db.DeleteSchedule(schedule.ScheduleId))
                        {
                            MessageBox.Show("Schedule deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Log the deletion with object creation
                            var scheduleActivity = new ScheduleActivity
                            {
                                Schedule = schedule
                            };

                            // Refresh UI components
                            LoadSchedules();
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
                // Fetch data for the graph
                DataTable dt = db.GetSchedulesSummary(frmLogin.user.PersonID);

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Clear existing data points
                    gunaBarDataset1.DataPoints.Clear();

                    // List to hold Schedule and ScheduleActivity objects
                    List<ScheduleActivity> scheduleActivities = new List<ScheduleActivity>();

                    foreach (DataRow row in dt.Rows)
                    {
                        string activityName = row["Activity"].ToString();
                        int totalSchedules = Convert.ToInt32(row["TotalSchedules"]);

                        // Create Activity object
                        var activity = new Activity
                        {
                            ActivityName = activityName
                        };

                        // Create Schedule object (dummy schedule for visualization purposes)
                        var schedule = new Schedule
                        {
                            Person = frmLogin.user,
                            ScheduledDate = DateTime.Now // Set to the current date or a placeholder
                        };

                        // Create ScheduleActivity object
                        var scheduleActivity = new ScheduleActivity
                        {
                            Schedule = schedule,
                            Activity = activity,
                            // StartTime and DurationMinutes are not relevant here, but can be added if needed
                        };

                        // Add to the list
                        scheduleActivities.Add(scheduleActivity);

                        // Add data points to the dataset
                        gunaBarDataset1.DataPoints.Add(activityName, totalSchedules);
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
                DataTable dt = db.GetFilteredSchedules(frmLogin.user.PersonID, dateFilter, dateEnd);

                if (dt != null && dt.Rows.Count > 0)
                {
                    dataGridViewSchedule.Rows.Clear(); // Clear existing rows
                    int rowIndex = 1;

                    // List to hold Schedule and ScheduleActivity objects
                    List<ScheduleActivity> scheduleActivities = new List<ScheduleActivity>();

                    foreach (DataRow row in dt.Rows)
                    {
                        // Create Schedule object
                        var schedule = new Schedule
                        {
                            ScheduleId = Convert.ToInt32(row["Schedule Id"]),
                            Person = frmLogin.user, // Assuming the logged-in user is the person
                            ScheduledDate = Convert.ToDateTime(row["Date"])
                        };

                        // Create Activity object
                        var activity = new Activity
                        {
                            ActivityName = row["Activity"].ToString()
                        };

                        // Create ScheduleActivity object
                        var scheduleActivity = new ScheduleActivity
                        {
                            Schedule = schedule,
                            Activity = activity,
                            StartTime = TimeSpan.Parse(row["Start Time"].ToString()),
                            DurationMinutes = Convert.ToInt32(row["Duration"])
                        };

                        // Add to the list for potential further use
                        scheduleActivities.Add(scheduleActivity);

                        // Populate DataGridView
                        dataGridViewSchedule.Rows.Add(
                            schedule.ScheduleId,
                            rowIndex++, // Row number
                            scheduleActivity.Activity.ActivityName,
                            scheduleActivity.Schedule.ScheduledDate.ToString("yyyy-MM-dd"),
                            scheduleActivity.StartTime.ToString(@"hh\:mm"),
                            $"{scheduleActivity.DurationMinutes} minutes",
                            "Delete"
                        );
                    }

                    // Debug or further process objects if needed
                    Console.WriteLine($"Loaded {scheduleActivities.Count} schedules.");
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
        private void LoadSchedulesGraph(DateTime? dateFilter, DateTime? dateEnd)
        {
            try
            {
                DataTable dt = db.GetFilteredSchedules(frmLogin.user.PersonID, dateFilter, dateEnd);

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Clear the existing data points in the graph
                    gunaBarDataset1.DataPoints.Clear();

                    // List to hold ScheduleActivity objects
                    List<ScheduleActivity> scheduleActivityList = new List<ScheduleActivity>();

                    // Dictionary to track the counts of schedules for each activity
                    Dictionary<string, int> activityCounts = new Dictionary<string, int>();

                    foreach (DataRow row in dt.Rows)
                    {
                        // Create Schedule object
                        Schedule schedule = new Schedule
                        {
                            ScheduleId = Convert.ToInt32(row["Schedule Id"]),
                            Person = frmLogin.user,
                            ScheduledDate = Convert.ToDateTime(row["Date"])
                        };

                        // Create Activity object
                        Activity activity = new Activity
                        {
                            ActivityName = row["Activity"].ToString()
                        };

                        // Create ScheduleActivity object
                        ScheduleActivity scheduleActivity = new ScheduleActivity
                        {
                            Schedule = schedule,
                            Activity = activity,
                            StartTime = TimeSpan.Parse(row["Start Time"].ToString()),
                            DurationMinutes = Convert.ToInt32(row["Duration"])
                        };

                        // Add ScheduleActivity object to the list
                        scheduleActivityList.Add(scheduleActivity);

                        // Increment count for the activity
                        if (activityCounts.ContainsKey(scheduleActivity.Activity.ActivityName))
                        {
                            activityCounts[scheduleActivity.Activity.ActivityName]++;
                        }
                        else
                        {
                            activityCounts[scheduleActivity.Activity.ActivityName] = 1;
                        }
                    }

                    // Populate the dataset with the accumulated counts
                    foreach (var entry in activityCounts)
                    {
                        gunaBarDataset1.DataPoints.Add(entry.Key, entry.Value);
                    }

                    // Customize dataset appearance
                    gunaBarDataset1.Label = "Number of Schedules";
                    gunaBarDataset1.BorderWidth = 1;

                    // Add the dataset to the chart if not already added
                    if (!chartSchedules.Datasets.Contains(gunaBarDataset1))
                    {
                        chartSchedules.Datasets.Add(gunaBarDataset1);
                    }

                    // Customize chart title and update
                    chartSchedules.Title.Text = "Schedules by Activity";
                    chartSchedules.Update();
                }
                else
                {
                    // Clear the dataset if no data is returned
                    gunaBarDataset1.DataPoints.Clear();
                    chartSchedules.Update();

                    MessageBox.Show("No data available for the selected date range.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading schedules graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadScheduleDistributionByTime()
        {
            try
            {
                // Fetch data from the database
                DataTable dt = db.GetScheduleDistributionByTime(frmLogin.user.PersonID);

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Clear existing data points
                    gunaAreaDataset1.DataPoints.Clear();

                    // List to store ScheduleActivity objects
                    List<ScheduleActivity> scheduleActivityList = new List<ScheduleActivity>();

                    foreach (DataRow row in dt.Rows)
                    {
                        // Create Schedule object
                        Schedule schedule = new Schedule
                        {
                            Person = frmLogin.user // Assuming the current user is the person associated with the schedule
                        };

                        // Create a dummy Activity object (since the query doesn't provide activity details)
                        Activity activity = new Activity();

                        // Create ScheduleActivity object
                        ScheduleActivity scheduleActivity = new ScheduleActivity
                        {
                            Schedule = schedule,
                            Activity = activity,
                            StartTime = TimeSpan.FromHours(Convert.ToInt32(row["Hour"])),
                            DurationMinutes = 0 // Duration is not relevant in this context
                        };

                        // Add the ScheduleActivity object to the list
                        scheduleActivityList.Add(scheduleActivity);

                        // Add data points to the chart
                        string hourLabel = $"{Convert.ToInt32(row["Hour"]):D2}:00"; // Format hour as "HH:00"
                        int count = Convert.ToInt32(row["Count"]);
                        gunaAreaDataset1.DataPoints.Add(hourLabel, count);
                    }

                    // Customize dataset appearance
                    gunaAreaDataset1.Label = "Schedules by Time";
                    gunaAreaDataset1.BorderWidth = 1;
                    gunaAreaDataset1.BorderColor = Color.Blue;
                }

                // Add the dataset to the chart if not already added
                if (!chartScheduleByTime.Datasets.Contains(gunaAreaDataset1))
                {
                    chartScheduleByTime.Datasets.Add(gunaAreaDataset1);
                }

                // Customize chart title and update
                chartScheduleByTime.Title.Text = "Schedule Distribution by Time";
                chartScheduleByTime.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading schedule distribution graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
