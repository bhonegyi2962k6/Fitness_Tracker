using Fitness_Tracker.dao;
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
            LoadSchedules();
            LoadActivities();
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
                if (db != null) db.CloseConnection();
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
    }
}
