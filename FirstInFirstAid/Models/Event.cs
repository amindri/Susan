using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FirstInFirstAid.Models
{
    public enum BusinessId { FISFA, FA_SERVICES }

    public enum EventState { YES = 1, NO = 2, CANCELLED = 3, POSTPONED = 4, QUOTE_NO_DATE = 5, NOT_ACCEPTED = 6}

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