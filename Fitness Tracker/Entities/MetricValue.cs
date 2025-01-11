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
        private Record record;
        private Metric metric;
        private double value;

        public MetricValue() { }
        public MetricValue(int metricValueId, Activity activity, Record record, Metric metric, double value)
        {
            this.metricValueId = metricValueId;
            this.activity = activity;
            this.record = record;
            this.metric = metric;
            this.value = value;
        }

        public int MetricValueId { get => metricValueId; set => metricValueId = value; }
        public Activity Activity { get => activity; set => activity = value; }
        public Record Record { get => record; set => record = value; }
        public Metric Metric { get => metric; set => metric = value; }
        public double Value { get => value; set => this.value = value; }
    }
}
