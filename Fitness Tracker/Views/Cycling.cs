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
    public partial class frmCycling : UserControl
    {
        private ConnectionDB db;
        public frmCycling()
        {
            InitializeComponent();
        }

        private void btnCyclingRecord_Click(object sender, EventArgs e)
        {
            try
            {
                if (!double.TryParse(txtCyclingSpeed.Text, out double speed) || speed <= 0)
                {
                    MessageBox.Show("Please enter a valid positive number for Speed.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtCyclingDistance.Text, out double distance) || distance <= 0)
                {
                    MessageBox.Show("Please enter a valid positive number for Distance.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtCyclingDuration.Text, out double rideDuration) || rideDuration <= 0)
                {
                    MessageBox.Show("Please enter a valid positive number for Ride Duration.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Dictionary<int, double> metrics = new Dictionary<int, double>
                {
                    { 7, speed },          // Metric ID: Speed
                    { 8, distance },       // Metric ID: Distance
                    { 9, rideDuration }    // Metric ID: Ride Duration
                };

                HandleActivityRecord(3, metrics); // Cycling Activity ID = 3
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
