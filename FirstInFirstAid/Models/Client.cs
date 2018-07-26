using FirstInFirstAid.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FirstInFirstAid.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Client name is missing")]        
        [CustomRemoteValidation("ClientExists", "Clients", AdditionalFields = "Id", ErrorMessage = "Client Name already in use")]
        public string Name { get; set; }
        
        public Address Address { get; set; }
        public ICollection<Event> Events { get; set; }
        public ICollection<ClientContact> ClientContacts { get; set; }

    }
}