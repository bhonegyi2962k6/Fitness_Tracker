using Fitness_Tracker.dao;
using Fitness_Tracker.Entities;
using Fitness_Tracker.Utilities;
using Guna.UI2.WinForms;
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
    public partial class frmSetting : UserControl
    {
        private readonly ConnectionDB db;
        // Define an event for photo updates
        public event Action<string> OnPhotoUpdated;
        public frmSetting()
        {
            InitializeComponent();
            db = ConnectionDB.GetInstance(); // Use the Singleton instance
            txtCurrentPassword.UseSystemPasswordChar = true;
            txtNewPassword.UseSystemPasswordChar = true;
        }

        private void frmSetting_Load(object sender, EventArgs e)
        {
            try
            {
                // Get the current user instance
                var user = User.GetInstance();

                // Fill the data into respective controls
                txtUsername.Text = user.Username;
                txtFirstName.Text = user.Firstname;
                txtLastName.Text = user.Lastname;
                txtEmail.Text = user.Email;
                txtMobile.Text = user.Mobile;
                txtWeight.Text = user.Weight.ToString();
                txtHeight.Text = user.Height.ToString();
                dtpDateOfBirth.Value = user.DateOfBirth;

                // Set the gender combobox value
                cboGender.SelectedItem = user.Gender;

                // Load the profile photo
                if (!string.IsNullOrEmpty(user.PhotoPath) && File.Exists(user.PhotoPath))
                {
                    picProfilePhoto.Image = Image.FromFile(user.PhotoPath);
                }
                else
                {
                    // Load a default image if no photo is found
                    string defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Images\default-profile.png");
                    if (File.Exists(defaultImagePath))
                    {
                        picProfilePhoto.Image = Image.FromFile(defaultImagePath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void UpdateUserInfo()
        {
            try
            {
                // Get current user instance
                var user = User.GetInstance();

                // Validate fields
                if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtMobile.Text))
                {
                    MessageBox.Show("All fields are required. Please complete the form.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!IsValidEmail(txtEmail.Text))
                {
                    MessageBox.Show("Invalid email format. Please enter a valid email.", "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!IsValidMobileNumber(txtMobile.Text))
                {
                    MessageBox.Show("Invalid mobile number. Please enter a valid number.", "Mobile Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtWeight.Text, out double weight) || weight <= 0)
                {
                    MessageBox.Show("Invalid weight. Please enter a valid number.", "Weight Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtHeight.Text, out double height) || height <= 0)
                {
                    MessageBox.Show("Invalid height. Please enter a valid number.", "Height Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate date of birth (user must be at least 13 years old)
                if (dtpDateOfBirth.Value > DateTime.Now.AddYears(-13))
                {
                    MessageBox.Show("You must be at least 13 years old to use this application.", "Age Restriction", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check if email already exists for another user
                if (!user.Email.Equals(txtEmail.Text.Trim(), StringComparison.OrdinalIgnoreCase) &&
                    db.IsUserExists(user.Username, txtEmail.Text.Trim()))
                {
                    MessageBox.Show("The email is already in use by another account. Please choose a different email.", "Duplicate Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Update user data
                user.SetUserData(
                    user.PersonID, // ID remains the same
                    user.Username, // Username cannot be changed
                    user.Password, // Password remains unchanged
                    txtFirstName.Text.Trim(),
                    txtLastName.Text.Trim(),
                    txtEmail.Text.Trim(),
                    dtpDateOfBirth.Value,
                    cboGender.SelectedItem.ToString(),
                    txtMobile.Text.Trim(),
                    weight,
                    height,
                    user.PhotoPath // Use the existing photo path
                );

                // Update database
                if (db.UpdateUserInfo(user))
                {
                    MessageBox.Show("User information updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to update user information. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating user information: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void UpdatePassword()
        {
            try
            {
                // Validate old password
                string currentPassword = txtCurrentPassword.Text.Trim();
                if (!PasswordHelper.VerifyPassword(currentPassword, User.GetInstance().Password))
                {
                    MessageBox.Show("Current password is incorrect. Please try again.", "Password Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validate new password
                string newPassword = txtNewPassword.Text.Trim();
                if (string.IsNullOrWhiteSpace(newPassword) || !IsValidPassword(newPassword))
                {
                    MessageBox.Show("Invalid new password. Password must be at least 12 characters long and contain uppercase and lowercase letters.", "Password Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Hash and update password
                string hashedPassword = PasswordHelper.HashPassword(newPassword);

                if (db.UpdatePassword(User.GetInstance().PersonID, hashedPassword))
                {
                    MessageBox.Show("Password updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Update the current user instance
                    User.GetInstance().Password = hashedPassword;

                    // Clear password fields
                    txtCurrentPassword.Clear();
                    txtNewPassword.Clear();
                }
                else
                {
                    MessageBox.Show("Failed to update the password. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating the password: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void UpdateProfilePhoto(string photoPath)
        {
            try
            {
                var user = User.GetInstance();
                user.PhotoPath = photoPath;

                if (db.UpdatePhotoPath(user.PersonID, photoPath))
                {
                    MessageBox.Show("Profile photo updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Trigger the OnPhotoUpdated event
                    OnPhotoUpdated?.Invoke(photoPath);
                }
                else
                {
                    MessageBox.Show("Failed to update profile photo. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating profile photo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        private void PasswordVisibility(Guna2TextBox textBox, bool useSystemPasswordChar)
        {
            textBox.UseSystemPasswordChar = useSystemPasswordChar;
            textBox.IconRight = useSystemPasswordChar ? Properties.Resources.invisible : Properties.Resources.visible;
        }

        private bool IsValidMobileNumber(string mobile)
        {
            return mobile.Length >= 8 && mobile.Length <= 12 && mobile.All(char.IsDigit);
        }

        private bool IsValidPassword(string password)
        {
            return password.Length >= 12 && password.Any(char.IsUpper) && password.Any(char.IsLower);
        }

        private void btnUploadPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                try
                {
                    openFileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *bmp; *.png)|*.jpg;*.jpeg;*.gif;*.bmp;*.png";
                    openFileDialog.Title = "Select a Profile Photo";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (!File.Exists(openFileDialog.FileName))
                        {
                            MessageBox.Show("The selected file does not exist. Please choose a valid file.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        string rootDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
                        string imagesFolderPath = Path.Combine(rootDirectory, "Images");

                        if (!Directory.Exists(imagesFolderPath))
                        {
                            Directory.CreateDirectory(imagesFolderPath);
                        }

                        string fileName = Path.GetFileName(openFileDialog.FileName);
                        string destinationPath = Path.Combine(imagesFolderPath, fileName);

                        if (!File.Exists(destinationPath))
                        {
                            File.Copy(openFileDialog.FileName, destinationPath, true);
                        }

                        picProfilePhoto.Image = Image.FromFile(destinationPath);

                        UpdateProfilePhoto(destinationPath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            UpdateUserInfo();
        }

        private void btnUpdatePassword_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text) || string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                MessageBox.Show("Please fill both the current and new password fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UpdatePassword();
        }

        private void txtCurrentPassword_IconRightClick(object sender, EventArgs e)
        {
            PasswordVisibility(txtCurrentPassword, !txtCurrentPassword.UseSystemPasswordChar);
        }

        private void txtNewPassword_IconRightClick(object sender, EventArgs e)
        {
            PasswordVisibility(txtNewPassword, !txtNewPassword.UseSystemPasswordChar);
        }
    }
}
