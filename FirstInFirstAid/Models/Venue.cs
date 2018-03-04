using System;
using System.ComponentModel.DataAnnotations;

namespace FirstInFirstAid.Models
{
    public class Venue
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Venue name is missing")]
        public String VenueName { get; set; }

        public Address Address { get; set; }
    }
}