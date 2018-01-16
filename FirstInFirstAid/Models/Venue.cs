using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FirstInFirstAid.Models
{
    public class Venue
    {
        public int Id { get; set; }
        public String VenueName { get; set; }

        public Address one { get; set; }
    }
}