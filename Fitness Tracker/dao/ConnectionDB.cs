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
        public int InsertRecords(double burnedCalories, int activityId, string intensityLevel)
        {
            int recordId = -1; // Default to -1 to indicate failure

            try
            {
                // Step 1: Insert into the `record` table with intensity_level
                string sql = @"
                        INSERT INTO record (record_date, burned_calories, activity_id, person_id, intensity_level)
                        VALUES (@recordDate, @burnedCalories, @activityId, @personID, @intensityLevel);
                        SELECT LAST_INSERT_ID();";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@recordDate", DateTime.Now.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@burnedCalories", burnedCalories);
                cmd.Parameters.AddWithValue("@activityId", activityId);
                cmd.Parameters.AddWithValue("@personID", frmLogin.person.PersonID);
                cmd.Parameters.AddWithValue("@intensityLevel", intensityLevel);

                // Execute the query and get the last inserted record ID
                recordId = Convert.ToInt32(cmd.ExecuteScalar());

                if (recordId > 0)
                {
                    // Step 2: Insert into the `user_record` table
                    string userRecordSql = @"INSERT INTO user_record (person_id, record_id) VALUES (@personID, @recordID);";

                    MySqlCommand userRecordCmd = new MySqlCommand(userRecordSql, conn);
                    userRecordCmd.Parameters.AddWithValue("@personID", frmLogin.person.PersonID);
                    userRecordCmd.Parameters.AddWithValue("@recordID", recordId);

                    int userRecordRowsAffected = userRecordCmd.ExecuteNonQuery();

                    // If `user_record` insertion fails, reset recordId to indicate failure
                    if (userRecordRowsAffected <= 0)
                    {
                        recordId = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return recordId;
        }

        public bool InsertMetricValues(int activityId, int recordId, Dictionary<int, double> metrics)
        {
            bool isSuccess = true;

            try
            {
                foreach (var metric in metrics)
                {
                    string sql = @"
                INSERT INTO metric_values (activity_id, record_id, metric_id, value)
                VALUES (@activityId, @recordId, @metricId, @value)";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@activityId", activityId);
                    cmd.Parameters.AddWithValue("@recordId", recordId);
                    cmd.Parameters.AddWithValue("@metricId", metric.Key);
                    cmd.Parameters.AddWithValue("@value", metric.Value);

                    if (cmd.ExecuteNonQuery() <= 0)
                    {
                        isSuccess = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting metric values: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isSuccess = false;
            }

            return isSuccess;
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
                 sql = @"INSERT INTO schedule_activity (schedule_id, activity_id, start_time, duration_minutes)
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
                sql = @"SELECT 
                            s.schedule_id AS 'Schedule Id',
                            a.activity_name AS 'Activity', 
                            s.scheduled_date AS 'Date', 
                            sa.start_time AS 'Start Time', 
                            sa.duration_minutes AS 'Duration'
                        FROM schedule s
                        JOIN schedule_activity sa ON s.schedule_id = sa.schedule_id
                        JOIN activity a ON sa.activity_id = a.activity_id
                        WHERE s.person_id = @personId;"; // Sort by date and time

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

                sql = "SELECT activity_id, activity_name FROM activity";
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
        public bool DeleteSchedule(int scheduleId)
        {
            try
            {
                // Open the database connection
                OpenConnection();

                // Delete schedule activities associated with the schedule
                sql = "DELETE FROM schedule_activity WHERE schedule_id = @scheduleId";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@scheduleId", scheduleId);
                cmd.ExecuteNonQuery();

                // Delete the schedule
                sql = "DELETE FROM schedule WHERE schedule_id = @scheduleId";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@scheduleId", scheduleId);

                return cmd.ExecuteNonQuery() > 0; // Return true if rows were affected
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting schedule: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                // Ensure the connection is closed
                CloseConnection();
            }
        }
        public DataTable GetRecords(int personID)
        {
            try
            {
                sql = @"
            SELECT 
                r.record_id AS 'Record Id',
                a.activity_name AS 'Activity',
                r.record_date AS 'Record Date',
                r.burned_calories AS 'Burned Calories',
                r.intensity_level AS 'Activity Type' -- Use the new column
            FROM record r
            JOIN activity a ON r.activity_id = a.activity_id
            WHERE r.person_id = @personId
            ORDER BY r.record_date DESC";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personId", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving records: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public bool DeleteRecord(int recordId)
        {
            try
            {
                OpenConnection();

                // Delete the associated rows in the user_record table
                sql = "DELETE FROM user_record WHERE record_id = @recordId";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@recordId", recordId);
                cmd.ExecuteNonQuery();

                // Delete the record from the record table
                sql = "DELETE FROM record WHERE record_id = @recordId";
                MySqlCommand deleteRecordCmd = new MySqlCommand(sql, conn);
                deleteRecordCmd.Parameters.AddWithValue("@recordId", recordId);

                return deleteRecordCmd.ExecuteNonQuery() > 0; // Return true if rows were affected
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        public string GetActivityDetails(int recordId)
        {
            StringBuilder activityDetails = new StringBuilder();

            try
            {
                string sql = @"
            SELECT m.metric_name, mv.value
            FROM metric_values mv
            JOIN metric m ON mv.metric_id = m.metric_id
            WHERE mv.record_id = @recordId";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@recordId", recordId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string metricName = reader["metric_name"].ToString();
                        double value = Convert.ToDouble(reader["value"]);
                        activityDetails.Append($"{metricName}: {value}, ");
                    }
                }

                // Remove the trailing comma and space
                if (activityDetails.Length > 0)
                {
                    activityDetails.Length -= 2;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving activity details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return activityDetails.ToString();
        }

        public double GetMetValue(int activityId, string intensity)
        {
            double metValue = 0;

            try
            {
                string sql = "SELECT met_value FROM met_values WHERE activity_id = @activityId AND intensity_level = @intensity";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@activityId", activityId);
                cmd.Parameters.AddWithValue("@intensity", intensity);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    metValue = Convert.ToDouble(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving MET value: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return metValue;
        }

        public bool InsertGoal(int personID, string goalType, double targetWeight, double dailyCaloriesTarget)
        {
            try
            {
                string sql = @"
        INSERT INTO goal_tracking 
        (person_id, goal_type, target_weight, daily_calories_target, is_achieved, created_at)
        VALUES (@personID, @goalType, @targetWeight, @dailyCaloriesTarget, 0, NOW())";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personID", personID);
                cmd.Parameters.AddWithValue("@goalType", goalType);
                cmd.Parameters.AddWithValue("@targetWeight", targetWeight);
                cmd.Parameters.AddWithValue("@dailyCaloriesTarget", dailyCaloriesTarget);

                return cmd.ExecuteNonQuery() > 0; // Returns true if the insert was successful
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool UpdateGoal(int goalId, string goalType, double targetWeight, double dailyCaloriesTarget)
        {
            try
            {
                sql = @"UPDATE goal_tracking 
                                    SET 
                                        goal_type = @goalType, 
                                        target_weight = @targetWeight, 
                                        daily_calories_target = @dailyCaloriesTarget 
                                    WHERE goal_id = @goalID";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@goalId", goalId);
                cmd.Parameters.AddWithValue("@goalType", goalType);
                cmd.Parameters.AddWithValue("@targetWeight", targetWeight);
                cmd.Parameters.AddWithValue("@dailyCaloriesTarget", dailyCaloriesTarget);

                return cmd.ExecuteNonQuery() > 0; // Returns true if the update was successful
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool DeleteGoal(int goalId)
        {
            MySqlTransaction transaction = null;

            try
            {
                transaction = conn.BeginTransaction();

                // Step 1: Delete from weight_tracking
                string weightSql = @"DELETE FROM weight_tracking 
                             WHERE person_id = (SELECT person_id FROM goal_tracking WHERE goal_id = @goalID)";
                MySqlCommand weightCmd = new MySqlCommand(weightSql, conn, transaction);
                weightCmd.Parameters.AddWithValue("@goalID", goalId);
                weightCmd.ExecuteNonQuery();

                // Step 2: Delete from goal_tracking
                string goalSql = @"DELETE FROM goal_tracking WHERE goal_id = @goalID";
                MySqlCommand goalCmd = new MySqlCommand(goalSql, conn, transaction);
                goalCmd.Parameters.AddWithValue("@goalID", goalId);
                goalCmd.ExecuteNonQuery();

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                MessageBox.Show($"Error deleting goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool MarkGoalAsAchieved(int goalID)
        {
            try
            {
                string sql = "UPDATE goal_tracking SET is_achieved = 1 WHERE goal_id = @goalID";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@goalID", goalID);

                return cmd.ExecuteNonQuery() > 0; // Returns true if the update was successful
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error marking goal as achieved: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public DataTable GetGoals(int personID)
        {
            DataTable dt = new DataTable();
            try
            {
                string sql = @"
        SELECT 
            goal_id, 
            goal_type, 
            target_weight, 
            daily_calories_target, 
            created_at, 
            is_achieved 
        FROM goal_tracking 
        WHERE person_id = @personID
        ORDER BY created_at ASC";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personID", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching goals: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        public int GetAchievedGoalsCountByType(int personID, string goalType)
        {
            try
            {
                string sql = "SELECT COUNT(*) FROM goal_tracking WHERE person_id = @personID AND is_achieved = 1 AND goal_type = @goalType";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personID", personID);
                cmd.Parameters.AddWithValue("@goalType", goalType);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving achieved goals count for {goalType}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0; // Return 0 on error
            }
        }
    }
}

