using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FirstInFirstAid.Models
{
    public class Trainor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime DOB { get; set; }      
        public string TaxFileNo { get; set; }

        public Address Address { get; set; }
        public ICollection<Qualification> Qualifications { get; set; }        
    }
}