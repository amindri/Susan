using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FirstInFirstAid.Models
{
    public class Qualification
    {
        [Key]
        public int Id { get; set; }
        public string QualificationName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime DateAttained { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime ExpiryDate { get; set; }
        public Trainor Trainor { get; set; }
    }
}