using Fitness_Tracker.Entities;
using Fitness_Tracker.Utilities;
using Fitness_Tracker.Views;
using Google.Protobuf.Compiler;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management.Instrumentation;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Fitness_Tracker.dao
{
    public class ConnectionDB
    {
        // Singleton instance
        private static ConnectionDB instance;
        private MySqlConnection conn;
        private MySqlCommand cmd;
        private MySqlDataReader reader;

        private static readonly object lockObj = new object();
        
        private readonly string server = "localhost";
        private readonly string username = "root";
        private readonly string password = "monk2962006";
        private readonly string database = "fitness_tracker";
        private string sql;

        private ConnectionDB()
        {
            string url = $"Server={server};Uid={username};Pwd={password};Database={database}";
            conn = new MySqlConnection(url);
            InitializeDatabase();
        }

        // Thread-safe Singleton Instance Getter
        public static ConnectionDB GetInstance()
        {
            if (instance == null)
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = new ConnectionDB();
                    }
                }
            }
            return instance;
        }

        public void OpenConnection()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open connection: " + ex.Message);
            }
        }
        public void CloseConnection()
        {
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to close connection: " + ex.Message);
            }
        }
        private void InitializeDatabase()
        {
            try
            {
                // Step 1: Create Database If Not Exists
                using (var tempConn = new MySqlConnection($"Server={server};Uid={username};Pwd={password};"))
                {
                    tempConn.Open();
                    string createDatabaseQuery = "CREATE DATABASE IF NOT EXISTS fitness_tracker";
                    using (var cmd = new MySqlCommand(createDatabaseQuery, tempConn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                // Step 2: Connect to the Newly Created Database
                OpenConnection();

                // Step 3: Create Tables
                string schemaQuery = @"
            USE fitness_tracker;

            CREATE TABLE IF NOT EXISTS person (
                person_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                username VARCHAR(255) NOT NULL,
                password VARCHAR(255) NOT NULL,
                first_name VARCHAR(255),
                last_name VARCHAR(255),
                email VARCHAR(255),
                date_of_birth DATE,
                gender VARCHAR(255),
                mobile VARCHAR(255),
                weight DOUBLE,
                height DOUBLE,
                photo_path VARCHAR(255)
            );

            CREATE TABLE IF NOT EXISTS activity (
                activity_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                activity_name VARCHAR(100) UNIQUE NOT NULL,
                description VARCHAR(255) NOT NULL
            );

            CREATE TABLE IF NOT EXISTS metric (
                metric_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                activity_id INT NOT NULL,
                metric_name VARCHAR(255) NOT NULL,
                calculation_factor DOUBLE NOT NULL,
                FOREIGN KEY (activity_id) REFERENCES activity(activity_id)
            );

            CREATE TABLE IF NOT EXISTS record (
                record_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                person_id INT NOT NULL,
                activity_id INT NOT NULL,
                record_date DATE NOT NULL,
                burned_calories DOUBLE NOT NULL,
                intensity_level VARCHAR(50),
                FOREIGN KEY (person_id) REFERENCES person(person_id) ON DELETE CASCADE,
                FOREIGN KEY (activity_id) REFERENCES activity(activity_id)
            );

            CREATE TABLE IF NOT EXISTS user_record (
                person_id INT NOT NULL,
                record_id INT NOT NULL,
                FOREIGN KEY (person_id) REFERENCES person(person_id) ON DELETE CASCADE,
                FOREIGN KEY (record_id) REFERENCES record(record_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS metric_values (
                metric_value_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                activity_id INT NOT NULL,
                record_id INT NOT NULL,
                metric_id INT NOT NULL,
                value DOUBLE NOT NULL,
                FOREIGN KEY (activity_id) REFERENCES activity(activity_id) ON DELETE CASCADE,
                FOREIGN KEY (record_id) REFERENCES record(record_id) ON DELETE CASCADE,
                FOREIGN KEY (metric_id) REFERENCES metric(metric_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS goal_tracking (
                goal_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                person_id INT NOT NULL,
                goal_type VARCHAR(255) NOT NULL,
                target_weight DECIMAL(5,2) NOT NULL,
                daily_calories_target DOUBLE NOT NULL,
                is_achieved BOOLEAN DEFAULT FALSE,
                created_at DATE,
                target_date DATE,
                activity_id INT,
                achieved_date DATE NULL,
                FOREIGN KEY (person_id) REFERENCES person(person_id),
                FOREIGN KEY (activity_id) REFERENCES activity(activity_id) ON DELETE SET NULL
            );

            CREATE TABLE IF NOT EXISTS schedule (
                schedule_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                person_id INT NOT NULL,
                scheduled_date DATE NOT NULL,
                FOREIGN KEY (person_id) REFERENCES person(person_id)
            );

            CREATE TABLE IF NOT EXISTS schedule_activity (
                schedule_id INT NOT NULL,
                activity_id INT NOT NULL,
                start_time TIME NOT NULL,
                duration_minutes INT NOT NULL,
                FOREIGN KEY (schedule_id) REFERENCES schedule(schedule_id),
                FOREIGN KEY (activity_id) REFERENCES activity(activity_id)
            );

            CREATE TABLE IF NOT EXISTS met_values (
                met_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                activity_id INT NOT NULL,
                intensity_level VARCHAR(50) NOT NULL,
                met_value DOUBLE NOT NULL,
                FOREIGN KEY (activity_id) REFERENCES activity(activity_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS weight_tracking (
                weight_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                person_id INT NOT NULL,
                recorded_date DATE NOT NULL,
                weight DOUBLE NOT NULL,
                FOREIGN KEY (person_id) REFERENCES person(person_id) ON DELETE CASCADE
            );";

                ExecuteNonQuery(schemaQuery);

                // Step 4: Insert Default Data
                string insertActivitiesQuery = @"
            INSERT IGNORE INTO activity (activity_name, description)
            VALUES 
                ('Swimming', 'A water-based physical activity'),
                ('Walking', 'A low-impact cardio exercise'),
                ('Cycling', 'A cardio exercise performed using a bicycle'),
                ('Hiking', 'Walking in nature or on trails'),
                ('Weightlifting', 'A strength-based physical activity involving lifting weights'),
                ('Rowing', 'A full-body workout simulating rowing motion');";
                ExecuteNonQuery(insertActivitiesQuery);

                string insertMetricsQuery = @"
            INSERT IGNORE INTO metric (activity_id, metric_name, calculation_factor)
            VALUES
                (1, 'Laps', 0.1), (1, 'Time Taken', 0.02), (1, 'Average Heart Rate', 0.05),
                (2, 'Steps', 0.05), (2, 'Distance', 0.1), (2, 'Time Taken', 0.02),
                (3, 'Speed', 0.2), (3, 'Distance', 0.1), (3, 'Ride Duration', 0.03),
                (4, 'Elevation Gained', 0.15), (4, 'Distance', 0.1), (4, 'Time Taken', 0.02),
                (5, 'Weight Lifted', 0.25), (5, 'Repetitions', 0.05), (5, 'Sets Completed', 0.1),
                (6, 'Total Strokes', 0.1), (6, 'Distance', 0.08), (6, 'Time Taken', 0.02);";
                ExecuteNonQuery(insertMetricsQuery);

                string insertMetValuesQuery = @"
            INSERT IGNORE INTO met_values (activity_id, intensity_level, met_value)
            VALUES
                (1, 'Light', 6.0), (1, 'Moderate', 8.0), (1, 'Vigorous', 9.8),
                (2, 'Light', 3.3), (2, 'Moderate', 5.0), (2, 'Vigorous', 6.3),
                (3, 'Light', 4.0), (3, 'Moderate', 8.0), (3, 'Vigorous', 10.0),
                (4, 'Moderate', 6.0), (4, 'Vigorous', 9.0),
                (5, 'Light', 3.0), (5, 'Moderate', 4.5), (5, 'Vigorous', 6.0),
                (6, 'Light', 3.5), (6, 'Moderate', 6.0), (6, 'Vigorous', 8.5);";
                ExecuteNonQuery(insertMetValuesQuery);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Initialization Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection();
            }
        }
        private void ExecuteNonQuery(string query)
        {
            try
            {
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"SQL Execution Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Check if User Exists
        public bool IsUserExists(string username, string email)
        {
            try
            {
                OpenConnection();
                sql = "SELECT COUNT(*) FROM person WHERE username = @username OR email = @email";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@email", email);

                int userExists = Convert.ToInt32(cmd.ExecuteScalar());
                return userExists > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking user existence: " + ex.Message);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
        // Validate User Login
        public User IsValidUser(string username, string password)
        {
            try
            {
                OpenConnection();
                string sql = "SELECT * FROM person WHERE username = @username";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@username", username);

                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Safely retrieve data
                    string dbPassword = reader["password"] != DBNull.Value ? reader.GetString("password") : null;

                    // Verify password
                    if (!string.IsNullOrEmpty(dbPassword) && PasswordHelper.VerifyPassword(password, dbPassword))
                    {
                        // Create User instance and populate data
                        User user = User.GetInstance();
                        user.SetUserData(
                            reader.GetInt32("person_id"),
                            reader.GetString("username"),
                            dbPassword,
                            reader.GetString("first_name"),
                            reader.GetString("last_name"),
                            reader.GetString("email"),
                            reader.GetDateTime("date_of_birth"),
                            reader.GetString("gender"),
                            reader.GetString("mobile"),
                            reader.GetDouble("weight"),
                            reader.GetDouble("height"),
                            reader.IsDBNull(reader.GetOrdinal("photo_path")) ? null : reader.GetString("photo_path")
                        );

                        return user;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error validating user: " + ex.Message);
            }
            finally
            {
                reader?.Close();
                CloseConnection();
            }

            return null; // Invalid credentials
        }
        // Add User
        public bool AddUser(Person person)
        {
            try
            {
                OpenConnection();
                string sql = @"
                INSERT INTO person (username, password, first_name, last_name, email, date_of_birth, gender, mobile, weight, height, photo_path)
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
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding user: " + ex.Message);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
        public Dictionary<int, double> GetCalculationFactors(int activityId)
        {
            Dictionary<int, double> calculationFactors = new Dictionary<int, double>();

            try
            {
                OpenConnection();
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
            finally 
            {
                CloseConnection(); 
            }

            return calculationFactors;
        }
        public int InsertRecords(double burnedCalories, int activityId, string intensityLevel)
        {
            int recordId = -1; // Default to -1 to indicate failure

            try
            {
                OpenConnection();
                // Step 1: Insert into the `record` table with intensity_level
                sql = @"
                        INSERT INTO record (record_date, burned_calories, activity_id, person_id, intensity_level)
                        VALUES (@recordDate, @burnedCalories, @activityId, @personID, @intensityLevel);
                        SELECT LAST_INSERT_ID();";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@recordDate", DateTime.Now.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@burnedCalories", burnedCalories);
                cmd.Parameters.AddWithValue("@activityId", activityId);
                cmd.Parameters.AddWithValue("@personID", frmLogin.user.PersonID);
                cmd.Parameters.AddWithValue("@intensityLevel", intensityLevel);

                // Execute the query and get the last inserted record ID
                recordId = Convert.ToInt32(cmd.ExecuteScalar());

                if (recordId > 0)
                {
                    // Step 2: Insert into the `user_record` table
                    string userRecordSql = @"INSERT INTO user_record (person_id, record_id) VALUES (@personID, @recordID);";

                    MySqlCommand userRecordCmd = new MySqlCommand(userRecordSql, conn);
                    userRecordCmd.Parameters.AddWithValue("@personID", frmLogin.user.PersonID);
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
            finally
            { 
                CloseConnection();
            }
            return recordId;
        }
        public bool InsertMetricValues(int activityId, int recordId, Dictionary<int, double> metrics)
        {
            bool isSuccess = true;

            try
            {
                OpenConnection(); // connection is open

                foreach (var metric in metrics)
                {
                    sql = @"
                            INSERT INTO metric_values (activity_id, record_id, metric_id, value)
                            VALUES (@activityId, @recordId, @metricId, @value)";

                    cmd = new MySqlCommand(sql, conn);
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
            finally
            {
                CloseConnection(); // connection is closed
            }
            return isSuccess;
        }
        public int InsertSchedule(int personID, DateTime scheduledDate)
        {
            try
            {
                OpenConnection(); // connection is open

                sql = @"
                    INSERT INTO schedule (person_id, scheduled_date)
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
            finally
            {
                CloseConnection(); // connection is closed
            }
        }
        public bool InsertScheduleActivity(int scheduleId, int activityId, TimeSpan startTime, int durationMinutes)
        {
            try
            {
                OpenConnection(); // connection is open

                sql = @"
                        INSERT INTO schedule_activity (schedule_id, activity_id, start_time, duration_minutes)
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
            finally
            {
                CloseConnection(); // connection is closed
            }
        }
        public DataTable GetSchedules(int personID)
        {
            DataTable dt = new DataTable();

            try
            {
                OpenConnection(); // Ensure the connection is open

                sql = @"
                        SELECT 
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
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving schedules: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // Ensure the connection is closed
            }

            return dt;
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
                // connection is closed
                CloseConnection();
            }
        }
        public DataTable GetRecords(int personID)
        {
            DataTable dt = new DataTable();

            try
            {
                OpenConnection(); // the connection is open

                sql = @"
                    SELECT 
                        r.record_id AS 'Record Id',
                        a.activity_name AS 'Activity',
                        r.record_date AS 'Record Date',
                        r.burned_calories AS 'Burned Calories',
                        r.intensity_level AS 'Activity Type' 
                    FROM record r
                    JOIN activity a ON r.activity_id = a.activity_id
                    WHERE r.person_id = @personId
                    ORDER BY r.record_date DESC";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personId", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving records: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return dt;
        }
        public bool DeleteRecord(int recordId)
        {
            try
            {
                OpenConnection();

                // Step 1: Delete from metric_values table first 
                sql = "DELETE FROM metric_values WHERE record_id = @recordId";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@recordId", recordId);
                cmd.ExecuteNonQuery();  // Execute deletion

                // Step 2: Delete from user_record table
                sql = "DELETE FROM user_record WHERE record_id = @recordId";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@recordId", recordId);
                cmd.ExecuteNonQuery();

                // Step 3: Finally, delete from record table
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
                OpenConnection(); // connection is open

                sql = @"
                    SELECT m.metric_name, mv.value
                    FROM metric_values mv
                    JOIN metric m ON mv.metric_id = m.metric_id
                    WHERE mv.record_id = @recordId";

                cmd = new MySqlCommand(sql, conn);
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

                if (activityDetails.Length > 0)
                {
                    activityDetails.Length -= 2;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving activity details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return activityDetails.ToString();
        }
        public double GetMetValue(int activityId, string intensity)
        {
            double metValue = 0;

            try
            {
                OpenConnection(); // connection is open

                sql = "SELECT met_value FROM met_values WHERE activity_id = @activityId AND intensity_level = @intensity";
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
            finally
            {
                CloseConnection(); // connection is closed
            }

            return metValue;
        }
        public bool UpdateGoal(int goalId, string goalType, double targetWeight, double dailyCaloriesTarget, DateTime targetDate, int activityId)
        {
            bool isSuccess = false;

            try
            {
                OpenConnection(); // connection is open

                sql = @"
                        UPDATE goal_tracking 
                        SET goal_type = @goalType, 
                            target_weight = @targetWeight, 
                            daily_calories_target = @dailyCaloriesTarget, 
                            target_date = @targetDate, 
                            activity_id = @activityId 
                        WHERE goal_id = @goalId";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@goalId", goalId);
                cmd.Parameters.AddWithValue("@goalType", goalType);
                cmd.Parameters.AddWithValue("@targetWeight", targetWeight);
                cmd.Parameters.AddWithValue("@dailyCaloriesTarget", dailyCaloriesTarget);
                cmd.Parameters.AddWithValue("@targetDate", targetDate.Date);
                cmd.Parameters.AddWithValue("@activityId", activityId);

                isSuccess = cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return isSuccess;
        }
        public bool DeleteGoal(int goalId)
        {
            bool isSuccess = false;

            try
            {
                OpenConnection(); // the connection is open

                sql = "DELETE FROM goal_tracking WHERE goal_id = @goalId";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@goalId", goalId);

                isSuccess = cmd.ExecuteNonQuery() > 0; // Returns true if the delete operation is successful
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return isSuccess;
        }
        public bool MarkGoalAsAchieved(int goalId)
        {
            bool isSuccess = false;

            try
            {
                OpenConnection(); // connection is open

                sql = @"
                UPDATE goal_tracking
                SET is_achieved = 1, achieved_date = NOW()
                WHERE goal_id = @goalID;";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@goalID", goalId);

                isSuccess = cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error marking goal as achieved: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); //  connection is closed
            }

            return isSuccess;
        }
        public DataTable GetGoals(int personID)
        {
            DataTable dt = new DataTable();

            try
            {
                OpenConnection(); //  connection is open

                sql = @"SELECT 
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
            finally
            {
                CloseConnection(); // connection is closed
            }

            return dt;
        }
        public int GetAchievedGoalsCountByType(int personID, string goalType)
        {
            int achievedGoalsCount = 0;

            try
            {
                OpenConnection(); //  connection is open

                string sql = "SELECT COUNT(*) FROM goal_tracking WHERE person_id = @personID AND is_achieved = 1 AND goal_type = @goalType";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personID", personID);
                cmd.Parameters.AddWithValue("@goalType", goalType);

                achievedGoalsCount = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving achieved goals count for {goalType}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return achievedGoalsCount;
        }
        public bool InsertGoal(int personID, string goalType, double targetWeight, double dailyCaloriesTarget, DateTime targetDate, int activityId)
        {
            bool isSuccess = false;

            try
            {
                OpenConnection(); // Open the connection

                sql = @"
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

                isSuccess = cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting goal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return isSuccess;
        }
        public bool UpdateUserWeight(int personID, double newWeight)
        {
            bool isUpdated = false;

            try
            {
                OpenConnection(); // Open the connection

                sql = "UPDATE person SET weight = @newWeight WHERE person_id = @personID";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@newWeight", newWeight);
                cmd.Parameters.AddWithValue("@personID", personID);

                isUpdated = cmd.ExecuteNonQuery() > 0; // Return true if rows were updated
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating weight: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return isUpdated;
        }
        public int GetActivityIdByName(string activityName)
        {
            int activityId = -1;

            try
            {
                OpenConnection(); // Open the connection

                sql = "SELECT activity_id FROM activity WHERE activity_name = @activityName";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@activityName", activityName);

                object result = cmd.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int id))
                {
                    activityId = id;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving activity ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return activityId;
        }
        public DataTable GetGoalAchievementByMonth(int personID)
        {
            DataTable dt = new DataTable();
            try
            {
                OpenConnection(); // Open the database connection

                string sql = @"
                    SELECT 
                        DATE_FORMAT(achieved_date, '%Y-%m') AS achievement_month, 
                        COUNT(*) AS achieved_count
                    FROM 
                        goal_tracking
                    WHERE 
                        is_achieved = 1 AND person_id = @personID
                    GROUP BY 
                        DATE_FORMAT(achieved_date, '%Y-%m')
                    ORDER BY 
                        achievement_month ASC";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@personID", personID);
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching goal achievement by month: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return dt;
        }

        public DataTable GetAchievedAndPendingGoals(int personID)
        {
            DataTable dt = new DataTable();
            try
            {
                OpenConnection(); // Open the database connection

                sql = @"
                    SELECT 
                        COALESCE(SUM(CASE WHEN is_achieved = 1 THEN 1 ELSE 0 END), 0) AS achieved_count,
                        COALESCE(SUM(CASE WHEN is_achieved = 0 THEN 1 ELSE 0 END), 0) AS pending_count
                    FROM goal_tracking
                    WHERE person_id = @personID";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@personID", personID);
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching achieved and pending goals: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return dt;
        }

        public bool InsertWeightTracking(int personID, double weight)
        {
            bool isSuccess = false;
            try
            {
                OpenConnection(); // Open the database connection

                sql = @"
                    INSERT INTO weight_tracking (person_id, recorded_date, weight)
                    VALUES (@personID, NOW(), @weight);";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personID", personID);
                cmd.Parameters.AddWithValue("@weight", weight);

                isSuccess = cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting weight tracking: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return isSuccess;
        }
        public DataTable GetWeightTrackingData(int personID)
        {
            DataTable dt = new DataTable();
            try
            {
                OpenConnection(); // Open the database connection

                sql = @"
                        SELECT 
                            DATE_FORMAT(recorded_date, '%Y-%m-%d') AS recorded_date,
                            weight
                        FROM 
                            weight_tracking
                        WHERE 
                            person_id = @personID
                        ORDER BY 
                            recorded_date ASC";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personID", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching weight tracking data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return dt;
        }
        public DataTable GetRecordsForDateRange(int personID, DateTime startDate, DateTime endDate)
        {
            DataTable dt = new DataTable();
            try
            {
                OpenConnection(); // Open the database connection

                sql = @"
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

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personID", personID);
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving records for date range: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return dt;
        }
        public DataTable GetRecordsByActivity(int personID, string activityName)
        {
            DataTable dt = new DataTable();
            try
            {
                OpenConnection(); // Open the database connection

                sql = @"
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

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personID", personID);
                cmd.Parameters.AddWithValue("@activityName", activityName);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving records by activity: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return dt;
        }
        public DataTable GetDailyCaloriesData(int personID, DateTime startDate, DateTime endDate)
        {
            DataTable dt = new DataTable();
            try
            {
                OpenConnection(); // Open the database connection

                sql = @"
                    SELECT 
                        DATE(record_date) AS record_date, 
                        SUM(burned_calories) AS total_calories
                    FROM record
                    WHERE person_id = @personID AND record_date BETWEEN @startDate AND @endDate
                    GROUP BY DATE(record_date)
                    ORDER BY record_date ASC";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personID", personID);
                cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"));

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving daily calories data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); //connection is closed
            }

            return dt;
        }
        public DataTable GetSchedulesSummary(int personID)
        {
            DataTable dt = new DataTable();
            try
            {
                OpenConnection(); // Open the database connection

                sql = @"
                    SELECT 
                        a.activity_name AS Activity,
                        COUNT(s.schedule_id) AS TotalSchedules
                    FROM schedule s
                    JOIN schedule_activity sa ON s.schedule_id = sa.schedule_id
                    JOIN activity a ON sa.activity_id = a.activity_id
                    WHERE s.person_id = @personID
                    GROUP BY a.activity_name
                    ORDER BY TotalSchedules DESC";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personID", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving schedules summary: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return dt;
        }
        public int GetScheduleCountForDate(int personID, DateTime date)
        {
            int count = 0;
            try
            {
                OpenConnection(); // Open the database connection

                sql = @"
                    SELECT COUNT(*) 
                    FROM schedule s
                    JOIN schedule_activity sa ON s.schedule_id = sa.schedule_id
                    WHERE s.person_id = @personId AND s.scheduled_date = @scheduleDate";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personId", personID);
                cmd.Parameters.AddWithValue("@scheduleDate", date.Date);

                object result = cmd.ExecuteScalar();
                count = result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving schedule count: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return count;
        }
        public DataTable GetFilteredSchedules(int personID, DateTime? startDate, DateTime? endDate)
        {
            DataTable dt = new DataTable();
            try
            {
                OpenConnection(); // Open the database connection

                sql = @"
                        SELECT 
                            s.schedule_id AS 'Schedule Id',
                            a.activity_name AS 'Activity',
                            s.scheduled_date AS 'Date',
                            sa.start_time AS 'Start Time',
                            sa.duration_minutes AS 'Duration'
                        FROM schedule s
                        JOIN schedule_activity sa ON s.schedule_id = sa.schedule_id
                        JOIN activity a ON sa.activity_id = a.activity_id
                        WHERE s.person_id = @personID";

                if (startDate.HasValue && endDate.HasValue)
                {
                    sql += " AND DATE(s.scheduled_date) BETWEEN @startDate AND @endDate";
                }

                sql += " ORDER BY s.scheduled_date ASC";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personID", personID);

                if (startDate.HasValue && endDate.HasValue)
                {
                    cmd.Parameters.AddWithValue("@startDate", startDate.Value.Date);
                    cmd.Parameters.AddWithValue("@endDate", endDate.Value.Date);
                }

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving filtered schedules: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return dt;
        }
        public bool IsOverlappingSchedule(int personId, DateTime date, TimeSpan startTime, int durationMinutes)
        {
            bool isOverlapping = false;

            try
            {
                OpenConnection(); // Open the database connection

                sql = @"
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
                cmd.Parameters.AddWithValue("@startTime", startTime.ToString(@"hh\:mm\:ss"));
                cmd.Parameters.AddWithValue("@durationMinutes", durationMinutes);

                isOverlapping = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking overlapping schedule: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return isOverlapping;
        }
        public DataTable GetScheduleDistributionByTime(int personId)
        {
            DataTable dt = new DataTable();

            try
            {
                OpenConnection(); // Open the database connection

                sql = @"
                    SELECT 
                        HOUR(sa.start_time) AS Hour, 
                        COUNT(*) AS Count
                    FROM schedule s
                    JOIN schedule_activity sa ON s.schedule_id = sa.schedule_id
                    WHERE s.person_id = @personId
                    GROUP BY Hour
                    ORDER BY Hour";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personId", personId);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving schedule distribution: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return dt;
        }
        public DataTable GetActivityGraphData(int personID, int activityID)
        {
            DataTable dt = new DataTable();

            try
            {
                OpenConnection(); // Open the database connection

                sql = @"
                    SELECT 
                        DATE(r.record_date) AS Date, 
                        SUM(r.burned_calories) AS CaloriesBurned
                    FROM record r
                    JOIN activity a ON r.activity_id = a.activity_id
                    WHERE r.person_id = @personId AND r.activity_id = @activityId
                    GROUP BY DATE(r.record_date)
                    ORDER BY DATE(r.record_date)";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personId", personID);
                cmd.Parameters.AddWithValue("@activityId", activityID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving activity graph data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return dt;
        }
        public DataTable GetActivityMetricsOverTime(int personId, int activityId)
        {
            try
            {
                OpenConnection();

                sql = @"
                    SELECT r.record_date AS Date, 
                           mv.value AS Value, 
                           m.metric_name AS MetricName
                    FROM record r
                    INNER JOIN metric_values mv ON r.record_id = mv.record_id
                    INNER JOIN metric m ON mv.metric_id = m.metric_id
                    WHERE r.person_id = @personId AND r.activity_id = @activityId
                    ORDER BY r.record_date;";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personId", personId);
                cmd.Parameters.AddWithValue("@activityId", activityId);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching activity metrics: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }
        public DataTable GetActivityMetrics(int personId, int activityId)
        {
            try
            {
                OpenConnection();

                sql = @"
                    SELECT 
                        m.metric_name,
                        mv.value
                    FROM metric_values mv
                    INNER JOIN metric m ON mv.metric_id = m.metric_id
                    INNER JOIN record r ON mv.record_id = r.record_id
                    WHERE r.person_id = @personId AND r.activity_id = @activityId;";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personId", personId);
                cmd.Parameters.AddWithValue("@activityId", activityId);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching activity metrics: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }
        public double GetMaxCaloriesForActivity(int personID, int activityID)
        {
            double maxCalories = 0.0;

            try
            {
                OpenConnection(); // Open the database connection

                sql = @"
                    SELECT MAX(r.burned_calories) AS MaxCalories
                    FROM record r
                    WHERE r.person_id = @personId
                      AND r.activity_id = @activityId;";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personId", personID);
                cmd.Parameters.AddWithValue("@activityId", activityID);

                object result = cmd.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    maxCalories = Convert.ToDouble(result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving maximum calories for activity ID {activityID}: {ex.Message}");
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return maxCalories;
        }
        public DataTable GetHistoricalComparison(int personID)
        {
            DataTable dt = new DataTable();

            try
            {
                OpenConnection(); // Open the database connection

                sql = @"
                SELECT 
                    a.activity_name AS ActivityName,
                    SUM(r.burned_calories) AS CaloriesBurned
                FROM record r
                JOIN activity a ON r.activity_id = a.activity_id
                WHERE r.person_id = @personId
                GROUP BY a.activity_id, a.activity_name
                ORDER BY CaloriesBurned DESC;";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personId", personID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving historical comparison data: {ex.Message}");
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return dt;
        }
        public DataTable GetUpcomingActivitySchedules(int personID, int activityID)
        {
            DataTable dt = new DataTable();

            try
            {
                OpenConnection(); // Open the database connection

                sql = @"
                    SELECT 
                        s.scheduled_date AS Date, 
                        sa.start_time AS StartTime, 
                        sa.duration_minutes AS Duration
                    FROM schedule s
                    JOIN schedule_activity sa ON s.schedule_id = sa.schedule_id
                    WHERE 
                        s.person_id = @personId 
                        AND sa.activity_id = @activityId 
                        AND s.scheduled_date >= CURDATE()
                    ORDER BY 
                        s.scheduled_date ASC, 
                        sa.start_time ASC;";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personId", personID);
                cmd.Parameters.AddWithValue("@activityId", activityID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving upcoming activity schedules: {ex.Message}");
            }
            finally
            {
                CloseConnection(); // connection is closed
            }

            return dt;
        }
        public bool UpdateUserInfo(Person person)
        {
            try
            {
                OpenConnection();
                sql = @"
                    UPDATE person 
                    SET first_name = @firstname, 
                        last_name = @lastname, 
                        email = @email, 
                        date_of_birth = @dob, 
                        gender = @gender, 
                        mobile = @mobile, 
                        weight = @weight, 
                        height = @height, 
                        photo_path = @photo
                    WHERE person_id = @personId";

                cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@firstname", person.Firstname ?? string.Empty);
                cmd.Parameters.AddWithValue("@lastname", person.Lastname ?? string.Empty);
                cmd.Parameters.AddWithValue("@email", person.Email ?? string.Empty);
                cmd.Parameters.AddWithValue("@dob", person.DateOfBirth);
                cmd.Parameters.AddWithValue("@gender", person.Gender ?? string.Empty);
                cmd.Parameters.AddWithValue("@mobile", person.Mobile ?? string.Empty);

                // Handle weight
                if (person.Weight > 0)
                {
                    cmd.Parameters.AddWithValue("@weight", person.Weight);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@weight", DBNull.Value);
                }

                // Handle height
                if (person.Height > 0)
                {
                    cmd.Parameters.AddWithValue("@height", person.Height);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@height", DBNull.Value);
                }

                cmd.Parameters.AddWithValue("@photo", person.PhotoPath ?? string.Empty);
                cmd.Parameters.AddWithValue("@personId", person.PersonID);

                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user info: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        public bool UpdatePhotoPath(int personId, string photoPath)
        {
            try
            {
                OpenConnection();
                sql = "UPDATE person SET photo_path = @photoPath WHERE person_id = @personId";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@photoPath", photoPath);
                cmd.Parameters.AddWithValue("@personId", personId);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating photo path: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
        // for setting
        public bool IsUserExists(string username, string email, int personID)
        {
            try
            {
                OpenConnection();
                sql = "SELECT COUNT(*) FROM person WHERE (username = @username OR email = @email) AND person_id <> @personID";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@personID", personID);

                int userExists = Convert.ToInt32(cmd.ExecuteScalar());
                return userExists > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking user existence: " + ex.Message);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool UpdatePassword(int personId, string newPassword)
        {
            try
            {
                OpenConnection();
                sql = "UPDATE person SET password = @password WHERE person_id = @personId";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@password", newPassword);
                cmd.Parameters.AddWithValue("@personId", personId);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating password: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
        public List<Record> GetAllRecordsForUser(int personId)
        {
            var records = new List<Record>();

            try
            {
                OpenConnection();

                sql = "SELECT * FROM record WHERE person_id = @personId";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personId", personId);

                using (reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // retrieve data
                        double burnedCalories = reader["burned_calories"] != DBNull.Value
                            ? reader.GetDouble("burned_calories")
                            : 0;

                        DateTime recordDate = reader["record_date"] != DBNull.Value
                            ? reader.GetDateTime("record_date")
                            : DateTime.MinValue;

                        string intensityLevel = reader["intensity_level"] != DBNull.Value
                            ? reader.GetString("intensity_level")
                            : "Unknown";

                        // Create Person object
                        var person = User.GetInstance();

                        // Create Activity object
                        var activity = new Activity
                        {
                            ActivityId = reader["activity_id"] != DBNull.Value ? reader.GetInt32("activity_id") : 0
                        };

                        // Create Record object
                        var record = new Record
                        {
                            RecordId = reader.GetInt32("record_id"),
                            Person = person,
                            Activity = activity,
                            RecordDate = recordDate,
                            BurnedCalories = burnedCalories,
                            IntensityLevel = intensityLevel
                        };

                        records.Add(record);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving records: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection();
            }

            return records;
        }

        // Method to fetch the last activity
        public Record GetLastActivityForUser(int personId)
        {
            Record lastActivity = null;
            try
            {
                OpenConnection();
                sql = @"
                    SELECT r.*, a.activity_name, p.first_name, p.last_name 
                    FROM record r 
                    JOIN activity a ON r.activity_id = a.activity_id 
                    JOIN person p ON r.person_id = p.person_id 
                    WHERE r.person_id = @personId 
                    ORDER BY r.record_date DESC LIMIT 1";
                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personId", personId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Create Person object
                        User person = User.GetInstance();
                        person.PersonID = reader.GetInt32("person_id");
                        person.Firstname = reader.GetString("first_name");
                        person.Lastname = reader.GetString("last_name");

                        // Create Activity object
                        Activity activity = new Activity
                        {
                            ActivityId = reader.GetInt32("activity_id"),
                            ActivityName = reader.GetString("activity_name")
                        };

                        // Create Record object
                        lastActivity = new Record
                        {
                            RecordId = reader.GetInt32("record_id"),
                            Person = person,
                            Activity = activity,
                            RecordDate = reader.GetDateTime("record_date"),
                            BurnedCalories = reader.GetDouble("burned_calories"),
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving last activity: {ex.Message}");
            }
            finally
            {
                CloseConnection();
            }
            return lastActivity;
        }
        public GoalTracking GetNextUpcomingGoalForUser(int personId)
        {
            try
            {
                OpenConnection();

                sql = @"
            SELECT gt.goal_id, gt.goal_type, gt.target_weight, gt.daily_calories_target, 
                   gt.is_achieved, gt.created_at, gt.target_date, gt.achieved_date, 
                   a.activity_id, a.activity_name
            FROM goal_tracking gt
            LEFT JOIN activity a ON gt.activity_id = a.activity_id
            WHERE gt.person_id = @personId AND gt.is_achieved = 0 AND gt.target_date > CURDATE()
            ORDER BY gt.target_date ASC
            LIMIT 1;";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personId", personId);

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Create Person object 
                    Person person = User.GetInstance();

                    // Create Activity object 
                    Activity activity = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("activity_id")))
                    {
                        activity = new Activity
                        {
                            ActivityId = reader.GetInt32("activity_id"),
                            ActivityName = reader.GetString("activity_name")
                        };
                    }

                    // Create and return GoalTracking object
                    return new GoalTracking
                    {
                        GoalId = reader.GetInt32("goal_id"),
                        Person = person,
                        GoalType = reader.GetString("goal_type"),
                        TargetWeight = reader.GetDecimal("target_weight"),
                        DailyCaloriesTarget = reader.GetDouble("daily_calories_target"),
                        IsAchieved = reader.GetBoolean("is_achieved"),
                        CreatedAt = reader.GetDateTime("created_at"),
                        TargetDate = reader.GetDateTime("target_date"),
                        Activity = activity,
                        AchievedDate = !reader.IsDBNull(reader.GetOrdinal("achieved_date"))
                            ? reader.GetDateTime("achieved_date")
                            : default(DateTime)
                    };
                }

                return null; // No upcoming goal found
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving upcoming goal: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                reader?.Close();
                CloseConnection();
            }
        }
        public Schedule GetNextScheduleForUser(int personId)
        {
            try
            {
                OpenConnection();

                sql = @"
            SELECT schedule_id, scheduled_date 
            FROM schedule
            WHERE person_id = @person_id AND scheduled_date >= CURDATE()
            ORDER BY scheduled_date ASC
            LIMIT 1";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@person_id", personId);

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Schedule
                    {
                        ScheduleId = reader.GetInt32("schedule_id"),
                        ScheduledDate = reader.GetDateTime("scheduled_date"),
                        Person = User.GetInstance()
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving schedule: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection();
            }

            return null;
        }
        public ScheduleActivity GetFirstActivityForSchedule(int scheduleId)
        {
            try
            {
                OpenConnection();

                sql = @"
            SELECT sa.activity_id, sa.start_time, sa.duration_minutes, a.activity_name
            FROM schedule_activity sa
            INNER JOIN activity a ON sa.activity_id = a.activity_id
            WHERE sa.schedule_id = @schedule_id
            ORDER BY sa.start_time ASC
            LIMIT 1";

                cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@schedule_id", scheduleId);

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new ScheduleActivity
                    {
                        Schedule = new Schedule { ScheduleId = scheduleId },
                        Activity = new Activity
                        {
                            ActivityId = reader.GetInt32("activity_id"),
                            ActivityName = reader.GetString("activity_name")
                        },
                        StartTime = reader.GetTimeSpan("start_time"),
                        DurationMinutes = reader.GetInt32("duration_minutes")
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving schedule activity: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection();
            }

            return null;
        }
        public List<Record> GetUserActivityRecords(int personId)
        {
            List<Record> records = new List<Record>();

            try
            {
                OpenConnection();

                sql = @"SELECT r.record_id, r.record_date, r.burned_calories, r.intensity_level,
                       a.activity_id, a.activity_name,
                       p.person_id, p.first_name, p.last_name, p.username
                FROM record r
                INNER JOIN activity a ON r.activity_id = a.activity_id
                INNER JOIN person p ON r.person_id = p.person_id
                WHERE r.person_id = @personId
                ORDER BY r.record_date DESC";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personId", personId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Create Person object
                        var person = User.GetInstance();

                        // Create Activity object
                        Activity activity = new Activity
                        {
                            ActivityId = reader.GetInt32("activity_id"),
                            ActivityName = reader.IsDBNull(reader.GetOrdinal("activity_name"))
                                ? "Unknown Activity"
                                : reader.GetString("activity_name")
                        };

                        // Create Record object
                        Record record = new Record
                        {
                            RecordId = reader.GetInt32("record_id"),
                            Person = person,
                            Activity = activity,
                            RecordDate = reader.IsDBNull(reader.GetOrdinal("record_date"))
                                ? DateTime.MinValue
                                : reader.GetDateTime("record_date"),
                            BurnedCalories = reader.IsDBNull(reader.GetOrdinal("burned_calories"))
                                ? 0
                                : reader.GetDouble("burned_calories"),
                            IntensityLevel = reader.IsDBNull(reader.GetOrdinal("intensity_level"))
                                ? "Unknown"
                                : reader.GetString("intensity_level")
                        };

                        records.Add(record);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving records: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection();
            }

            return records;
        }
        public List<GoalTracking> GetGoalsForUser(int personId)
        {
            List<GoalTracking> goals = new List<GoalTracking>();

            try
            {
                OpenConnection();

                sql = @"SELECT g.goal_id, g.goal_type, g.target_weight, g.daily_calories_target, g.is_achieved,
                                g.created_at, g.target_date, g.achieved_date,
                                a.activity_id, a.activity_name,
                                p.person_id, p.first_name, p.last_name, p.username
                         FROM goal_tracking g
                         INNER JOIN person p ON g.person_id = p.person_id
                         LEFT JOIN activity a ON g.activity_id = a.activity_id
                         WHERE g.person_id = @personId";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personId", personId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Create Person object
                        var person = User.GetInstance();


                        // Create Activity object
                        Activity activity = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("activity_id")))
                        {
                            activity = new Activity
                            {
                                ActivityId = reader.GetInt32("activity_id"),
                                ActivityName = reader.GetString("activity_name")
                            };
                        }

                        GoalTracking goal = new GoalTracking
                        {
                            GoalId = reader.GetInt32("goal_id"),
                            Person = person,
                            GoalType = reader.GetString("goal_type"),
                            TargetWeight = reader.GetDecimal("target_weight"),
                            DailyCaloriesTarget = reader.GetDouble("daily_calories_target"),
                            IsAchieved = reader.GetBoolean("is_achieved"),
                            CreatedAt = reader.GetDateTime("created_at"),
                            TargetDate = reader.GetDateTime("target_date"),
                            AchievedDate = reader.IsDBNull(reader.GetOrdinal("achieved_date"))
                            ? DateTime.MinValue // Default value if NULL
                            : reader.GetDateTime("achieved_date"),
                            Activity = activity
                        };

                        goals.Add(goal);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving goals: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection();
            }

            return goals;
        }
        public double GetTotalBurnedCalories(int personId)
        {
            double totalBurnedCalories = 0;

            try
            {
                OpenConnection();

                sql = @"SELECT SUM(burned_calories) AS TotalCalories
                       FROM record
                       WHERE person_id = @personId";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@personId", personId);

                var result = cmd.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                {
                    totalBurnedCalories = Convert.ToDouble(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching total burned calories: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection();
            }

            return totalBurnedCalories;
        }
    }
}

