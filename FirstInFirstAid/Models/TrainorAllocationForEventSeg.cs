using System;
using System.ComponentModel.DataAnnotations;

namespace FirstInFirstAid.Models
{

    public class TrainorAllocationForEventSeg
    {
        [Key]
        public int Id { get; set; }              

        [Required(ErrorMessage = "Presence confirmation is missing")]
        public Boolean PresenceConfirmation { get; set; }

        public String PaymentNote { get; set; }

        [Required(ErrorMessage = "Hours is missing")]
        public long Hours { get; set; }


        public Boolean Paid { get; set; }

        [Required]
        public EventSegment EventSegment { get; set; }

        public Trainor Trainor { get; set; }

    }
}