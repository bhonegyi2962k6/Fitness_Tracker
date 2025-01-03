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
        private decimal weight;
        private DateTime recordedDate;

        private WeightTracking() { }
        public WeightTracking(int weightTrackingId, Person person, decimal weight, DateTime recordedDate)
        {
            this.weightTrackingId = weightTrackingId;
            this.person = person;
            this.weight = weight;
            this.recordedDate = recordedDate;
        }

        public int WeightTrackingId { get => weightTrackingId; set => weightTrackingId = value; }
        public Person Person { get => person; set => person = value; }
        public decimal Weight { get => weight; set => weight = value; }
        public DateTime RecordedDate { get => recordedDate; set => recordedDate = value; }
    }
}
