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
        [Key]
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm:ss}")]
        public DateTime StartTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm:ss}")]
        public DateTime EndTime { get; set; }

        public long Hours { get; set; }
        public int RequiredNumberOfStaff { get; set; }

        public ICollection<TrainorAllocationForEventSeg> TrainorAllocations { get; set; }
        
        public Venue Venue { get; set; }      
        public Event Event { get; set; }
        public ClientContact ClientContact { get; set; }
    }
}