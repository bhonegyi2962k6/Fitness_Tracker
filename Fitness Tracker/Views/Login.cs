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

        private int maxAttempts = 4;
        private int lockoutDuration = 30;
        private int remainingAttempts;
        private int lockoutTimeRemaining;
        private bool isLockedOut;
        public frmLogin()
        {
            InitializeComponent();
            txtPassword.UseSystemPasswordChar = true; 
            db = ConnectionDB.GetInstance();
            remainingAttempts = maxAttempts;
            lockoutTimeRemaining = lockoutDuration;
            lblLockOutMessage.Visible = false;
            loginAttemptTimer.Stop();
        }
        private void StartLockout()
        {
            isLockedOut = true;
            remainingAttempts = 1; // Allow only one retry after lockout
            btnLogIn.Enabled = false;

            lockoutTimeRemaining = lockoutDuration;
            lblLockOutMessage.Text = $"Please wait {lockoutTimeRemaining} seconds before trying again.";
            lblLockOutMessage.Visible = true;

            MessageBox.Show($"Maximum login attempts reached. Please try again in {lockoutDuration} seconds.",
                            "Login Disabled", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            loginAttemptTimer.Interval = 1000;
            loginAttemptTimer.Start();
        }

        private void SuccessfulLogin(Person user)
        {
            remainingAttempts = maxAttempts;
            isLockedOut = false;
            lblLockOutMessage.Visible = false;

            string fullName = $"{user.Firstname.Trim()} {user.Lastname.Trim()}";
            MessageBox.Show($"Welcome, {fullName}!", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                StartLockout();
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

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            lockoutTimeRemaining--;

            if (lockoutTimeRemaining > 0)
            {
                lblLockOutMessage.Text = $"Please wait {lockoutTimeRemaining} seconds before trying again.";
            }
            else
            {
                loginAttemptTimer.Stop();
                isLockedOut = false;
                btnLogIn.Enabled = true;
                lblLockOutMessage.Text = "You have 1 last attempt to log in again.";
                lblLockOutMessage.Visible = true;

                MessageBox.Show("The lockout period has ended. You can try logging in again.",
                                "Lockout Ended", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
