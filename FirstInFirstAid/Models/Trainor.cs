using FirstInFirstAid.Helpers;
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
        [RegularExpression("(\\d{10})", ErrorMessage = "Please enter a valid 10 digit phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email Address is missing")]
        [EmailAddress(ErrorMessage = "Invalid email address. Please use \'yourname@example.com\' format")]    
        [CustomRemoteValidation("TrainorExists", "Trainors", AdditionalFields = "Id", ErrorMessage = "Email Address already in use")]
        public string Email { get; set; }

        [Required(ErrorMessage = "DOB is missing")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Tax file number is missing")]
        public string TaxFileNo { get; set; }

        public Address Address { get; set; }

        public ICollection<Qualification> Qualifications { get; set; }        
    }
}