using Guna.UI2.WinForms;
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
    public partial class frmLoading : Form
    {
        private int progressValue = 0; // Tracks the progress for the progress bar
        public frmLoading()
        {
            InitializeComponent();
        }

        private void timerSplash_Tick(object sender, EventArgs e)
        {
            progressValue += 2; // Increment progress

            // Update the progress bar
            gunaProgressBar.Value = progressValue;

            // Rotate the GunaProgressIndicator automatically
            gunaProgressIndicator.Start();

            // Check if loading is complete
            if (progressValue >= gunaProgressBar.Maximum)
            {
                timerSplash.Stop();
                gunaProgressIndicator.Stop(); // Stop the indicator animation
                this.Close(); // Close the splash screen
            }
        }

        private void frmLoading_Load(object sender, EventArgs e)
        {
            // Start the timer when the form loads
            timerSplash.Interval = 70; // Timer tick every 50ms
            timerSplash.Start();
        }
    }
}
