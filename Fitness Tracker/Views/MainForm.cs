using Fitness_Tracker.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fitness_Tracker.Views
{
    public partial class frmMainForm : Form
    {
        bool isCollapsed;
        public frmMainForm()
        {
            InitializeComponent();
            InitializeMotivationalQuoteTimer();
        }

        private Timer motivationalQuoteTimer; // Timer for updating quotes
        private readonly string[] quotes = {
            "Quotes: The journey of a thousand miles begins with a single step.",
            "Quotes: Your health is an investment, not an expense.",
            "Quotes: Wake up with determination. Go to bed with satisfaction.",
            "Quotes: Push yourself because no one else will do it for you.",
            "Quotes: Your body can stand almost anything. It’s your mind that you have to convince.",
            "Quotes: Don’t limit your challenges, challenge your limits."
        };

        private void sideMenuTimer_Tick(object sender, EventArgs e)
        {
            if (isCollapsed)
            {
                panelSide.Width += 10; // Increment the width
                if (panelSide.Width >= panelSide.MaximumSize.Width) // Check against panelSide's maximum size
                {
                    sideMenuTimer.Stop(); // Stop the timer
                    isCollapsed = false; // Update the state
                }
            }
            else
            {
                panelSide.Width -= 10; // Decrement the width
                if (panelSide.Width <= panelSide.MinimumSize.Width) // Check against panelSide's minimum size
                {
                    sideMenuTimer.Stop(); // Stop the timer
                    isCollapsed = true; // Update the state
                }
            }
        }

        private void dropDownTimer_Tick(object sender, EventArgs e)
        {
            if (isCollapsed)
            {
                panelDropDown.Height += 10;
                if (panelDropDown.Size == panelDropDown.MaximumSize)
                {
                    dropDownTimer.Stop();
                    isCollapsed = false;
                }
            }
            else
            {
                panelDropDown.Height -= 10;
                if (panelDropDown.Size == panelDropDown.MinimumSize)
                {
                    dropDownTimer.Stop();
                    isCollapsed = true;
                }
            }
        }

        private void frmMainForm_Load(object sender, EventArgs e)
        {
            User currentUser = User.GetInstance();
            lblWelcomeUsername.Text = currentUser.Username;

            ClearUpperPanelForHome();
            panelMain.Controls.Clear();
            panelMain.Controls.Add(new frmHome());

            if (!string.IsNullOrEmpty(currentUser.PhotoPath) && File.Exists(currentUser.PhotoPath))
            {
                try
                {
                    picProfilePhoto.Image = Image.FromFile(currentUser.PhotoPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load profile photo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            DisplayMotivationalQuote();
        }
        private void InitializeMotivationalQuoteTimer()
        {
            motivationalQuoteTimer = new Timer
            {
                Interval = 10000 // 10 seconds
            };
            motivationalQuoteTimer.Tick += MotivationalQuoteTimer_Tick;
            motivationalQuoteTimer.Start();
        }

        private void MotivationalQuoteTimer_Tick(object sender, EventArgs e)
        {
            DisplayMotivationalQuote();
        }
        private void DisplayMotivationalQuote()
        {
            Random rnd = new Random();
            lblMotivationalQuote.Text = quotes[rnd.Next(quotes.Length)];
        }

        private void btnMenuBar_Click(object sender, EventArgs e)
        {
            sideMenuTimer.Start();
        }

        private void btnActivity_Click(object sender, EventArgs e)
        {
            dropDownTimer.Start();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            // Ask for confirmation before logging out
            DialogResult result = MessageBox.Show(
                "Are you sure you want to log out?",
                "Logout Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                // Clear any sensitive data
                ClearUserSession();
                // Navigate back to the login form
                this.Hide();
                using (frmLogin loginForm = new frmLogin())
                {
                    loginForm.ShowDialog();
                }
                this.Close();
            }
        }

        private void ClearUserSession()
        {
            // Clear the logged-in user object
            User.GetInstance().ClearUserData();
            User.ResetInstance();
        }
        private void btnSwimming_Click(object sender, EventArgs e)
        {
            RestoreUpperPanel();
            panelMain.Controls.Clear();
            panelMain.Controls.Add(new frmSwimming());
        }

        private void btnWalking_Click(object sender, EventArgs e)
        {
            RestoreUpperPanel();
            panelMain.Controls.Clear();
            panelMain.Controls.Add(new frmWalking());
        }

        private void btnCycling_Click(object sender, EventArgs e)
        {
            RestoreUpperPanel();
            panelMain.Controls.Clear();
            panelMain.Controls.Add(new frmCycling());
        }

        private void btnHiking_Click(object sender, EventArgs e)
        {
            RestoreUpperPanel();
            panelMain.Controls.Clear();
            panelMain.Controls.Add(new frmHiking());
        }

        private void btnWeightlifiting_Click(object sender, EventArgs e)
        {
            RestoreUpperPanel();
            panelMain.Controls.Clear();
            panelMain.Controls.Add(new frmWeightlifting());
        }

        private void btnRowing_Click(object sender, EventArgs e)
        {
            RestoreUpperPanel();
            panelMain.Controls.Clear();
            panelMain.Controls.Add(new frmRowing());
        }

        private void btnSchedule_Click(object sender, EventArgs e)
        {
            RestoreUpperPanel();
            panelMain.Controls.Clear();
            panelMain.Controls.Add(new frmSchedule());
        }

        private void btnRecords_Click(object sender, EventArgs e)
        {
            RestoreUpperPanel();
            panelMain.Controls.Clear();
            panelMain.Controls.Add(new frmMonitorActivity());
        }

        private void btnSetGoal_Click(object sender, EventArgs e)
        {
            RestoreUpperPanel();
            panelMain.Controls.Clear();
            panelMain.Controls.Add(new frmSetGoal());
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            DisplayMotivationalQuote();
            ClearUpperPanelForHome();
            panelMain.Controls.Clear();
            panelMain.Controls.Add(new frmHome());
        }
        private void ClearUpperPanelForHome()
        {
            foreach (Control control in panelUserSide.Controls)
            {
                // Keep only the control box and logout button visible
                if (control.Name == "panelUserSide")
                {
                    control.Visible = true;
                }
                else
                {
                    control.Visible = false; // Hide other controls
                }
            }
        }

        private void RestoreUpperPanel()
        {
            foreach (Control control in panelUserSide.Controls)
            {
                // Make all controls visible again
                control.Visible = true;
            }
        }


        private void btnSetting_Click(object sender, EventArgs e)
        {
            frmSetting settingsForm = new frmSetting();

            // Subscribe to the OnPhotoUpdated event
            settingsForm.OnPhotoUpdated += UpdateProfilePhotoInMainForm;
            RestoreUpperPanel();
            panelMain.Controls.Clear();
            panelMain.Controls.Add(settingsForm);
        }
        private void UpdateProfilePhotoInMainForm(string newPhotoPath)
        {
            if (!string.IsNullOrEmpty(newPhotoPath) && File.Exists(newPhotoPath))
            {
                try
                {
                    picProfilePhoto.Image = Image.FromFile(newPhotoPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to update profile photo in main form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
       

    }
}
