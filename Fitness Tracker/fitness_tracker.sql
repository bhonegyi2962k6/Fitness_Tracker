-- Create database if it does not exist
CREATE DATABASE IF NOT EXISTS fitness_tracker;
USE fitness_tracker;

-- Create `person` table
CREATE TABLE IF NOT EXISTS person (
    person_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(255) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    first_name VARCHAR(255),
    last_name VARCHAR(255),
    email VARCHAR(255) UNIQUE,
    date_of_birth DATE,
    gender VARCHAR(255),
    mobile VARCHAR(255) UNIQUE,
    weight DOUBLE,
    height DOUBLE,
    photo_path VARCHAR(255)
);

-- Create `activity` table
CREATE TABLE IF NOT EXISTS activity (
    activity_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    activity_name VARCHAR(100) UNIQUE NOT NULL,
    description VARCHAR(255) NOT NULL
);

-- Create `metric` table
CREATE TABLE IF NOT EXISTS metric (
    metric_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    activity_id INT NOT NULL,
    metric_name VARCHAR(255) NOT NULL,
    calculation_factor DOUBLE NOT NULL,
    FOREIGN KEY (activity_id) REFERENCES activity(activity_id) ON DELETE CASCADE
);

-- Create `record` table
CREATE TABLE IF NOT EXISTS record (
    record_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    person_id INT NOT NULL,
    activity_id INT NOT NULL,
    record_date DATE NOT NULL,
    burned_calories DOUBLE NOT NULL,
    intensity_level VARCHAR(50), -- New column for intensity
    FOREIGN KEY (person_id) REFERENCES person(person_id) ON DELETE CASCADE,
    FOREIGN KEY (activity_id) REFERENCES activity(activity_id) ON DELETE CASCADE
);

-- Create `user_record` table
CREATE TABLE IF NOT EXISTS user_record (
    person_id INT NOT NULL,
    record_id INT NOT NULL,
    FOREIGN KEY (person_id) REFERENCES person(person_id) ON DELETE CASCADE,
    FOREIGN KEY (record_id) REFERENCES record(record_id) ON DELETE CASCADE
);

-- Create `metric_values` table
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

-- Insert `activity` data if not exists
INSERT IGNORE INTO activity (activity_name, description)
VALUES 
    ('Swimming', 'A water-based physical activity'),
    ('Walking', 'A low-impact cardio exercise'),
    ('Cycling', 'A cardio exercise performed using a bicycle'),
    ('Hiking', 'Walking in nature or on trails'),
    ('Weightlifting', 'A strength-based physical activity involving lifting weights'),
    ('Rowing', 'A full-body workout simulating rowing motion');

-- Insert `metric` data if not exists
INSERT IGNORE INTO metric (activity_id, metric_name, calculation_factor)
VALUES
    (1, 'Laps', 0.1),
    (1, 'Time Taken', 0.02),
    (1, 'Average Heart Rate', 0.05),
    (2, 'Steps', 0.05),
    (2, 'Distance', 0.1),
    (2, 'Time Taken', 0.02),
    (3, 'Speed', 0.2),
    (3, 'Distance', 0.1),
    (3, 'Ride Duration', 0.03),
    (4, 'Elevation Gained', 0.15),
    (4, 'Distance', 0.1),
    (4, 'Time Taken', 0.02),
    (5, 'Weight Lifted', 0.25),
    (5, 'Repetitions', 0.05),
    (5, 'Sets Completed', 0.1),
    (6, 'Total Strokes', 0.1),
    (6, 'Distance', 0.08),
    (6, 'Time Taken', 0.02);

-- Create `goal_tracking` table
CREATE TABLE IF NOT EXISTS goal_tracking (
    goal_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    person_id INT NOT NULL,
    goal_type VARCHAR(255) NOT NULL,
    target_weight DECIMAL(5,2) NOT NULL,
    daily_calories_target DOUBLE NOT NULL,
    is_achieved BOOLEAN DEFAULT FALSE,
    created_at DATE,
    target_date DATE,
    activity_id INT, -- Add activity_id as a foreign key
    achieved_date DATE NULL,
    FOREIGN KEY (person_id) REFERENCES person(person_id) ON DELETE CASCADE,
    FOREIGN KEY (activity_id) REFERENCES activity(activity_id) ON DELETE SET NULL
);

-- Create `schedule` table
CREATE TABLE IF NOT EXISTS schedule (
    schedule_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    person_id INT NOT NULL,
    scheduled_date DATE NOT NULL,
    FOREIGN KEY (person_id) REFERENCES person(person_id) ON DELETE CASCADE
);

-- Create `schedule_activity` table
CREATE TABLE IF NOT EXISTS schedule_activity (
    schedule_id INT NOT NULL,
    activity_id INT NOT NULL,
    start_time TIME NOT NULL,
    duration_minutes INT NOT NULL,
    FOREIGN KEY (schedule_id) REFERENCES schedule(schedule_id) ON DELETE CASCADE,
    FOREIGN KEY (activity_id) REFERENCES activity(activity_id) ON DELETE CASCADE
);

-- Create `met_values` table
CREATE TABLE IF NOT EXISTS met_values (
    met_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    activity_id INT NOT NULL,
    intensity_level VARCHAR(50) NOT NULL, -- Use VARCHAR instead of ENUM
    met_value DOUBLE NOT NULL,
    FOREIGN KEY (activity_id) REFERENCES activity(activity_id) ON DELETE CASCADE
);

-- Insert `met_values` data if not exists
INSERT IGNORE INTO met_values (activity_id, intensity_level, met_value)
VALUES
    (1, 'Light', 6.0),    -- Swimming
    (1, 'Moderate', 8.0),
    (1, 'Vigorous', 9.8),
    (2, 'Light', 3.3),    -- Walking
    (2, 'Moderate', 5.0),
    (2, 'Vigorous', 6.3),
    (3, 'Light', 4.0),    -- Cycling
    (3, 'Moderate', 8.0),
    (3, 'Vigorous', 10.0),
    (4, 'Moderate', 6.0), -- Hiking
    (4, 'Vigorous', 9.0),
    (5, 'Light', 3.0),    -- Weightlifting
    (5, 'Moderate', 4.5),
    (5, 'Vigorous', 6.0),
    (6, 'Light', 3.5),    -- Rowing
    (6, 'Moderate', 6.0),
    (6, 'Vigorous', 8.5);

-- Create `weight_tracking` table
CREATE TABLE IF NOT EXISTS weight_tracking (
    weight_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    person_id INT NOT NULL,
    recorded_date DATE NOT NULL,
    weight DOUBLE NOT NULL,
    FOREIGN KEY (person_id) REFERENCES person(person_id) ON DELETE CASCADE
);
