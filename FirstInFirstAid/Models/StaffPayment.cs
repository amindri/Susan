using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstInFirstAid.Models
{
    public class StaffPayment
    {
        public int Id { get; set; }
        //public string Status { get; set; }
        //public DateTime StartTime { get; set; }
        //public DateTime FinishTime { get; set; }
        public long HourlyRateATT { get; set; }
        public long Hours { get; set; }
        public long TotalPayment { get; set; }
        public Boolean Paid { get; set; }
        //public String PaidStandard { get; set; } // check for correct type
        //public long ExtraHours { get; set; }
        //public Boolean PaidExtra { get; set; } //check for correct type
        //public DateTime LastEdited { get; set; }
        //public string LastEditor { get; set; }



        public Trainor Staff { get; set; }
        public Event Event { get; set; }
    }
}