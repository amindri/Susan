using System;
using System.ComponentModel.DataAnnotations;

namespace FirstInFirstAid.Models
{
    public class EquipmentAllocation
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Supplied date is missing")]
        public DateTime DateSupplied { get; set; }

        public DateTime DateReturned { get; set; }

        [Required(ErrorMessage = "Trainer is missing")]
        public Trainor Trainor { get; set; }

        [Required]
        public Equipment Equipment { get; set; }
    }
}