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
        public string ContactPhone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address. Please use \'yourname@example.com\' format")]
        public string ContactEmail { get; set; }

        public Client Client { get; set; }
        public ICollection<EventSegment> EventSegments { get; set; }

    }
}