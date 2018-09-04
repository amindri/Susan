using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FirstInFirstAid.Models
{
    public enum Coverage { COVERAGE = 1, TBC_COVERAGE = 2, TRAINING = 3, SUSAN_COVERAGE = 4, ADMIN = 5, PERSONAL = 6, PROFESSIONAL_DEVELOPMENT = 7 }

    public class EventSegment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Event segment name is missing")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Coverage is missing")]
        public Coverage Coverage { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Start time is missing")]
        public DateTime StartTime { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "End time is missing")]
        public DateTime EndTime { get; set; }

        public double Hours { get; set; }
                
        public double TotalFee { get; set; }

        [Required(ErrorMessage = "Number off staff is missing")]
        public int RequiredNumberOfStaff { get; set; }

        public List<TrainorAllocationForEventSeg> TrainorAllocations { get; set; }
        
        public Venue Venue { get; set; }

        [Required]
        public Event Event { get; set; }

        public ClientContact ClientContact { get; set; }
    }
}