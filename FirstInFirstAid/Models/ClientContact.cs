using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FirstInFirstAid.Models
{
    public class ClientContact
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is missing")]
        public string ContactName { get; set; }

        public string ContactPosition { get; set; }
        [RegularExpression("(\\d{10})", ErrorMessage = "Please enter a valid 10 digit phone number")]
        public string ContactPhone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address. Please use \'yourname@example.com\' format")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                            @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                            ErrorMessage = "Please enter a valid email address")]
        public string ContactEmail { get; set; }

        public Client Client { get; set; }
        public ICollection<EventSegment> EventSegments { get; set; }

    }
}