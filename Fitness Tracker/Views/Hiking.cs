﻿using Fitness_Tracker.dao;
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

                Dictionary<int, double> metrics = new Dictionary<int, double>
                {
                    { 10, elevationGained },  // Metric ID: Elevation Gained
                    { 11, distance },         // Metric ID: Distance
                    { 12, timeTaken }         // Metric ID: Time Taken
                };

                HandleActivityRecord(4, metrics); // Hiking Activity ID = 4
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
