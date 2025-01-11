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
    public partial class frmHiking : UserControl
    {
        private ConnectionDB db;
        public frmHiking()
        {
            InitializeComponent();
        }

        private void btnHikingRecord_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (!double.TryParse(txtHikingElevation.Text, out double elevationGained) || elevationGained < 0)
                {
                    MessageBox.Show("Please enter a valid non-negative number for Elevation Gained.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtHikingDistance.Text, out double distance) || distance <= 0)
                {
                    MessageBox.Show("Please enter a valid positive number for Distance.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtHikingTimeTaken.Text, out double timeTaken) || timeTaken <= 0)
                {
                    MessageBox.Show("Please enter a valid positive number for Time Taken.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    { 10, elevationGained },  // Metric ID: Elevation Gained
                    { 11, distance },         // Metric ID: Distance
                    { 12, timeTaken }         // Metric ID: Time Taken
                };

                HandleActivityRecord(4, metrics, selectedIntensity); // Hiking Activity ID = 4
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
                double durationHours = metrics[12] / 60; // Time Taken in minutes

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
