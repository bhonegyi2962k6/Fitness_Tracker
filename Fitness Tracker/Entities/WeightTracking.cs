using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_Tracker.Entities
{
    public class WeightTracking
    {
        private int weightId;
        private Person person;
        private DateTime recorededDate;
        private double weight;
        public WeightTracking() { }

        public WeightTracking(int weightId, Person person, DateTime recorededDate, double weight)
        {
            this.weightId = weightId;
            this.person = person;
            this.recorededDate = recorededDate;
            this.weight = weight;
        }

        public int WeightId { get => weightId; set => weightId = value; }
        public Person Person { get => person; set => person = value; }
        public DateTime RecorededDate { get => recorededDate; set => recorededDate = value; }
        public double Weight { get => weight; set => weight = value; }
    }
}
