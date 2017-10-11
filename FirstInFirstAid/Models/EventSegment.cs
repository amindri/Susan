using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstInFirstAid.Models
{
    public class EventSegment
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long Hours { get; set; }
        public int RequiredNumberOfstaff { get; set; }

        public Venue Venue { get; set; }
        public ICollection<TrainorAllocationForEventSeg> TrainorAllocations { get; set; }
        public Event Event { get; set; }
    }
}