using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FirstInFirstAid.Models
{
    public class EventSegment
    {
        [Key, ForeignKey("ClientContact")]
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long Hours { get; set; }
        public int RequiredNumberOfstaff { get; set; }

        public Venue Venue { get; set; }
        public ICollection<TrainorAllocationForEventSeg> TrainorAllocations { get; set; }
        public Event Event { get; set; }
        public ClientContact ClientContact { get; set; }
    }
}