using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_Tracker.Entities
{
    public class ScheduleActivity
    {
        private Schedule schedule; // Association with Schedule
        private Activity activity; // Association with Activity
        private TimeSpan startTime;
        private int durationMinutes;

        public ScheduleActivity() { }
        public ScheduleActivity(Schedule schedule, Activity activity, TimeSpan startTime, int durationMinutes)
        {
            this.schedule = schedule;
            this.activity = activity;
            this.startTime = startTime;
            this.durationMinutes = durationMinutes;
        }

        public Schedule Schedule { get => schedule; set => schedule = value; }
        public Activity Activity { get => activity; set => activity = value; }
        public TimeSpan StartTime { get => startTime; set => startTime = value; }
        public int DurationMinutes { get => durationMinutes; set => durationMinutes = value; }
    }
}
