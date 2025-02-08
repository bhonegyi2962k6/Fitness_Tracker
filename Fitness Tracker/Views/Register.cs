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
    public partial class frmRegister : Form
    {
        private ConnectionDB db;
        private string photoFilePath = null; // To store the selected photo file path
        public frmRegister()
        {
            InitializeComponent();
            db = ConnectionDB.GetInstance();
            txtRegisterPassword.UseSystemPasswordChar = true;
            txtConfirmPassword.UseSystemPasswordChar = true;
            dtpBirthDate.MaxDate = DateTime.Today;
            dtpBirthDate.Value = DateTime.Today;
            cboRegisterGender.SelectedIndex = 0;
        }

        private void linkLabelLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // show Login
            frmLogin showform = new frmLogin();
            this.Hide();
            showform.ShowDialog();
            this.Close();  // Hide current form
        }

        private bool IsValidUsername(string username)
        {
            // Validate that the username contains only letters and numbers and is between 6 and 15 characters long
            return System.Text.RegularExpressions.Regex.IsMatch(username, "^[a-zA-Z0-9]{6,15}$");
        }

        private bool IsValidPassword(string password)
        {
            // Validate that password is 12 characters long and contains at least one uppercase and one lowercase letter
            return System.Text.RegularExpressions.Regex.IsMatch(password, @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).{12,}$");
        }

        public bool isEmptyString()
        {
            return string.IsNullOrEmpty(txtRegisterUsername.Text) || string.IsNullOrEmpty(txtRegisterPassword.Text) || string.IsNullOrEmpty(txtRegisterFirstName.Text) ||
                string.IsNullOrEmpty(txtRegisterLastName.Text) || string.IsNullOrEmpty(txtRegisterEmail.Text) || string.IsNullOrEmpty(txtRegisterMobile.Text) ||
                string.IsNullOrEmpty(cboRegisterGender.Text) || string.IsNullOrEmpty(txtConfirmPassword.Text) || string.IsNullOrEmpty(txtRegisterWeight.Text) ||
                string.IsNullOrEmpty(txtRegisterHeight.Text);
        }
        private bool IsValidEmail(string email)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
        private bool IsValidMobileNumber(string mobile)
        {
            // Validate mobile number format: only digits and length between 6 and 12
            return System.Text.RegularExpressions.Regex.IsMatch(mobile, @"^\d{6,12}$");
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            int age = DateTime.Now.Year - dateOfBirth.Year;
            if (dateOfBirth > DateTime.Now.AddYears(-age)) age--;
            return age;
        }

        private bool IsAgeRestricted(DateTime dateOfBirth, int minimumAge)
        {
            int age = CalculateAge(dateOfBirth);
            return age < minimumAge;
        }

        private bool TryValidateDouble(string input, double min, double max, out double result, string fieldName)
        {
            // Attempt to parse the input as a double
            if (!double.TryParse(input, out result))
            {
                MessageBox.Show($"Please enter a valid numeric value for {fieldName}.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Check if the value is within the specified range
            if (result < min || result > max)
            {
                MessageBox.Show($"{fieldName} is out of range. Please enter a value between {min} and {max}.", "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Valid value
            return true;
        }

        private void PasswordVisibility(Guna2TextBox textBox, bool useSystemPasswordChar)
        {
            textBox.UseSystemPasswordChar = useSystemPasswordChar;
            textBox.IconRight = useSystemPasswordChar ? Properties.Resources.invisible : Properties.Resources.visible;
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
                        // Check if the file exists
                        if (!File.Exists(openFileDialog.FileName))
                        {
                            MessageBox.Show("The selected file does not exist. Please choose a valid file.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Display the selected image in the PictureBox
                        picProfilePhoto.Image = Image.FromFile(openFileDialog.FileName);

                        // Get the root directory (navigate to parent of `bin`)
                        string rootDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));

                        // Set the path to the Images folder in the root directory
                        string imagesFolderPath = Path.Combine(rootDirectory, "Images");

                        // Ensure the Images folder exists
                        if (!Directory.Exists(imagesFolderPath))
                        {
                            Directory.CreateDirectory(imagesFolderPath);
                        }

                        // Get the file name and construct the destination path
                        string fileName = Path.GetFileName(openFileDialog.FileName);
                        string destinationPath = Path.Combine(imagesFolderPath, fileName);

                        // Automatically use the existing file if it already exists
                        if (File.Exists(destinationPath))
                        {
                            // Set the photo file path and exit the upload process
                            photoFilePath = destinationPath;
                            return;
                        }

                        // Copy the file to the Images folder
                        File.Copy(openFileDialog.FileName, destinationPath, true);

                        // Store the file path in a private field
                        photoFilePath = destinationPath;

                    }
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("You do not have permission to save the file to the destination folder. Please try again with proper permissions.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (IOException ioEx)
                {
                    MessageBox.Show($"An error occurred while accessing the file: {ioEx.Message}", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtRegisterPassword_IconRightClick(object sender, EventArgs e)
        {
            PasswordVisibility(txtRegisterPassword, !txtRegisterPassword.UseSystemPasswordChar);
        }

        private void txtConfirmPassword_IconRightClick(object sender, EventArgs e)
        {
            PasswordVisibility(txtConfirmPassword, !txtConfirmPassword.UseSystemPasswordChar);
        }

        private void btnRegister_Click_1(object sender, EventArgs e)
        {
            // Collect data from form inputs
            string username = txtRegisterUsername.Text.Trim();
            string password = txtRegisterPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();
            string firstname = txtRegisterFirstName.Text.Trim();
            string lastname = txtRegisterLastName.Text.Trim();
            string email = txtRegisterEmail.Text.Trim();
            DateTime dateOfBirth = dtpBirthDate.Value;
            string gender = cboRegisterGender.Text.Trim();
            string mobile = txtRegisterMobile.Text.Trim();
            double weight;
            double height;


            // Check for empty fields
            if (isEmptyString())
            {
                MessageBox.Show("All fields are required. Please complete the form.", "Need to fill the field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (IsAgeRestricted(dateOfBirth, 13))
            {
                MessageBox.Show("You must be at least 13 years old to register.", "Age Restriction", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if username and password are the same
            if (username.Equals(password, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Username and password cannot be the same.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Check if password and confirm password match
            if (!password.Equals(confirmPassword))
            {
                MessageBox.Show("Password and Confirm Password do not match. Please re-enter the passwords.", "Password Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Basic email format validation
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Invalid email format. Please enter a valid email.", "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate mobile number format (basic length check)
            if (!IsValidMobileNumber(mobile))
            {
                MessageBox.Show("Invalid mobile number. Please enter a valid number with a length of 8 to 12 digits.", "Mobile Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidUsername(username))
            {
                MessageBox.Show("Invalid username! Username should contain only letters, numbers and 6 to 15 characters long.", "Invalid Username", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (!IsValidPassword(password))
            {
                MessageBox.Show("Invalid password! Password must be 12 characters long and contain at least one uppercase, one lowercase and one special letter.", "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate weight
            if (!TryValidateDouble(txtRegisterWeight.Text, 20, 500, out weight, "Weight"))
            {
                return; // Stop registration if invalid
            }

            // Validate height
            if (!TryValidateDouble(txtRegisterHeight.Text, 50, 300, out height, "Height"))
            {
                return; // Stop registration if invalid
            }

            // Check if photo is selected or skip if allowed
            if (string.IsNullOrEmpty(photoFilePath))
            {
                DialogResult result = MessageBox.Show(
                    "You have not uploaded a profile photo. Would you like to skip this step and upload it later?",
                    "Upload Photo for Later?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.No)
                {
                    MessageBox.Show("Please upload a profile photo before proceeding.", "Photo Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    photoFilePath = ""; // Set an empty path or leave null to indicate no photo
                }
            }

            // Hash the password before storing it
            string hashedPassword = PasswordHelper.HashPassword(password);
            // Prepare person object
            User user = User.GetInstance();
            user.SetUserData(0, username, hashedPassword, firstname, lastname, email, dateOfBirth, gender, mobile,
                             weight, height, photoFilePath);

            try
            {

                if (db.IsUserExists(username, email))
                {
                    MessageBox.Show("An account with this username or email already exists.", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (db.AddUser(user))
                {
                    MessageBox.Show("Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Hide();
                    new frmLogin().ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to register the user. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during registration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnExit_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
