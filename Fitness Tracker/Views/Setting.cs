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
        public event Action<string> OnPhotoUpdated;
        private bool isChanged = false;

        public frmSetting()
        {
            InitializeComponent();
            db = ConnectionDB.GetInstance();
            txtCurrentPassword.UseSystemPasswordChar = true;
            txtNewPassword.UseSystemPasswordChar = true;
            btnChange.Enabled = false;

            txtFirstName.TextChanged += UserInfoChanged;
            txtLastName.TextChanged += UserInfoChanged;
            txtUsername.TextChanged += UserInfoChanged;
            txtEmail.TextChanged += UserInfoChanged;
            txtMobile.TextChanged += UserInfoChanged;
            txtWeight.TextChanged += UserInfoChanged;
            txtHeight.TextChanged += UserInfoChanged;
            cboGender.SelectedIndexChanged += UserInfoChanged;
            dtpDateOfBirth.ValueChanged += UserInfoChanged;
        }
        private void UserInfoChanged(object sender, EventArgs e)
        {
            var user = User.GetInstance();
            
            // Check if any field has been modified
            bool hasChanges =
                txtFirstName.Text.Trim() != user.Firstname ||
                txtLastName.Text.Trim() != user.Lastname ||
                txtUsername.Text.Trim() != user.Username ||
                txtEmail.Text.Trim() != user.Email ||
                txtMobile.Text.Trim() != user.Mobile ||
                txtWeight.Text.Trim() != user.Weight.ToString() ||
                txtHeight.Text.Trim() != user.Height.ToString() ||
                cboGender.SelectedItem?.ToString() != user.Gender ||
                dtpDateOfBirth.Value.Date != user.DateOfBirth.Date;

            isChanged = hasChanges;  // 🔹 Update the flag correctly
            btnChange.Enabled = hasChanges; // Enable button only if there's a change
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
            if (!isChanged)
            {
                MessageBox.Show("No changes detected.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var user = User.GetInstance();

                // Validate username length (6 to 15 alphanumeric characters)
                if (!IsValidUsername(txtUsername.Text.Trim()))
                {
                    MessageBox.Show("Username must be 6-15 characters long and contain only letters and numbers.", "Invalid Username", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUsername.Focus();
                    return;
                }

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

                // Exclude current user when checking username & email existence
                if (db.IsUserExists(txtUsername.Text.Trim(), txtEmail.Text.Trim(), user.PersonID))
                {
                    MessageBox.Show("Username or email is already taken. Please use a different one.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                if (dtpDateOfBirth.Value > DateTime.Now.AddYears(-13))
                {
                    MessageBox.Show("You must be at least 13 years old to use this application.", "Age Restriction", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                user.SetUserData(
                    user.PersonID,
                    txtUsername.Text.Trim(),
                    user.Password,
                    txtFirstName.Text.Trim(),
                    txtLastName.Text.Trim(),
                    txtEmail.Text.Trim(),
                    dtpDateOfBirth.Value,
                    cboGender.SelectedItem.ToString(),
                    txtMobile.Text.Trim(),
                    weight,
                    height,
                    user.PhotoPath
                );

                if (db.UpdateUserInfo(user))
                {
                    MessageBox.Show("User information updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    isChanged = false;
                    btnChange.Enabled = false;
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
        private bool IsValidUsername(string username)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(username, "^[a-zA-Z0-9]{6,15}$");
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
        private string PromptForPassword()
        {
            Form passwordPrompt = new Form()
            {
                Width = 420,
                Height = 270, // Adjusted to fit toggle
                Text = "Confirm Your Password",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false,
                BackColor = Color.White
            };

            Label lblPrompt = new Label()
            {
                Left = 20,
                Top = 20,
                Width = 380,
                Height = 40,
                Text = "Please enter your password to proceed:",
                Font = new Font("Century Gothic", 10, FontStyle.Regular),
                ForeColor = Color.Black,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Guna2TextBox txtPassword = new Guna2TextBox()
            {
                Left = 20,
                Top = 70,
                Width = 280,
                Height = 40,
                PlaceholderText = "Enter your password",
                Font = new Font("Century Gothic", 10, FontStyle.Regular),
                BorderRadius = 8,
                BorderColor = Color.Gray,
                ForeColor = Color.Black,
                UseSystemPasswordChar = true,  // Initially hidden
                PasswordChar = '●'  // Bullet char for hidden text
            };

            // Toggle Switch for Show Password
            Guna2ToggleSwitch toggleShowPassword = new Guna2ToggleSwitch()
            {
                Left = 22,
                Top = txtPassword.Bottom + 10, // Place below password textbox
                Width = 40,
                Height = 20,
                CheckedState = { FillColor = Color.FromArgb(33, 53, 85) },
                UncheckedState = { FillColor = Color.Gray }
            };

            Label lblShowPassword = new Label()
            {
                Left = toggleShowPassword.Right + 5, // Place beside toggle
                Top = toggleShowPassword.Top,
                Text = "Show Password",
                Font = new Font("Century Gothic", 10, FontStyle.Regular),
                ForeColor = Color.Black,
                AutoSize = true
            };

            // 🔹 Ensure password is hidden initially inside Form Load
            passwordPrompt.Load += (sender, e) =>
            {
                txtPassword.UseSystemPasswordChar = true; // Ensure it starts hidden
            };

            // 🔹 Toggle Password Visibility (Fixed)
            toggleShowPassword.CheckedChanged += (sender, e) =>
            {
                txtPassword.UseSystemPasswordChar = !toggleShowPassword.Checked;
                txtPassword.PasswordChar = toggleShowPassword.Checked ? '\0' : '●'; // Show/hide text properly
            };

            Guna2Button btnOK = new Guna2Button()
            {
                Text = "Confirm",
                Left = (passwordPrompt.Width - 140) / 2,
                Width = 120,
                Top = lblShowPassword.Bottom + 20,
                Height = 40,
                DialogResult = DialogResult.OK,
                FillColor = Color.FromArgb(33, 53, 85),
                ForeColor = Color.White,
                Font = new Font("Century Gothic", 10, FontStyle.Bold),
                BorderRadius = 10
            };

            btnOK.Click += (sender, e) =>
            {
                passwordPrompt.DialogResult = DialogResult.OK;
                passwordPrompt.Close();
            };

            // Add controls to the form
            passwordPrompt.Controls.Add(lblPrompt);
            passwordPrompt.Controls.Add(txtPassword);
            passwordPrompt.Controls.Add(toggleShowPassword);
            passwordPrompt.Controls.Add(lblShowPassword);
            passwordPrompt.Controls.Add(btnOK);
            passwordPrompt.AcceptButton = btnOK;

            return passwordPrompt.ShowDialog() == DialogResult.OK ? txtPassword.Text.Trim() : null;
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            // Ask user for their current password
            string enteredPassword = PromptForPassword();

            if (string.IsNullOrWhiteSpace(enteredPassword))
            {
                MessageBox.Show("Password is required to update your information.", "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verify the password before updating info
            if (!PasswordHelper.VerifyPassword(enteredPassword, User.GetInstance().Password))
            {
                MessageBox.Show("Incorrect password. Please try again.", "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // If the password is correct, proceed with updating the information
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
