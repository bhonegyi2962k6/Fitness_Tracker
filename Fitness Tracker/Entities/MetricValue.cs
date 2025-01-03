using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_Tracker.Entities
{
    public class MetricValue
    {
        private int metricValueId;
        private Activity activity;
        private Metric metric;
        private double value;
        private string unit;
        private DateTime recordedDate;

        public MetricValue() { }
        public MetricValue(int metricValueId, Activity activity, Metric metric, double value, string unit, DateTime recordedDate)
        {
            this.metricValueId = metricValueId;
            this.activity = activity;
            this.metric = metric;
            this.value = value;
            this.unit = unit;
            this.recordedDate = recordedDate;
        }

        public int MetricValueId { get => metricValueId; set => metricValueId = value; }
        public Activity Activity { get => activity; set => activity = value; }
        public Metric Metric { get => metric; set => metric = value; }
        public double Value { get => value; set => this.value = value; }
        public string Unit { get => unit; set => unit = value; }
        public DateTime RecordedDate { get => recordedDate; set => recordedDate = value; }
    }
}
