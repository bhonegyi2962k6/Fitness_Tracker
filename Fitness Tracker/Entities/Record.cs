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
        private int recordId;
        private Person person;
        private Activity activity;
        private DateTime recordDate;
        private double burnedCalories;
        private string intesityLevel;
        private MetValues metValues;

        public Record() { } 

        public Record(int recordId, Person person, Activity activity, DateTime recordDate, double burnedCalories, string intesityLevel, MetValues metValues)
        {
            this.recordId = recordId;
            this.person = person;
            this.activity = activity;
            this.recordDate = recordDate;
            this.burnedCalories = burnedCalories;
            this.intesityLevel = intesityLevel;
            this.metValues = metValues;
        }

        public int RecordId { get => recordId; set => recordId = value; }
        public Person Person { get => person; set => person = value; }
        public Activity Activity { get => activity; set => activity = value; }
        public DateTime RecordDate { get => recordDate; set => recordDate = value; }
        public double BurnedCalories { get => burnedCalories; set => burnedCalories = value; }
        public string IntesityLevel { get => intesityLevel; set => intesityLevel = value; }
        public MetValues MetValues { get => metValues; set => metValues = value; }
    }
}
