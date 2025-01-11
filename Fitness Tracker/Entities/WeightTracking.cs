using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_Tracker.Entities
{
    public class WeightTracking
    {
        private int weightTrackingId;
        private Person person;
        private double currentWeight;
        private DateTime recordedDate;

        public WeightTracking() { }

        public WeightTracking(int weightTrackingId, Person person, double currentWeight, DateTime recordedDate)
        {
            this.weightTrackingId = weightTrackingId;
            this.person = person;
            this.currentWeight = currentWeight;
            this.recordedDate = recordedDate;
        }

        public int WeightTrackingId { get => weightTrackingId; set => weightTrackingId = value; }
        public Person Person { get => person; set => person = value; }
        public double CurrentWeight { get => currentWeight; set => currentWeight = value; }
        public DateTime RecordedDate { get => recordedDate; set => recordedDate = value; }
    }
}
