using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FirstInFirstAid.Models
{
    public class Trainor
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is missing")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is missing")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Phone number is missing")]
        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address. Please use \'yourname@example.com\' format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "DOB is missing")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Tax file number is missing")]
        public string TaxFileNo { get; set; }

        public Address Address { get; set; }

        public ICollection<Qualification> Qualifications { get; set; }        
    }
}