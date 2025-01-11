using Fitness_Tracker.dao;
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
    public partial class frmMonitorActivity : UserControl
    {
        private ConnectionDB db;
        public frmMonitorActivity()
        {
            InitializeComponent();
            LoadRecords();
        }

        private void LoadRecords()
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                // Query to retrieve records
                DataTable dt = db.GetRecords(frmLogin.person.PersonID);

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
                    lblTotalCalories.Text = $"Total Calories: {totalCalories}";
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
            finally
            {
                db?.CloseConnection();
            }
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

                        db = new ConnectionDB();
                        db.OpenConnection();

                        if (db.DeleteRecord(recordId))
                        {
                            MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadRecords(); // Refresh the records
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
                    finally
                    {
                        if (db != null) db.CloseConnection();
                    }
                }
            }
        }
    }
}
