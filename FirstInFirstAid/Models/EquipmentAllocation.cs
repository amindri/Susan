using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FirstInFirstAid.Models
{
    public class EquipmentAllocation
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateSupplied { get; set; }
        public DateTime DateReturned { get; set; }
        public Equipment Equipment { get; set; }
        public Trainor Trainor { get; set; }    
    }
}