using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_Tracker.Entities
{
    public class MetValues
    {
        private int metId;
        private Activity activity;
        private string intensityLevel;
        private double metValue;

        public MetValues()
        {

        }

        public MetValues(int metId, Activity activity, string intensityLevel, double metValue)
        {
            this.metId = metId;
            this.activity = activity;
            this.intensityLevel = intensityLevel;
            this.metValue = metValue;
        }

        public int MetId { get => metId; set => metId = value; }
        public Activity Activity { get => activity; set => activity = value; }
        public string IntensityLevel { get => intensityLevel; set => intensityLevel = value; }
        public double MetValue { get => metValue; set => metValue = value; }
    }
}
