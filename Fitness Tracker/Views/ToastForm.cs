using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fitness_Tracker.Views
{
    public partial class frmToastForm : Form
    {
        private Timer closeTimer;

        public frmToastForm(string title, string message, Color? badgeColor = null)
        {
            InitializeComponent();

            // Set title and message
            lblTitle.Text = title;
            lblMessage.Text = message;

            // Set panelBadgeColor's background color if provided
            if (badgeColor != null)
            {
                panelBadgeColor.BackColor = badgeColor.Value;
            }

            // Style the form
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.ShowInTaskbar = false;

            // Position the toast in the bottom-right corner of the screen
            var screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Width - this.Width - 10, screen.Height - this.Height - 10);

            // Initialize and start the timer
            closeTimer = new Timer();
            closeTimer.Interval = 5000; // 5 seconds
            closeTimer.Tick += CloseTimer_Tick;
            closeTimer.Start();
        }


        // Timer tick event to close the toast
        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            closeTimer.Stop();
            this.Close();
        }

        // Call this method when you need to dispose of the timer
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            closeTimer?.Dispose();
            base.OnFormClosed(e);
        }

    }

}
