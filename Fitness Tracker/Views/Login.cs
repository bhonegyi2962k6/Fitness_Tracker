using Fitness_Tracker.dao;
using Fitness_Tracker.Entities;
using Google.Protobuf.Compiler;
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
    public partial class frmLogin : Form
    {
        private readonly ConnectionDB db;
        public static User user;

        private const int maxAttempts = 4;
        private const int lockoutDuration = 30;

        private static int remainingAttempts = maxAttempts;
        private static DateTime? lockoutEndTime = null;
        private static bool isLockedOut = false;
        private static bool lockoutMessageShown = false; // Prevent multiple message boxes

        public frmLogin()
        {
            InitializeComponent();
            txtPassword.UseSystemPasswordChar = true;
            db = ConnectionDB.GetInstance();

            if (isLockedOut && lockoutEndTime.HasValue)
            {
                int secondsLeft = (int)(lockoutEndTime.Value - DateTime.Now).TotalSeconds;
                if (secondsLeft > 0)
                {
                    StartLockout(secondsLeft);
                }
                else
                {
                    EndLockout();
                }
            }
            else
            {
                remainingAttempts = maxAttempts;
                isLockedOut = false;
                lblLockOutMessage.Visible = false;
                loginAttemptTimer.Stop();
            }
        }
        private void StartLockout(int secondsRemaining)
        {
            isLockedOut = true;
            lockoutEndTime = DateTime.Now.AddSeconds(secondsRemaining);
            btnLogIn.Enabled = false;

            lblLockOutMessage.Text = $"Please wait {secondsRemaining} seconds before trying again.";
            lblLockOutMessage.Visible = true;

            if (!lockoutMessageShown)
            {
                MessageBox.Show($"Maximum login attempts reached. Please try again in {secondsRemaining} seconds.",
                                "Login Disabled", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lockoutMessageShown = true;
            }

            loginAttemptTimer.Interval = 1000; // Timer updates every second
            loginAttemptTimer.Start();
        }
        private void EndLockout()
        {
            loginAttemptTimer.Stop();
            isLockedOut = false;
            btnLogIn.Enabled = true;
            lockoutEndTime = null;

            // Reset remaining attempts to 1
            remainingAttempts = 1;

            // Update the label to reflect the new attempt count
            lblLockOutMessage.Text = "The lockout period has ended. You have 1 attempt to log in.";
            lblLockOutMessage.Visible = true;

            // Show the message box
            MessageBox.Show("The lockout period has ended. You have 1 attempt to log in.",
                            "Lockout Ended", MessageBoxButtons.OK, MessageBoxIcon.Information);

            lockoutMessageShown = false; // Reset flag so it can show again next time
        }
        private void SuccessfulLogin(Person user)
        {
            remainingAttempts = maxAttempts;
            isLockedOut = false;
            lblLockOutMessage.Visible = false;

            string fullName = $"{user.Firstname.Trim()} {user.Lastname.Trim()}";
            MessageBox.Show($"Welcome, {fullName}!", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // Show the splash screen
            using (frmLoading splash = new frmLoading())
            {
                this.Hide();
                splash.ShowDialog(); // Block further execution until splash screen is closed
            }
            frmMainForm mainForm = new frmMainForm();
            this.Hide();
            mainForm.ShowDialog();
            this.Close();
        }
        private void FailedLogin()
        {
            remainingAttempts--;

            if (remainingAttempts > 0)
            {
                lblLockOutMessage.Text = $"Invalid Username or Password! Remaining attempts: {remainingAttempts}";
                lblLockOutMessage.Visible = true;
            }
            else
            {
                // If the user fails their 1 attempt, restart the lockout cycle
                StartLockout(lockoutDuration);
            }
        }
        private void linkLabelRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // show Register
            frmRegister showform = new frmRegister();
            this.Hide();
            showform.ShowDialog();
            this.Close();  // Hide current form
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            if (isLockedOut)
            {
                MessageBox.Show("Please wait until the lockout period ends.", "Login Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            // Check if both fields are empty
            if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            // Check if username is empty
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter your username.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            // Check if password is empty
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter your password.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }
            try
            {
                
                user = User.GetInstance();
                // Authenticate user
                var validUser = db.IsValidUser(username, password);
                if (validUser != null)
                {
                    user.SetUserData(validUser.PersonID, validUser.Username, validUser.Password, validUser.Firstname,
                                     validUser.Lastname, validUser.Email, validUser.DateOfBirth, validUser.Gender,
                                     validUser.Mobile, validUser.Weight, validUser.Height, validUser.PhotoPath);

                    SuccessfulLogin(user);
                }
                else
                {
                    FailedLogin();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while processing your login: {ex.Message}",
                                "Login Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void loginAttemptTimer_Tick(object sender, EventArgs e)
        {
            if (!lockoutEndTime.HasValue) return;

            int secondsLeft = (int)(lockoutEndTime.Value - DateTime.Now).TotalSeconds;

            if (secondsLeft > 0)
            {
                lblLockOutMessage.Text = $"Please wait {secondsLeft} seconds before trying again.";
            }
            else
            {
                EndLockout(); // Reset lockout and give 1 attempt
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void PasswordVisibility(Guna2TextBox textBox, bool useSystemPasswordChar)
        {
            textBox.UseSystemPasswordChar = useSystemPasswordChar;
            textBox.IconRight = useSystemPasswordChar ? Properties.Resources.invisible : Properties.Resources.visible;
        }

        private void txtPassword_IconRightClick(object sender, EventArgs e)
        {
            PasswordVisibility(txtPassword, !txtPassword.UseSystemPasswordChar);
        }
    }
}
