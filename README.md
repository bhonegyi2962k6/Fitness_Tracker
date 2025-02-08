# 🏋️ Fitness Tracker - NCC Level 4 DDOOCP

## 📌 Overview
The **Fitness Tracker** is a software application designed to help users track their fitness activities, set personal fitness goals, and monitor progress. This project is developed as part of the **NCC Level 4 - Designing and Developing Object-Oriented Computer Programs (DDOOCP) assignment**.

## 🎯 Features
### ✅ **User Registration & Authentication**
- Users can register with a **username** and a **secure password** (12 characters, at least one uppercase and one lowercase letter).
- Login system with error handling for **failed attempts**.

### 🔥 **Activity Tracking**
- Users can record **six different activities**:
  - Walking
  - Swimming
  - Cycling
  - Hiking
  - Weightlifting
  - Rowing
- Each activity tracks **three predefined metrics** (e.g., steps, distance, time for walking).

### 📊 **Goal Setting & Progress Monitoring**
- Users can set a **calorie-burning goal**.
- The system calculates **calories burned** using predefined metrics.
- The program determines **if the user achieved their goal**.

### 🎨 **Graphical User Interface (GUI)**
- Intuitive **C# Windows Forms application**.
- Consistent **layout with clear labels and error handling**.

## 📂 **Project Structure**
```
/Fitness_Tracker
│── /Entities         # Class files for core objects (User, Activity, GoalTracking, Metric)
│── /dao              # Database connection and handling
│── /Resources        # UI assets
│── ConnectionDB.cs   # Handles MySQL or SQL Server database connection
│── README.md         # Project documentation
│── Fitness_Tracker.sln  # Visual Studio Solution file
│── fitness_tracker.sql  # SQL script for database setup
│── MainForm.cs       # Entry point for GUI
```

## 🛠 **Technologies Used**
- **C# (.NET Framework)**
- **MySQL (or SQL Server)**
- **Windows Forms (WinForms)**
- **Object-Oriented Programming (OOP)** principles

## ⚙ **Installation & Setup**
### 1️⃣ **Database Setup**
- Open **MySQL Workbench** or **SQL Server Management Studio (SSMS)**.
- Run the SQL script:
  ```sh
  source path/to/fitness_tracker.sql;
  ```
- Ensure that the **ConnectionDB.cs** file has the correct database credentials.

### 2️⃣ **Run the Application**
- Open **Visual Studio Community**.
- Load `Fitness_Tracker.sln`.
- Build and run the project.

## 🔍 **Testing**
A **test plan** was developed to validate:
- User login and registration.
- Activity recording functionality.
- Calorie calculation based on **user-input metrics**.
- Goal tracking and **achievement reporting**.

## 🎯 **Assignment Requirements & Justification**
This project fulfills the **NCC Level(4) DDOOCP functional and non-functional requirements**, including:
- **Object-Oriented Design (OOP)**
- **Encapsulation & Code Readability**
- **Validation & Exception Handling**
- **SQL Database Integration**
- **Graphical User Interface (GUI)**

## 🏆 **Author**
- **[Bhone Naing Khant]**
- **GitHub Repository:** [Fitness_Tracker](https://github.com/bhonegyi2962k6/Fitness_Tracker.git)

---
