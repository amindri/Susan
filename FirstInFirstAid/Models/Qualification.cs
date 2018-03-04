using System;
using System.ComponentModel.DataAnnotations;

namespace FirstInFirstAid.Models
{
    public class Qualification
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Qualification name is missing")]
        public string QualificationName { get; set; }

        [Required(ErrorMessage = "Date attained is missing")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime DateAttained { get; set; }

        [Required(ErrorMessage = "Date of expiry is missing")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime ExpiryDate { get; set; }

        public Trainor Trainor { get; set; }
    }
}