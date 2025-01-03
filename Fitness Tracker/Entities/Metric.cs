using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_Tracker.Entities
{
    public class Metric
    {
        private int metricId;
        private Activity activity;
        private string metricName;
        private double calculationFactor;
        public Metric() 
        {

        }
        public Metric(int metricId, Activity activity, string metricName, double calculationFactor)
        {
            this.metricId = metricId;
            this.activity = activity;
            this.metricName = metricName;
            this.calculationFactor = calculationFactor;
        }

        public int MetricId { get => metricId; set => metricId = value; }
        public Activity Activity { get => activity; set => activity = value; }
        public string MetricName { get => metricName; set => metricName = value; }
        public double CalculationFactor { get => calculationFactor; set => calculationFactor = value; }
    }
}
