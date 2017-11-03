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
        
        public Event Event { get; set; }
        public ICollection<ClientContact> ClientContacts { get; set; }

    }
}