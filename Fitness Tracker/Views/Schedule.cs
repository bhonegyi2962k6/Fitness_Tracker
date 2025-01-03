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
            LoadSchedules();
            LoadActivities();
        }
        private void LoadSchedules()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                // Set the font for column headers
                dataGridViewSchedule.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 10, FontStyle.Bold);
                dataGridViewSchedule.ColumnHeadersDefaultCellStyle.ForeColor = Color.White; // Optional, for text color

                DataTable dt = db.GetSchedules(frmLogin.person.PersonID);

                dataGridViewSchedule.Rows.Clear(); // Clear previous rows

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        int rowIndex = dataGridViewSchedule.Rows.Add(
                            row["Activity"].ToString(),
                            Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd"),
                            TimeSpan.Parse(row["Start Time"].ToString()).ToString(@"hh\:mm"),
                            $"{row["Duration"]} minutes"
                        );

                        // Apply the font to the newly added row
                        DataGridViewRow addedRow = dataGridViewSchedule.Rows[rowIndex];
                        
                        addedRow.DefaultCellStyle.Font = new Font("Century Gothic", 9, FontStyle.Regular);
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
                if (db != null) db.CloseConnection();
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

                if (!TimeSpan.TryParse(txtActivityStartTime.Text, out TimeSpan startTime))
                {
                    MessageBox.Show("Please enter a valid start time.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        }
    }
}
