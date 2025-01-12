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

        public GoalTracking() { }

        public GoalTracking(int goalId, Person person, string goalType, decimal targetWeight, double dailyCaloriesTarget, bool isAchieved, DateTime createdAt, DateTime targetDate)
        {
            this.goalId = goalId;
            this.person = person;
            this.goalType = goalType;
            this.targetWeight = targetWeight;
            this.dailyCaloriesTarget = dailyCaloriesTarget;
            this.isAchieved = isAchieved;
            this.createdAt = createdAt;
            this.targetDate = targetDate;
        }

        public int GoalId { get => goalId; set => goalId = value; }
        public Person Person { get => person; set => person = value; }
        public string GoalType { get => goalType; set => goalType = value; }
        public decimal TargetWeight { get => targetWeight; set => targetWeight = value; }
        public double DailyCaloriesTarget { get => dailyCaloriesTarget; set => dailyCaloriesTarget = value; }
        public bool IsAchieved { get => isAchieved; set => isAchieved = value; }
        public DateTime CreatedAt { get => createdAt; set => createdAt = value; }
        public DateTime TargetDate { get => targetDate; set => targetDate = value; }
    }
}
