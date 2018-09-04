using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FirstInFirstAid.Models
{
    public enum BusinessId { FISFA, FA_SERVICES }

    public enum EventState { YES, NO, CANCELLED, POSTPONED, QUOTE_NO_DATE, NOT_ACCEPTED}

    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Event name is missing")]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Invoice number is missing")]
        public string InvoiceNumber { get; set; }

        [Required(ErrorMessage = "Hourly rate is missing")]
        public double HourlyRate { get; set; }             

        [Required(ErrorMessage = "Business id is missing")]
        public BusinessId BusinessId { get; set; }

        [Required(ErrorMessage = "Event state is missing")]
        public EventState EventState { get; set; }

        public Client Client { get; set; }

        public ICollection<EventSegment> EventSegments { get; set; }

 
    }
}