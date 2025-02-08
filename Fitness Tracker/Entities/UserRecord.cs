using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_Tracker.Entities
{
    public class UserRecord
    {
        private Person person;
        private Record record;

        public UserRecord() 
        { 

        }
        public UserRecord(Person person, Record record)
        {
            this.person = person;
            this.record = record;
        }

        public Person Person { get => person; set => person = value; }
        public Record Record { get => record; set => record = value; }
    }
}
