using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_Tracker.Entities
{
    public class Schedule
    {
        private int scheduleId;
        private Person person;
        private DateTime scheduledDate;

        public Schedule() { }
        public Schedule(int scheduleId, Person person, DateTime scheduledDate)
        {
            this.scheduleId = scheduleId;
            this.person = person;
            this.scheduledDate = scheduledDate;
        }

        public int ScheduleId { get => scheduleId; set => scheduleId = value; }
        public Person Person { get => person; set => person = value; }
        public DateTime ScheduledDate { get => scheduledDate; set => scheduledDate = value; }
    }
}
