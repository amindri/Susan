using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstInFirstAid.Models
{
    public class ClientContact
    {
        public int Id { get; set; }
        public string ContactName { get; set; }
        public string ContactPosition { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }

        public Client Client { get; set; }
        public EventSegment EventSegment { get; set; }

    }
}