﻿using Fitness_Tracker.Entities;
using Fitness_Tracker.Utilities;
using Fitness_Tracker.Views;
using Google.Protobuf.Compiler;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

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


        public bool UpdateGoal(int goalId, string goalType, double targetWeight, double dailyCaloriesTarget, DateTime targetDate, int activityId)
        {
            try
            {
                string sql = @"UPDATE goal_tracking SET goal_type = @goalType, target_weight = @targetWeight, " +
                   "daily_calories_target = @dailyCaloriesTarget, target_date = @targetDate, activity_id = @activityId " +
                   "WHERE goal_id = @goalId";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@goalID", goalId);
                cmd.Parameters.AddWithValue("@goalType", goalType);
                cmd.Parameters.AddWithValue("@targetWeight", targetWeight);
                cmd.Parameters.AddWithValue("@dailyCaloriesTarget", dailyCaloriesTarget);
                cmd.Parameters.AddWithValue("@targetDate", targetDate.Date);
                cmd.Parameters.AddWithValue("@activityId", activityId);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool DeleteGoal(int goalId)
        {
            try
            {
                sql = "DELETE FROM goal_tracking WHERE goal_id = @goalId";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@goalId", goalId);

                return cmd.ExecuteNonQuery() > 0; // Returns true if the delete operation is successful
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool MarkGoalAsAchieved(int goalId)
        {
            try
            {
                string query = @"
        UPDATE goal_tracking
        SET is_achieved = 1, achieved_date = NOW()
        WHERE goal_id = @goalID;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@goalID", goalId);

                return cmd.ExecuteNonQuery() > 0;
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
                string sql = @"SELECT 
            goal_tracking.goal_id,
            goal_tracking.goal_type,
            goal_tracking.target_weight,
            goal_tracking.daily_calories_target,
            goal_tracking.is_achieved,
            goal_tracking.created_at,
            goal_tracking.target_date,
            activity.activity_name
        FROM 
            goal_tracking
        LEFT JOIN 
            activity ON goal_tracking.activity_id = activity.activity_id
        WHERE 
            goal_tracking.person_id = @personID
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

        public bool InsertGoal(int personID, string goalType, double targetWeight, double dailyCaloriesTarget, DateTime targetDate, int activityId)
        {
            try
            {
                string sql = @"
        INSERT INTO goal_tracking 
        (person_id, goal_type, target_weight, daily_calories_target, is_achieved, created_at, target_date, activity_id)
        VALUES (@personID, @goalType, @targetWeight, @dailyCaloriesTarget, 0, NOW(), @targetDate, @activityId)";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personID", personID);
                cmd.Parameters.AddWithValue("@goalType", goalType);
                cmd.Parameters.AddWithValue("@targetWeight", targetWeight);
                cmd.Parameters.AddWithValue("@dailyCaloriesTarget", dailyCaloriesTarget);
                cmd.Parameters.AddWithValue("@targetDate", targetDate.Date);
                cmd.Parameters.AddWithValue("@activityId", activityId);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool UpdateUserWeight(int personID, double newWeight)
        {
            try
            {
                string sql = "UPDATE person SET weight = @newWeight WHERE person_id = @personID";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@newWeight", newWeight);
                cmd.Parameters.AddWithValue("@personID", personID);

                return cmd.ExecuteNonQuery() > 0; // Return true if rows were updated
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating weight: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        public int GetActivityIdByName(string activityName)
        {
            try
            {
                sql = "SELECT activity_id FROM activity WHERE activity_name = @activityName";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@activityName", activityName);

                object result = cmd.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int activityId))
                {
                    return activityId;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving activity ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return -1; // Return -1 if not found
        }

        public DataTable GetGoalAchievementByMonth()
        {
            try
            {

                string query = @"
            SELECT 
                DATE_FORMAT(achieved_date, '%Y-%m') AS achievement_month, 
                COUNT(*) AS achieved_count
            FROM 
                goal_tracking
            WHERE 
                is_achieved = 1
            GROUP BY 
                DATE_FORMAT(achieved_date, '%Y-%m')
            ORDER BY 
                achievement_month ASC";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching goal achievement by month: {ex.Message}");
            }
        }

        public DataTable GetAchievedAndPendingGoals(int personID)
        {
            string query = @"
        SELECT 
            SUM(CASE WHEN is_achieved = 1 THEN 1 ELSE 0 END) AS achieved_count,
            SUM(CASE WHEN is_achieved = 0 THEN 1 ELSE 0 END) AS pending_count
        FROM goal_tracking
        WHERE person_id = @personID";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@personID", personID);
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public bool InsertWeightTracking(int personID, double weight)
        {
            try
            {
                string sql = @"
        INSERT INTO weight_tracking (person_id, recorded_date, weight)
        VALUES (@personID, NOW(), @weight);";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personID", personID);
                cmd.Parameters.AddWithValue("@weight", weight);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting weight tracking: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public DataTable GetWeightTrackingData(int personID)
        {
            try
            {
                string query = @"
            SELECT 
                DATE_FORMAT(recorded_date, '%Y-%m-%d') AS recorded_date,
                weight
            FROM 
                weight_tracking
            WHERE 
                person_id = @personID
            ORDER BY 
                recorded_date ASC";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personID", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching weight tracking data: {ex.Message}");
                return null;
            }
        }

        public DataTable GetRecordsForDateRange(int personID, DateTime startDate, DateTime endDate)
        {
            try
            {
                string query = @"
        SELECT 
            r.record_id AS 'Record Id',
            a.activity_name AS 'Activity',
            r.record_date AS 'Record Date',
            r.burned_calories AS 'Burned Calories',
            r.intensity_level AS 'Activity Type'
        FROM record r
        JOIN activity a ON r.activity_id = a.activity_id
        WHERE r.person_id = @personID AND r.record_date BETWEEN @startDate AND @endDate
        ORDER BY r.record_date DESC";

                cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personID", personID);
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving records for date range: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public DataTable GetRecordsByActivity(int personID, string activityName)
        {
            try
            {
                string query = @"
        SELECT 
            r.record_id AS 'Record Id',
            a.activity_name AS 'Activity',
            r.record_date AS 'Record Date',
            r.burned_calories AS 'Burned Calories',
            r.intensity_level AS 'Activity Type'
        FROM record r
        JOIN activity a ON r.activity_id = a.activity_id
        WHERE r.person_id = @personID AND a.activity_name = @activityName
        ORDER BY r.record_date DESC";

                cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personID", personID);
                cmd.Parameters.AddWithValue("@activityName", activityName);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving records by activity: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public DataTable GetDailyCaloriesData(int personID, DateTime startDate, DateTime endDate)
        {
            try
            {
                string query = @"
            SELECT 
                DATE(record_date) AS record_date, 
                SUM(burned_calories) AS total_calories
            FROM record
            WHERE person_id = @personID AND record_date BETWEEN @startDate AND @endDate
            GROUP BY DATE(record_date)
            ORDER BY record_date ASC;";

                cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personID", personID);
                cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"));

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving daily calories data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public DataTable GetSchedulesSummary(int personID)
        {
            try
            {
                string query = @"
            SELECT 
                a.activity_name AS Activity,
                COUNT(s.schedule_id) AS TotalSchedules
            FROM schedule s
            JOIN schedule_activity sa ON s.schedule_id = sa.schedule_id
            JOIN activity a ON sa.activity_id = a.activity_id
            WHERE s.person_id = @personID
            GROUP BY a.activity_name
            ORDER BY TotalSchedules DESC";

                cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personID", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving schedules summary: {ex.Message}");
            }
        }
        public int GetScheduleCountForDate(int personID, DateTime date)
        {
            try
            {
                string query = @"
            SELECT COUNT(*) 
            FROM schedule s
            JOIN schedule_activity sa ON s.schedule_id = sa.schedule_id
            WHERE s.person_id = @personId AND s.scheduled_date = @scheduleDate;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personId", personID);
                cmd.Parameters.AddWithValue("@scheduleDate", date.Date);

                object result = cmd.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving schedule count: {ex.Message}");
            }
        }

        public DataTable GetFilteredSchedules(int personID, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string query = @"
            SELECT 
                s.schedule_id AS 'Schedule Id',
                a.activity_name AS 'Activity',
                s.scheduled_date AS 'Date',
                sa.start_time AS 'Start Time',
                sa.duration_minutes AS 'Duration'
            FROM schedule s
            JOIN schedule_activity sa ON s.schedule_id = sa.schedule_id
            JOIN activity a ON sa.activity_id = a.activity_id
            WHERE s.person_id = @personID"
                ;

                if (startDate.HasValue && endDate.HasValue)
                {
                    query += " AND DATE(s.scheduled_date) BETWEEN @startDate AND @endDate";
                }

                query += " ORDER BY s.scheduled_date ASC";

                cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personID", personID);
                cmd.Parameters.AddWithValue("@startDate", startDate.HasValue ? (object)startDate.Value.ToString("yyyy-MM-dd") : DBNull.Value);
                cmd.Parameters.AddWithValue("@endDate", endDate.HasValue ? (object)endDate.Value.ToString("yyyy-MM-dd") : DBNull.Value);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving filtered schedules: {ex.Message}");
            }
        }

        public bool IsOverlappingSchedule(int personId, DateTime date, TimeSpan startTime, int durationMinutes)
        {
            string sql = @"
        SELECT COUNT(*) 
        FROM schedule s
        JOIN schedule_activity sa ON s.schedule_id = sa.schedule_id
        WHERE s.person_id = @personId
        AND s.scheduled_date = @date
        AND (
            (@startTime >= sa.start_time AND @startTime < ADDTIME(sa.start_time, SEC_TO_TIME(sa.duration_minutes * 60)))
            OR (ADDTIME(@startTime, SEC_TO_TIME(@durationMinutes * 60)) > sa.start_time 
                AND ADDTIME(@startTime, SEC_TO_TIME(@durationMinutes * 60)) <= ADDTIME(sa.start_time, SEC_TO_TIME(sa.duration_minutes * 60)))
        )";

            cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@personId", personId);
            cmd.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@startTime", startTime.ToString(@"hh\:mm\:ss")); // Convert TimeSpan to MySQL TIME format
            cmd.Parameters.AddWithValue("@durationMinutes", durationMinutes);

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public DataTable GetScheduleDistributionByTime(int personId)
        {
            try
            {
                string query = @"
            SELECT 
                HOUR(sa.start_time) AS Hour, 
                COUNT(*) AS Count
            FROM schedule s
            JOIN schedule_activity sa ON s.schedule_id = sa.schedule_id
            WHERE s.person_id = @personId
            GROUP BY Hour
            ORDER BY Hour;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personId", personId);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving schedule distribution: {ex.Message}");
            }
        }
        public DataTable GetActivityGraphData(int personID, int activityID)
        {
            try
            {
                string query = @"
    SELECT 
        DATE(r.record_date) AS Date, 
        SUM(r.burned_calories) AS CaloriesBurned
    FROM record r
    JOIN activity a ON r.activity_id = a.activity_id
    WHERE r.person_id = @personId AND r.activity_id = @activityId
    GROUP BY DATE(r.record_date)
    ORDER BY DATE(r.record_date);";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personId", personID);
                cmd.Parameters.AddWithValue("@activityId", activityID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving activity graph data: {ex.Message}");
            }
        }

        public DataTable GetSwimmingMetricsOverTime(int personID)
        {
            try
            {
                string query = @"
        SELECT 
            r.record_date AS Date, 
            MAX(CASE WHEN m.metric_name = 'Laps' THEN mv.value ELSE 0 END) AS Laps,
            MAX(CASE WHEN m.metric_name = 'Time Taken' THEN mv.value ELSE 0 END) AS TimeTaken,
            MAX(CASE WHEN m.metric_name = 'Average Heart Rate' THEN mv.value ELSE 0 END) AS AvgHeartRate
        FROM 
            metric_values mv
        JOIN 
            metric m ON mv.metric_id = m.metric_id
        JOIN 
            record r ON mv.record_id = r.record_id
        JOIN 
            activity a ON r.activity_id = a.activity_id
        WHERE 
            r.person_id = @personID
            AND a.activity_name = 'Swimming'
        GROUP BY 
            r.record_date
        ORDER BY 
            r.record_date;
        ";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personID", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving swimming metrics over time: {ex.Message}");
            }
        }

        public DataTable GetSwimmingSummary(int personID)
        {
            try
            {
                string query = @"
        SELECT 
            SUM(CASE WHEN m.metric_name = 'Laps' THEN mv.value ELSE 0 END) AS TotalLaps,
            AVG(CASE WHEN m.metric_name = 'Average Heart Rate' THEN mv.value ELSE NULL END) AS AvgHeartRate,
            SUM(CASE WHEN m.metric_name = 'Time Taken' THEN mv.value ELSE 0 END) AS TotalTime
        FROM 
            metric_values mv
        JOIN 
            metric m ON mv.metric_id = m.metric_id
        JOIN 
            record r ON mv.record_id = r.record_id
        WHERE 
            r.person_id = @personId
            AND r.activity_id = 1;"; // 1 represents Swimming activity

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personId", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving swimming summary: {ex.Message}");
            }
        }

        public DataRow GetRecentSwimmingActivity(int personId)
        {
            try
            {
                string query = @"
        SELECT 
            r.record_date AS `Date`,
            SUM(CASE WHEN m.metric_name = 'Laps' THEN mv.value ELSE 0 END) AS `Laps`,
            SUM(CASE WHEN m.metric_name = 'Time Taken' THEN mv.value ELSE 0 END) AS `TimeTaken`,
            AVG(CASE WHEN m.metric_name = 'Average Heart Rate' THEN mv.value ELSE NULL END) AS `HeartRate`,
            r.burned_calories AS `CaloriesBurned`
        FROM 
            record r
        JOIN 
            metric_values mv ON r.record_id = mv.record_id
        JOIN 
            metric m ON mv.metric_id = m.metric_id
        WHERE 
            r.person_id = @personId
            AND r.activity_id = 1
        GROUP BY 
            r.record_id, r.record_date, r.burned_calories
        ORDER BY 
            r.record_date DESC
        LIMIT 1;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personId", personId);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows.Count > 0 ? dt.Rows[0] : null; // Return the first row or null if no data
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving recent swimming activity: {ex.Message}");
            }
        }

        public double GetMaxCaloriesForActivity(int personID, int activityID)
        {
            try
            {
                string query = @"
SELECT MAX(r.burned_calories) AS MaxCalories
FROM record r
WHERE r.person_id = @personId
  AND r.activity_id = @activityId;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personId", personID);
                cmd.Parameters.AddWithValue("@activityId", activityID);

                object result = cmd.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToDouble(result) : 0.0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving maximum calories for activity ID {activityID}: {ex.Message}");
            }
        }

        public DataTable GetHistoricalComparison(int personID)
        {
            try
            {
                string query = @"
        SELECT 
            a.activity_name AS ActivityName,
            SUM(r.burned_calories) AS CaloriesBurned
        FROM record r
        JOIN activity a ON r.activity_id = a.activity_id
        WHERE r.person_id = @personId
        GROUP BY a.activity_id, a.activity_name
        ORDER BY CaloriesBurned DESC;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personId", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving historical comparison data: {ex.Message}");
            }
        }

        public DataTable GetWalkingMetricsOverTime(int personID)
        {
            try
            {
                string query = @"
         SELECT 
            r.record_date AS Date, 
            MAX(CASE WHEN m.metric_name = 'Steps' THEN mv.value ELSE 0 END) AS TotalSteps,
            MAX(CASE WHEN m.metric_name = 'Distance' THEN mv.value ELSE 0 END) AS TotalDistance,
            MAX(CASE WHEN m.metric_name = 'Time Taken' THEN mv.value ELSE 0 END) AS TotalTime
        FROM 
            metric_values mv
        JOIN 
            metric m ON mv.metric_id = m.metric_id
        JOIN 
            record r ON mv.record_id = r.record_id
        JOIN 
            activity a ON r.activity_id = a.activity_id
        WHERE 
            r.person_id = @personID
            AND a.activity_name = 'Walking'
        GROUP BY 
            r.record_date
        ORDER BY 
            r.record_date;
        ";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personID", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving Walking metrics over time: {ex.Message}");
            }

        }

        public DataTable GetWalkingSummary(int personID)
        {
            try
            {
                string query = @"
        SELECT 
            SUM(CASE WHEN m.metric_name = 'Steps' THEN mv.value ELSE 0 END) AS TotalSteps,
            SUM(CASE WHEN m.metric_name = 'Distance' THEN mv.value ELSE 0 END) AS TotalDistance,
            SUM(CASE WHEN m.metric_name = 'Time Taken' THEN mv.value ELSE 0 END) AS TotalTime
        FROM 
            metric_values mv
        JOIN 
            metric m ON mv.metric_id = m.metric_id
        JOIN 
            record r ON mv.record_id = r.record_id
        WHERE 
            r.person_id = @personId
            AND r.activity_id = 2;"; // 2 represents Walking activity

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personId", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving walking summary: {ex.Message}");
            }
        }

        public DataRow GetRecentWalkingActivity(int personID)
        {
            try
            {
                string query = @"
        SELECT 
            r.record_date AS `Date`,
            SUM(CASE WHEN m.metric_name = 'Steps' THEN mv.value ELSE 0 END) AS `Steps`,
            SUM(CASE WHEN m.metric_name = 'Distance' THEN mv.value ELSE 0 END) AS `Distance`,
            SUM(CASE WHEN m.metric_name = 'Time Taken' THEN mv.value ELSE NULL END) AS `TimeTaken`,
            r.burned_calories AS `CaloriesBurned`
        FROM 
            record r
        JOIN 
            metric_values mv ON r.record_id = mv.record_id
        JOIN 
            metric m ON mv.metric_id = m.metric_id
        WHERE 
            r.person_id = @personId
            AND r.activity_id = 2
        GROUP BY 
            r.record_id, r.record_date, r.burned_calories
        ORDER BY 
            r.record_date DESC
        LIMIT 1;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personId", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows.Count > 0 ? dt.Rows[0] : null; // Return the first row or null if no data
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving recent swimming activity: {ex.Message}");
            }
        }

        public DataTable GetCyclingMetricsOverTime(int personID)
        {
            try
            {
                string query = @"
                            SELECT 
                        r.record_date AS Date, 
                        MAX(CASE WHEN m.metric_name = 'Speed' THEN mv.value ELSE 0 END) AS Speed,
                        MAX(CASE WHEN m.metric_name = 'Distance' THEN mv.value ELSE 0 END) AS Distance,
                        MAX(CASE WHEN m.metric_name = 'Ride Duration' THEN mv.value ELSE 0 END) AS RideDuration
                    FROM 
                        metric_values mv
                    JOIN 
                        metric m ON mv.metric_id = m.metric_id
                    JOIN 
                        record r ON mv.record_id = r.record_id
                    JOIN 
                        activity a ON r.activity_id = a.activity_id
                    WHERE 
                        r.person_id = @personID
                        AND a.activity_name = 'Cycling' -- Corrected activity name
                    GROUP BY 
                        r.record_date
                    ORDER BY 
                        r.record_date; ";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personID", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving Walking metrics over time: {ex.Message}");
            }
        }

        public DataRow GetRecentCyclingActivity(int personID)
        {
            try
            {
                string query = @"
                                SELECT 
                            r.record_date AS `Date`,
                            MAX(CASE WHEN m.metric_name = 'Speed' THEN mv.value ELSE 0 END) AS Speed,
                            MAX(CASE WHEN m.metric_name = 'Distance' THEN mv.value ELSE 0 END) AS Distance,
                            MAX(CASE WHEN m.metric_name = 'Ride Duration' THEN mv.value ELSE 0 END) AS RideDuration,
                            r.burned_calories AS `CaloriesBurned`
                        FROM 
                            record r
                        JOIN 
                            metric_values mv ON r.record_id = mv.record_id
                        JOIN 
                            metric m ON mv.metric_id = m.metric_id
                        JOIN 
                            activity a ON r.activity_id = a.activity_id
                        WHERE 
                            r.person_id = @personId
                            AND a.activity_name = 'Cycling' -- Corrected activity name
                        GROUP BY 
                            r.record_id, r.record_date, r.burned_calories
                        ORDER BY 
                            r.record_date DESC
                        LIMIT 1;"
                        ;

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personId", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows.Count > 0 ? dt.Rows[0] : null; // Return the first row or null if no data
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving recent swimming activity: {ex.Message}");
            }
        }

        public DataTable GetCyclingSummary(int personID)
        {
            try
            {
                string query = @"
        SELECT 
            Avg(CASE WHEN m.metric_name = 'Speed' THEN mv.value ELSE 0 END) AS Speed,
            SUM(CASE WHEN m.metric_name = 'Distance' THEN mv.value ELSE 0 END) AS TotalDistance,
            SUM(CASE WHEN m.metric_name = 'Ride Duration' THEN mv.value ELSE 0 END) AS RideDuration
        FROM 
            metric_values mv
        JOIN 
            metric m ON mv.metric_id = m.metric_id
        JOIN 
            record r ON mv.record_id = r.record_id
        WHERE 
            r.person_id = @personId
            AND r.activity_id = 3;"; // 2 represents Walking activity

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personId", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving walking summary: {ex.Message}");
            }
        }

        public DataTable GetHikingMetricsOverTime(int personID)
        {
            try
            {
                string query = @"
        SELECT 
            r.record_date AS Date, 
            MAX(CASE WHEN m.metric_name = 'Elevation Gained' THEN mv.value ELSE 0 END) AS ElevationGained,
            MAX(CASE WHEN m.metric_name = 'Distance' THEN mv.value ELSE 0 END) AS Distance,
            MAX(CASE WHEN m.metric_name = 'Time Taken' THEN mv.value ELSE 0 END) AS TimeTaken
        FROM 
            metric_values mv
        JOIN 
            metric m ON mv.metric_id = m.metric_id
        JOIN 
            record r ON mv.record_id = r.record_id
        JOIN 
            activity a ON r.activity_id = a.activity_id
        WHERE 
            r.person_id = @personID
            AND a.activity_name = 'Hiking'
        GROUP BY 
            r.record_date
        ORDER BY 
            r.record_date;
        ";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personID", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving hiking metrics over time: {ex.Message}");
            }
        }

        public DataTable GetHikingSummary(int personID)
        {
            try
            {
                string query = @"
        SELECT 
            SUM(CASE WHEN m.metric_name = 'Elevation Gained' THEN mv.value ELSE 0 END) AS TotalElevation,
            SUM(CASE WHEN m.metric_name = 'Distance' THEN mv.value ELSE 0 END) AS TotalDistance,
            SUM(CASE WHEN m.metric_name = 'Time Taken' THEN mv.value ELSE 0 END) AS TotalTime
        FROM 
            metric_values mv
        JOIN 
            metric m ON mv.metric_id = m.metric_id
        JOIN 
            record r ON mv.record_id = r.record_id
        JOIN 
            activity a ON r.activity_id = a.activity_id
        WHERE 
            r.person_id = @personID
            AND a.activity_name = 'Hiking';
        ";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personID", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving hiking summary: {ex.Message}");
            }
        }

        public DataRow GetRecentHikingActivity(int personID)
        {
            try
            {
                string query = @"
        SELECT 
            r.record_date AS `Date`,
            SUM(CASE WHEN m.metric_name = 'Elevation Gained' THEN mv.value ELSE 0 END) AS ElevationGained,
            SUM(CASE WHEN m.metric_name = 'Distance' THEN mv.value ELSE 0 END) AS Distance,
            SUM(CASE WHEN m.metric_name = 'Time Taken' THEN mv.value ELSE 0 END) AS TimeTaken,
            r.burned_calories AS CaloriesBurned
        FROM 
            record r
        JOIN 
            metric_values mv ON r.record_id = mv.record_id
        JOIN 
            metric m ON mv.metric_id = m.metric_id
        JOIN 
            activity a ON r.activity_id = a.activity_id
        WHERE 
            r.person_id = @personID
            AND a.activity_name = 'Hiking'
        GROUP BY 
            r.record_id, r.record_date, r.burned_calories
        ORDER BY 
            r.record_date DESC
        LIMIT 1;
        ";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personID", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows.Count > 0 ? dt.Rows[0] : null; // Return the first row or null if no data
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving recent hiking activity: {ex.Message}");
            }
        }

        public DataTable GetWeightliftingSummary(int personID)
        {
            try
            {
                string query = @"
        SELECT 
            AVG(CASE WHEN m.metric_name = 'Weight Lifted' THEN mv.value ELSE 0 END) AS AvgWeight,
            SUM(CASE WHEN m.metric_name = 'Repetitions' THEN mv.value ELSE 0 END) AS TotalRepetitions,
            SUM(CASE WHEN m.metric_name = 'Sets Completed' THEN mv.value ELSE 0 END) AS TotalSets
        FROM 
            metric_values mv
        JOIN 
            metric m ON mv.metric_id = m.metric_id
        JOIN 
            record r ON mv.record_id = r.record_id
        WHERE 
            r.person_id = @personId
            AND r.activity_id = 5; -- 5 represents Weightlifting Activity";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personId", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving weightlifting summary: {ex.Message}");
            }
        }

        public DataRow GetRecentWeightliftingActivity(int personID)
        {
            try
            {
                string query = @"
        SELECT 
            r.record_date AS `Date`,
            MAX(CASE WHEN m.metric_name = 'Weight Lifted' THEN mv.value ELSE 0 END) AS WeightLifted,
            MAX(CASE WHEN m.metric_name = 'Repetitions' THEN mv.value ELSE 0 END) AS Repetitions,
            MAX(CASE WHEN m.metric_name = 'Sets Completed' THEN mv.value ELSE 0 END) AS SetsCompleted,
            r.burned_calories AS CaloriesBurned
        FROM 
            metric_values mv
        JOIN 
            metric m ON mv.metric_id = m.metric_id
        JOIN 
            record r ON mv.record_id = r.record_id
        WHERE 
            r.person_id = @personID
            AND r.activity_id = 5 -- Weightlifting Activity ID
        GROUP BY 
            r.record_id, r.record_date, r.burned_calories
        ORDER BY 
            r.record_date DESC
        LIMIT 1;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personID", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving recent weightlifting activity: {ex.Message}");
            }
        }

        public DataTable GetWeightliftingMetricsOverTime(int personID)
        {
            try
            {
                string query = @"
        SELECT 
            r.record_date AS Date, 
            MAX(CASE WHEN m.metric_name = 'Weight Lifted' THEN mv.value ELSE 0 END) AS WeightLifted,
            MAX(CASE WHEN m.metric_name = 'Repetitions' THEN mv.value ELSE 0 END) AS Repetitions,
            MAX(CASE WHEN m.metric_name = 'Sets Completed' THEN mv.value ELSE 0 END) AS SetsCompleted
        FROM 
            metric_values mv
        JOIN 
            metric m ON mv.metric_id = m.metric_id
        JOIN 
            record r ON mv.record_id = r.record_id
        JOIN 
            activity a ON r.activity_id = a.activity_id
        WHERE 
            r.person_id = @personID
            AND a.activity_id = 5 -- 5 is Weightlifting Activity ID
        GROUP BY 
            r.record_date
        ORDER BY 
            r.record_date;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personID", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving weightlifting metrics over time: {ex.Message}");
            }
        }

        public DataTable GetRowingMetricsOverTime(int personID)
        {
            try
            {
                string query = @"
        SELECT 
            r.record_date AS Date, 
            MAX(CASE WHEN m.metric_name = 'Total Strokes' THEN mv.value ELSE 0 END) AS TotalStrokes,
            MAX(CASE WHEN m.metric_name = 'Distance' THEN mv.value ELSE 0 END) AS Distance,
            MAX(CASE WHEN m.metric_name = 'Time Taken' THEN mv.value ELSE 0 END) AS TimeTaken
        FROM 
            metric_values mv
        JOIN 
            metric m ON mv.metric_id = m.metric_id
        JOIN 
            record r ON mv.record_id = r.record_id
        JOIN 
            activity a ON r.activity_id = a.activity_id
        WHERE 
            r.person_id = @personID
            AND a.activity_name = 'Rowing'
        GROUP BY 
            r.record_date
        ORDER BY 
            r.record_date;
        ";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personID", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving rowing metrics over time: {ex.Message}");
            }
        }
        public DataTable GetRowingSummary(int personID)
        {
            try
            {
                string query = @"
        SELECT 
            SUM(CASE WHEN m.metric_name = 'Total Strokes' THEN mv.value ELSE 0 END) AS TotalStrokes,
            SUM(CASE WHEN m.metric_name = 'Distance' THEN mv.value ELSE 0 END) AS TotalDistance,
            SUM(CASE WHEN m.metric_name = 'Time Taken' THEN mv.value ELSE 0 END) AS TotalTime
        FROM 
            metric_values mv
        JOIN 
            metric m ON mv.metric_id = m.metric_id
        JOIN 
            record r ON mv.record_id = r.record_id
        WHERE 
            r.person_id = @personID
            AND r.activity_id = 6; -- Rowing Activity ID
        ";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personID", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving rowing summary: {ex.Message}");
            }
        }

        public DataRow GetRecentRowingActivity(int personID)
        {
            try
            {
                string query = @"
        SELECT 
            r.record_date AS Date,
            SUM(CASE WHEN m.metric_name = 'Total Strokes' THEN mv.value ELSE 0 END) AS TotalStrokes,
            SUM(CASE WHEN m.metric_name = 'Distance' THEN mv.value ELSE 0 END) AS Distance,
            SUM(CASE WHEN m.metric_name = 'Time Taken' THEN mv.value ELSE 0 END) AS TimeTaken,
            r.burned_calories AS CaloriesBurned
        FROM 
            record r
        JOIN 
            metric_values mv ON r.record_id = mv.record_id
        JOIN 
            metric m ON mv.metric_id = m.metric_id
        WHERE 
            r.person_id = @personID
            AND r.activity_id = 6 -- Rowing Activity ID
        GROUP BY 
            r.record_id, r.record_date, r.burned_calories
        ORDER BY 
            r.record_date DESC
        LIMIT 1;
        ";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personID", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving recent rowing activity: {ex.Message}");
            }
        }

        public DataTable GetUpcomingActivitySchedules(int personID, int activityID)
        {
            try
            {
                string query = @"
        SELECT s.scheduled_date AS Date, sa.start_time AS StartTime, sa.duration_minutes AS Duration
        FROM schedule s
        JOIN schedule_activity sa ON s.schedule_id = sa.schedule_id
        WHERE s.person_id = @personId 
        AND sa.activity_id = @activityId 
        AND s.scheduled_date >= CURDATE()
        ORDER BY s.scheduled_date, sa.start_time;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@personId", personID);
                cmd.Parameters.AddWithValue("@activityId", activityID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving upcoming activity schedules: {ex.Message}");
            }
        }


    }
}

