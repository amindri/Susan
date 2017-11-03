using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FirstInFirstAid.Models
{
    public enum BusinessId { FISFA, FA_SERVICES }

    public enum EventState { YES, NO, CANCELLED, POSTPONED, QUOTE_NO_DATE, NOT_ACCEPTED}

    public class Event
    {
        [Key, ForeignKey("Client")]
        public int Id { get; set; }
        public string EventName { get; set; }        
        public string InvoiceNumber { get; set; }
        public long HourlyRate { get; set; }
        public long TotalFee { get; set; }
        public ICollection<EventSegment> EventSegments { get; set; }
        private BusinessId BusinessId { get; set; }
        public EventState EventState { get; set; }

        public Client Client { get; set; }
    }
}