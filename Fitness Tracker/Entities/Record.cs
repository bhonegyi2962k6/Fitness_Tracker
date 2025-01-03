using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_Tracker.Entities
{
    public class Record
    {
        public int recordId;
        public Person person;
        public Activity activity;
        public DateTime recordDate;
        public double burnedCalories;
        public Record() 
        {

        }
        public Record(int recordId, DateTime recordDate, double burnedCalories, Activity activity, Person person)
        {
            this.recordId = recordId;
            this.recordDate = recordDate;
            this.burnedCalories = burnedCalories;
            this.activity = activity;
            this.person = person;
        }

        public int RecordId { get => recordId; set => recordId = value; }
        public DateTime RecordDate { get => recordDate; set => recordDate = value; }
        public double BurnedCalories { get => burnedCalories; set => burnedCalories = value; }
        public Activity Activity { get => activity; set => activity = value; }
        public Person Person { get => person; set => person = value; }
    }
}
