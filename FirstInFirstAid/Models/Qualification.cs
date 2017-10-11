using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstInFirstAid.Models
{
    public class Qualification
    {
        public int Id { get; set; }
        public string QualificationName { get; set; }            
        public DateTime DateAttained { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string LastEditor { get; set; }
        public DateTime LastedEdited { get; set; }

        public Trainor Trainor { get; set; }


    }
}