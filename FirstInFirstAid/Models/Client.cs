using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstInFirstAid.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactName { get; set; }
        public string ContactPosition { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }   
        
        public Event Event { get; set; }

    }
}