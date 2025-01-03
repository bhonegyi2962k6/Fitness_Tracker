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
    public partial class frmSwimming : UserControl
    {
        private ConnectionDB db;
        public frmSwimming()
        {
            InitializeComponent();
        }
        private void btnSwimmingRecord_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (!int.TryParse(txtSwimmingLaps.Text, out int laps) || laps < 0)
                {
                    MessageBox.Show("Please enter a valid non-negative number for Laps.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtSwimmingTime.Text, out double timeTaken) || timeTaken <= 0)
                {
                    MessageBox.Show("Please enter a valid positive number for Time Taken.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtSwimmingHeartRate.Text, out double averageHeartRate) || averageHeartRate <= 220)
                {
                    MessageBox.Show("Please enter a valid positive number for Average Heart Rate.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Dictionary<int, double> metrics = new Dictionary<int, double>
                    {
                        { 1, laps },               // Metric ID 1: Laps
                        { 2, timeTaken },          // Metric ID 2: Time Taken
                        { 3, averageHeartRate }    // Metric ID 3: Average Heart Rate
                    };

                HandleActivityRecord(1, metrics); // Swimming Activity ID = 1
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private double CalculateBurnedCalories(Dictionary<int, double> metrics, Dictionary<int, double> calculationFactors)
        {
            double totalBurnedCalories = 0;

            foreach (var metric in metrics)
            {
                if (calculationFactors.ContainsKey(metric.Key))
                {
                    totalBurnedCalories += metric.Value * calculationFactors[metric.Key];
                }
            }

            return totalBurnedCalories;
        }

        private void HandleActivityRecord(int activityId, Dictionary<int, double> metrics)
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                // Retrieve calculation factors for the activity
                Dictionary<int, double> calculationFactors = db.GetCalculationFactors(activityId);

                // Calculate burned calories
                double burnedCalories = CalculateBurnedCalories(metrics, calculationFactors);

                // Insert the main record
                if (db.InsertRecords(burnedCalories, activityId))
                {
                    // Insert metric values into the metric_values table
                    db.InsertMetricValues(activityId, metrics);

                    MessageBox.Show("Record successfully inserted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to insert record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (db != null)
                {
                    db.CloseConnection();
                }
            }

        }
    }
}
