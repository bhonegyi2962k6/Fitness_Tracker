using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_Tracker.Entities
{
    public class User : Person
    {
        private static User instance;
        private static readonly object lockObj = new object();

        // Private constructor to prevent instantiation
        private User() { }

        // Static method to get the single instance
        public static User GetInstance()
        {
            if (instance == null)
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = new User();
                    }
                }
            }
            return instance;
        }

        // Method to reset the singleton instance (logout scenario)
        public static void ResetInstance()
        {
            lock (lockObj)
            {
                instance = null;
            }
        }

        // Method to set user data
        public void SetUserData(int personID, string username, string password, string firstname, string lastname,
                                string email, DateTime dateOfBirth, string gender, string mobile, double weight,
                                double height, string photoPath)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username cannot be empty.");
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email cannot be empty.");
            if (weight <= 0) throw new ArgumentException("Weight must be a positive number.");
            if (height <= 0) throw new ArgumentException("Height must be a positive number.");

            PersonID = personID;
            Username = username;
            Password = password;
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
            DateOfBirth = dateOfBirth;
            Gender = gender; 
            Mobile = mobile;
            Weight = weight;
            Height = height;
            PhotoPath = photoPath;
        }

        // Method to clear user data
        public void ClearUserData()
        {
            PersonID = 0;
            Username = null;
            Password = null;
            Firstname = null;
            Lastname = null;
            Email = null;
            DateOfBirth = default;
            Gender = null;
            Mobile = null;
            Weight = 0;
            Height = 0;
            PhotoPath = null;
        }

    }
}
