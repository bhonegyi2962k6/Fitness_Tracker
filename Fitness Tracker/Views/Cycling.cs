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
                // Validate inputs
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

                if (cboIntensity.SelectedItem == null)
                {
                    MessageBox.Show("Please select an intensity level.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedIntensity = cboIntensity.SelectedItem.ToString();

                // Prepare metrics
                var metrics = new Dictionary<int, double>
                {
                    { 7, speed },          // Metric ID: Speed
                    { 8, distance },       // Metric ID: Distance
                    { 9, rideDuration }    // Metric ID: Ride Duration
                };

                HandleActivityRecord(3, metrics, selectedIntensity); // Cycling Activity ID = 3
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

            return caloriesFromMet + caloriesFromFactors;
        }


        private void HandleActivityRecord(int activityId, Dictionary<int, double> metrics, string intensity)
        {
            try
            {
                db = new ConnectionDB();
                db.OpenConnection();

                // Step 1: Retrieve calculation factors and MET value
                var calculationFactors = db.GetCalculationFactors(activityId);
                double metValue = db.GetMetValue(activityId, intensity);

                // Validate MET value
                if (metValue <= 0)
                {
                    MessageBox.Show("Invalid MET value. Please check the selected intensity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Step 2: Retrieve user's weight and calculate duration
                double userWeight = frmLogin.person.Weight;
                double durationHours = metrics[9] / 60; // Time Taken in minutes

                // Step 3: Calculate calories burned
                double burnedCalories = CalculateBurnedCalories(metrics, calculationFactors, metValue, userWeight, durationHours);
                burnedCalories = Math.Round(burnedCalories, 2);


                // Step 4: Insert record into the database
                string selectedIntensity = cboIntensity.SelectedItem.ToString();
                int recordId = db.InsertRecords(burnedCalories, activityId, selectedIntensity);

                if (recordId <= 0)
                {
                    MessageBox.Show("Failed to insert the record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Step 5: Insert metrics
                if (db.InsertMetricValues(activityId, recordId, metrics))
                {
                    MessageBox.Show("Record successfully inserted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to insert metric values.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


    }
}
