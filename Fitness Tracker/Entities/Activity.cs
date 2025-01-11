using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_Tracker.Entities
{
    public class Activity
    {
        private int activityId;
        private string activityName;
        private string descriptions;
        public Activity() 
        { 

        }

        public Activity(int activityId, string activityName, string descriptions)
        {
            this.activityId = activityId;
            this.activityName = activityName;
            this.descriptions = descriptions;
        }

        public int ActivityId { get => activityId; set => activityId = value; }
        public string ActivityName { get => activityName; set => activityName = value; }
        public string Descriptions { get => descriptions; set => descriptions = value; }
    }
}
