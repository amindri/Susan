using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FirstInFirstAid.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Client name is missing")]
        public string Name { get; set; }
        
        public Address Address { get; set; }
        public ICollection<Event> Events { get; set; }
        public ICollection<ClientContact> ClientContacts { get; set; }

    }
}