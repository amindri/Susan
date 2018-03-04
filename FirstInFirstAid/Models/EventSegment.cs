using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FirstInFirstAid.Models
{
    public class EventSegment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Event Segment Name missing")]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm:ss}")]
        [Required(ErrorMessage = "Start Time missing")]
        public DateTime StartTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm:ss}")]
        [Required(ErrorMessage = "End Time missing")]
        public DateTime EndTime { get; set; }

        public long Hours { get; set; }

        [Required(ErrorMessage = "Number Of Staff missing")]
        public int RequiredNumberOfStaff { get; set; }

        public ICollection<TrainorAllocationForEventSeg> TrainorAllocations { get; set; }
        
        public Venue Venue { get; set; }      

        public Event Event { get; set; }

        public ClientContact ClientContact { get; set; }
    }
}