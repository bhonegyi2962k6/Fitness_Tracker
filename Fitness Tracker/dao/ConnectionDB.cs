using Fitness_Tracker.Entities;
using Fitness_Tracker.Utilities;
using Fitness_Tracker.Views;
using Google.Protobuf.Compiler;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fitness_Tracker.dao
{
    public class ConnectionDB
    {
        // declare
        private MySqlConnection conn;
        private MySqlCommand cmd;
        private MySqlDataReader reader;

        private string server;
        private string username;
        private string password;
        private string database;

        private string sql;

        // constructor(initialize)
        public ConnectionDB()
        {
            server = "localhost";
            username = "root";
            password = "monk2962006";
            database = "assignment";
            string url = "Server =" + server
                        + ";Uid =" + username
                        + ";Pwd =" + password
                        + ";database =" + database;
            try
            {
                conn = new MySqlConnection(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Don't Get Connection!!" + ex.Message);
            }
        }

        //open
        public void OpenConnection()
        {
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Open Connection Failed! " + ex.Message);

            }
        }
        //close
        public void CloseConnection()
        {
            try
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Close connection failed!" + ex.Message);

            }
        }

        public bool IsUserExists(string username, string email)
        {
            // Check if a user with the given username or email already exists
            sql = "SELECT COUNT(*) FROM person WHERE username = '" + username + "' OR email = '" + email + "'";
            cmd = new MySqlCommand(sql, conn);
            int userExists = Convert.ToInt32(cmd.ExecuteScalar());
            return userExists > 0;
        }


        public Person IsValidUser(string username, string password)
        {
            Person person = null;

            sql = "SELECT * FROM person WHERE username = @username";

            cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", username);

            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                // Safely retrieve data and handle null cases
                string dbPassword = reader.IsDBNull(2) ? null : reader.GetString(2);

                if (dbPassword == null)
                {
                    MessageBox.Show("Error: User found but no password is set in the database.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                // Create the Person object
                person = new Person
                {
                    PersonID = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Password = dbPassword, // Hashed password from the database
                    Firstname = reader.GetString(3),
                    Lastname = reader.GetString(4),
                    Email = reader.GetString(5),
                    DateOfBirth = reader.GetDateTime(6),
                    Gender = reader.GetString(7),
                    Mobile = reader.GetString(8),
                    Weight = reader.GetDouble(9),
                    Height = reader.GetDouble(10),
                    PhotoPath = reader.GetString(11)
                };

                // Verify the password
                if (!PasswordHelper.VerifyPassword(password, dbPassword))
                {
                    person = null; // Password mismatch
                }
            }
            reader.Close();
            return person;
        }

        public bool AddUser(Person person)
        {
            bool flag = false;

            sql = @"INSERT INTO person (username, password, first_name, last_name, email, date_of_birth, gender, mobile, weight, height, photo_path) 
            VALUES (@username, @password, @first_name, @last_name, @email, @date_of_birth, @gender, @mobile, @weight, @height, @photo_path)";

            cmd = new MySqlCommand(sql, conn);

            // Use parameters to prevent SQL injection
            cmd.Parameters.AddWithValue("@username", person.Username);
            cmd.Parameters.AddWithValue("@password", person.Password); // Hashed password
            cmd.Parameters.AddWithValue("@first_name", person.Firstname);
            cmd.Parameters.AddWithValue("@last_name", person.Lastname);
            cmd.Parameters.AddWithValue("@email", person.Email);
            cmd.Parameters.AddWithValue("@date_of_birth", person.DateOfBirth);
            cmd.Parameters.AddWithValue("@gender", person.Gender);
            cmd.Parameters.AddWithValue("@mobile", person.Mobile);
            cmd.Parameters.AddWithValue("@weight", person.Weight);
            cmd.Parameters.AddWithValue("@height", person.Height);
            cmd.Parameters.AddWithValue("@photo_path", person.PhotoPath);

            int result = cmd.ExecuteNonQuery();

            flag = result > 0;
            return flag;
        }

        public Dictionary<int, double> GetCalculationFactors(int activityId)
        {
            Dictionary<int, double> calculationFactors = new Dictionary<int, double>();

            try
            {
                sql = @"SELECT metric_id, calculation_factor FROM metric WHERE activity_id = @activityId";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@activityId", activityId);

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int metricId = reader.GetInt32("metric_id");
                    double calculationFactor = reader.GetDouble("calculation_factor");

                    calculationFactors[metricId] = calculationFactor;
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving calculation factors: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return calculationFactors;
        }

        public bool InsertRecords(double burnedCalories, int activityId)
        {
            bool flag = false;

            try
            {
                string sql = @"
            INSERT INTO record (record_date, burned_calories, activity_id, person_id)
            VALUES (@recordDate, @burnedCalories, @activityId, @personID)";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@recordDate", DateTime.Now.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@burnedCalories", burnedCalories);
                cmd.Parameters.AddWithValue("@activityId", activityId);
                cmd.Parameters.AddWithValue("@personID", frmLogin.person.PersonID);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return flag;
        }

        public bool InsertMetricValues(int activityId, Dictionary<int, double> metrics)
        {
            bool flag = true;

            try
            {
                foreach (var metric in metrics)
                {
                    string sql = @"
                INSERT INTO metric_values (activity_id, metric_id, value, unit, recorded_date)
                VALUES (@activityId, @metricId, @value, @unit, @recordedDate)";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@activityId", activityId);
                    cmd.Parameters.AddWithValue("@metricId", metric.Key);
                    cmd.Parameters.AddWithValue("@value", metric.Value);
                    cmd.Parameters.AddWithValue("@unit", GetUnitForMetric(metric.Key)); // Assume a helper method for units
                    cmd.Parameters.AddWithValue("@recordedDate", DateTime.Now.ToString("yyyy-MM-dd"));

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected <= 0)
                    {
                        flag = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting metric values: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                flag = false;
            }

            return flag;
        }

        public string GetUnitForMetric(int key)
        {
            string unit = "";

            try
            {
                string sql = @"  SELECT unit FROM metric_values WHERE metric_id = @metricId";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@metricId", key); // Use the method parameter

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        unit = reader.GetString("unit");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving unit for metric: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return unit;
        }

        public int InsertSchedule(int personID, DateTime scheduledDate)
        {
            try
            {
                sql = @"INSERT INTO schedule (person_id, scheduled_date)
                       VALUES (@personId, @scheduledDate);
                       SELECT LAST_INSERT_ID();";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personID", personID);
                cmd.Parameters.AddWithValue("@scheduledDate", scheduledDate);

                int scheduleId = Convert.ToInt32(cmd.ExecuteScalar());
                return scheduleId;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting schedule: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public bool InsertScheduleActivity(int scheduleId, int activityId, TimeSpan startTime, int durationMinutes)
        {
            try
            {
                string sql = @"INSERT INTO schedule_activity (schedule_id, activity_id, start_time, duration_minutes)
                       VALUES (@scheduleId, @activityId, @startTime, @durationMinutes)";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@scheduleId", scheduleId);
                cmd.Parameters.AddWithValue("@activityId", activityId);
                cmd.Parameters.AddWithValue("@startTime", startTime);
                cmd.Parameters.AddWithValue("@durationMinutes", durationMinutes);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting schedule activity: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public DataTable GetSchedules(int personID)
        {
            try
            {
                sql = @"
            SELECT 
                a.activity_name AS 'Activity', 
                s.scheduled_date AS 'Date', 
                sa.start_time AS 'Start Time', 
                sa.duration_minutes AS 'Duration'
            FROM schedule s
            JOIN schedule_activity sa ON s.schedule_id = sa.schedule_id
            JOIN activity a ON sa.activity_id = a.activity_id
            WHERE s.person_id = @personId";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personId", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving schedules: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public Dictionary<int, string> GetActivities()
        {
            Dictionary<int, string> activities = new Dictionary<int, string>();

            try
            {
                OpenConnection();

                string sql = "SELECT activity_id, activity_name FROM activity";
                cmd = new MySqlCommand(sql, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        activities.Add(reader.GetInt32("activity_id"), reader.GetString("activity_name"));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving activities: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection();
            }

            return activities;
        }
    }
    
}
