using System;
using System.ComponentModel.DataAnnotations;

namespace FirstInFirstAid.Models
{
    public enum DutyType {COVERAGE, TBC_COVERAGE, TRAINING, SUSAN_COVERAGE, ADMIN, PERSONAL}

    public class TrainorAllocationForEventSeg
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Duty type is missing")]
        public DutyType DutyType { get; set; }

        [Required(ErrorMessage = "Presence confirmation is missing")]
        public Boolean PresenceConfirmation { get; set; }

        public String PaymentNote { get; set; }

        [Required(ErrorMessage = "Hours is missing")]
        public long Hours { get; set; }


        public Boolean Paid { get; set; }

        public EventSegment EventSegment { get; set; }
        public Trainor Trainor { get; set; }

    }
}