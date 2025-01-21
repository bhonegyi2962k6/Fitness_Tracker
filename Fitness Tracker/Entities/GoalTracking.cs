using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_Tracker.Entities
{
    public class GoalTracking
    {
        private int goalId;
        private Person person;
        private string goalType;
        private decimal targetWeight;
        private double dailyCaloriesTarget;
        private bool isAchieved;
        private DateTime createdAt;
        private DateTime targetDate;
        private Activity activity;
        private DateTime achievedDate;

        public GoalTracking() { }

        public GoalTracking(int goalId, Person person, string goalType, decimal targetWeight, double dailyCaloriesTarget, bool isAchieved, DateTime createdAt, DateTime targetDate, Activity activity, DateTime achievedDate)
        {
            this.goalId = goalId;
            this.person = person;
            this.goalType = goalType;
            this.targetWeight = targetWeight;
            this.dailyCaloriesTarget = dailyCaloriesTarget;
            this.isAchieved = isAchieved;
            this.createdAt = createdAt;
            this.targetDate = targetDate;
            this.activity = activity;
            this.achievedDate = achievedDate;
        }

        public int GoalId { get => goalId; set => goalId = value; }
        public Person Person { get => person; set => person = value; }
        public string GoalType { get => goalType; set => goalType = value; }
        public decimal TargetWeight { get => targetWeight; set => targetWeight = value; }
        public double DailyCaloriesTarget { get => dailyCaloriesTarget; set => dailyCaloriesTarget = value; }
        public bool IsAchieved { get => isAchieved; set => isAchieved = value; }
        public DateTime CreatedAt { get => createdAt; set => createdAt = value; }
        public DateTime TargetDate { get => targetDate; set => targetDate = value; }
        public Activity Activity { get => activity; set => activity = value; }
        public DateTime AchievedDate { get => achievedDate; set => achievedDate = value; }
    }
}
