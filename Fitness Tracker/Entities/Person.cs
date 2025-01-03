using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_Tracker.Entities
{
    public class Person
    {
        private int personID;
        private string username;
        private string password;
        private string firstname;
        private string lastname;
        private string email;
        private DateTime dateOfBirth;
        private string gender;
        private string mobile;
        private double weight;
        private double height;
        private string photoPath;
        public Person() { }
        public Person(int personID, string username, string password, string firstname, string lastname, string email, DateTime dateOfBirth, string gender, string mobile, double weight, double height, string photoPath)
        {
            this.personID = personID;
            this.username = username;
            this.password = password;
            this.firstname = firstname;
            this.lastname = lastname;
            this.email = email;
            this.dateOfBirth = dateOfBirth;
            this.gender = gender;
            this.mobile = mobile;
            this.weight = weight;
            this.height = height;
            this.photoPath = photoPath;
        }

        public int PersonID { get => personID; set => personID = value; }
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public string Firstname { get => firstname; set => firstname = value; }
        public string Lastname { get => lastname; set => lastname = value; }
        public string Email { get => email; set => email = value; }
        public DateTime DateOfBirth { get => dateOfBirth; set => dateOfBirth = value; }
        public string Gender { get => gender; set => gender = value; }
        public string Mobile { get => mobile; set => mobile = value; }
        public double Weight { get => weight; set => weight = value; }
        public double Height { get => height; set => height = value; }
        public string PhotoPath { get => photoPath; set => photoPath = value; }
    }
}
