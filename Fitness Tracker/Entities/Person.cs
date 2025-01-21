using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_Tracker.Entities
{
    /// <summary>
    /// Abstract base class representing a person.
    /// </summary>
    public abstract class Person
    {
        // Common properties
        public int PersonID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Mobile { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public string PhotoPath { get; set; }

        // Default constructor
        protected Person() { }

        // Parameterized constructor
        protected Person(int personID, string username, string password, string firstname, string lastname,
                         string email, DateTime dateOfBirth, string gender, string mobile, double weight,
                         double height, string photoPath)
        {
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
    }
}
