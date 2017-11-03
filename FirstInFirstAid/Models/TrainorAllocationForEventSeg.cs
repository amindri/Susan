using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstInFirstAid.Models
{
    public enum DutyType {COVERAGE, TBC_COVERAGE, TRAINING, SUSAN_COVERAGE, ADMIN, PERSONAL}

    public class TrainorAllocationForEventSeg
    {
        public int Id { get; set; }
        public Trainor Trainor { get; set; }
        public DutyType DutyType { get; set; }
        public Boolean PresenceConfirmation { get; set; }
        public String PaymentNote { get; set; }
        public long Hours { get; set; }
        public Boolean Paid { get; set; }

        public EventSegment EventSegment { get; set; }
    }
}